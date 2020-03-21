using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Tools.ExpressionTrees
{
    public class ExpressionDetail
    {
        public static string GetPropertyPath(MemberExpression expression)
        {
            var items = GetPropertyPathList(expression);
            var ret = string.Join(".", (from item in items select item.Value));
            return ret;
        }

        /// <summary>
        /// Returns type of expression Traversing down MemberExpressions
        /// </summary>
        /// <param name="e"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ExpressionType GetExpressionType(Expression e, ExpressionType? type = null)
        {
            if (type == null) type = e.NodeType;

            switch (e.NodeType)
            {
                case ExpressionType.Constant:
                    type = ExpressionType.Constant;
                    break;
                case ExpressionType.MemberAccess:
                    var mExp = e as MemberExpression;
                    if (mExp == null) throw new Exception("No MemberExpression in MemberAccess found");
                    if (mExp.Expression != null) return GetExpressionType(mExp.Expression, type);
                    break;
                case ExpressionType.Parameter:
                    type = ExpressionType.Parameter;
                    break;
                case ExpressionType.Call:
                    type = ExpressionType.Call;
                    break;
                default:
                    throw new Exception("Unhandled expression");
            }
            return type.Value;
        }

        public static List<string> GetPropertyPath2(Expression e)
        {
            var items = GetPropertyPathTraverse(e);
            items.Reverse();
            return items;
        }

        public static string GetPropertyName<TModel>(Expression<Func<TModel, object>> expression)
        {
            var ex = (MemberExpression) expression.Body;
            return ex.Member.Name;
        }

        private static List<string> GetPropertyPathTraverse(Expression e, List<string> path = null)
        {
            if (path == null) path = new List<string>();

            switch (e.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var mExp = e as MemberExpression;
                    if (mExp == null) throw new Exception("No MemberExpression in MemberAccess found");

                    path.Add(mExp.Member.Name);
                    if (mExp.Expression != null) GetPropertyPathTraverse(mExp.Expression, path);
                    break;
                case ExpressionType.Parameter:
                    var tExp = e as ParameterExpression;
                    if (tExp != null) path.Add(tExp.Name);
                    break;
                default:
                    throw new Exception("Unhandled expression");
            }
            return path;
        }

        public static string GetPropertyPathTrimLastElement(MemberExpression expression)
        {
            var items = GetPropertyPathList(expression);
            items.RemoveAt(items.Count - 1);
            return string.Join(".", (from item in items select item.Value));
        }

        public static string GetPropertyPath<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            return GetPropertyPath(expression.Body as MemberExpression);
        }

        public static List<KeyValuePair<Type, string>> GetPropertyPathList(MemberExpression expression)
        {
//         var list = GetMembersOnPath(expression);

            var members = GetMembersOnPath(expression)
                .Select(
                    x =>
                        x.Member.MemberType == MemberTypes.Property
                            ? new KeyValuePair<Type, string>(((PropertyInfo) x.Member).PropertyType, x.Member.Name)
                            : x.Member.MemberType == MemberTypes.Field
                                ? new KeyValuePair<Type, string>(((FieldInfo) x.Member).FieldType, x.Member.Name)
                                : new KeyValuePair<Type, string>())
                .Reverse()
                .ToList();

            return members;
        }

        public static List<KeyValuePair<Type, string>> GetPropertyPathList<T, TResult>(
            Expression<Func<T, TResult>> expression)
        {
            return GetPropertyPathList(expression.Body as MemberExpression);
        }

        public static Expression GetFinalExpression(MemberExpression mExp)
        {
            var expressionsPath = GetMembersOnPath(mExp);
            return expressionsPath.Last();
        }

        private static IEnumerable<MemberExpression> GetMembersOnPath(MemberExpression expression)
        {
            while (expression != null)
            {
                yield return expression;
                expression = expression.Expression as MemberExpression;
            }
        }

        public static List<object> GetObjectStack(Expression expr)
        {
            var ret = new List<object>();

            var memberInfos = new Stack<MemberInfo>();

            // "descend" toward's the root object reference:
            while (expr is MemberExpression)
            {
                var memberExpr = expr as MemberExpression;
                memberInfos.Push(memberExpr.Member);
                expr = memberExpr.Expression;
            }

            // fetch the root object reference:
            var constExpr = expr as ConstantExpression;
            var objReference = constExpr.Value;
            MemberInfo mi = null;

            // "ascend" back whence we came from and resolve object references along the way:
            while (memberInfos.Count > 0)  // or some other break condition
            {
                if (objReference == null) throw new Exception($"Parent object [{mi?.Name}] is null");

                mi = memberInfos.Pop();
                if (mi.MemberType == MemberTypes.Property)
                {
                    objReference = objReference.GetType()
                                               .GetProperty(mi.Name)
                                               .GetValue(objReference, null);
                }
                else if (mi.MemberType == MemberTypes.Field)
                {
                    objReference = objReference.GetType()
                                               .GetField(mi.Name)
                                               .GetValue(objReference);
                }

                ret.Add(objReference);
            }

            return ret;
        }

        private ExpressionDetail()
        {
        }

        public MemberInfo MemberInfo { get; private set; }

        public LambdaExpression Expression { get; private set; }

        //By figuring out MemberInfo from the Expression,
        //we can now have all these read-only properties to get expression detail.
        public string Name
        {
            get { return MemberInfo.Name; }
        }

        public Type DeclaringType
        {
            get { return MemberInfo.DeclaringType; }
        }

        public string FullName
        {
            get { return DeclaringType.FullName + "." + Name; }
        }

        //Depending on performance requirement, you may want to use Lazy<T> to calculate this value
        //only once.
        public Delegate Delegate
        {
            get { return Expression.Compile(); }
        }

        //We are expecting a lambda expression which should either point to a method or a property access.
        //To get body, we have to handle the case of expression being UnaryExpression
        //To learn more: http://stackoverflow.com/questions/3567857/why-are-some-object-properties-unaryexpression-and-others-memberexpression
        private static Expression GetBody(LambdaExpression expression)
        {
            //We don't validate arguments here only because it's a private method.

            var unaryExpression = expression.Body as UnaryExpression;
            return unaryExpression != null ? unaryExpression.Operand : expression.Body;
        }

        //In your original method, you returned the name.
        //However, it could be even more useful to get the MemberInfo and store it.
        //Now we will have access to Name as well as the Type in which the property/method is declared.

        // There are lots of edge cases here.
        // Refer to: http://stackoverflow.com/questions/671968/retrieving-property-name-from-lambda-expression and update this method to handle the edge cases that you care about.
        private static MemberInfo GetMemberInfo(Expression expression)
        {
            //We don't validate arguments here only because it's a private method.

            var memberExpression = expression as MemberExpression;

            if (memberExpression != null)
                return memberExpression.Member;

            var methodCallExpression = expression as MethodCallExpression;

            if (methodCallExpression != null)
                return methodCallExpression.Method;

            return null;
        }

        public static ExpressionDetail Create(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var body = GetBody(expression);

            var memberInfo = GetMemberInfo(body);

            if (memberInfo == null)
                throw new InvalidExpressionException(
                    string.Format(
                        "The expression '{0}' is invalid. You must supply an expression that references a property or a function.",
                        expression.Body));

            return new ExpressionDetail {MemberInfo = memberInfo, Expression = expression};
        }

        //A lambda expression can be a valid expression referring to a property or function.
        //But for our need, we will need to compile this expression to a delegate and run on object of Type T, let's make
        //sure that expression refers to the correct type.

        //IMPORTANT: We should not put this check in ExpressionDetail class. It is not
        //the responsibility of ExpressionDetail to enforce this type constraint.
        //This type constraint is only needed for ObjectToDictionaryConverter class.
        public void CheckDeclaringType<T>()
        {
            if (this.DeclaringType != typeof(T))
            {
                throw new InvalidExpressionException("Expression " + this.Expression.Body +
                                                     " is invalid. Expression Property/Member Type " +
                                                     this.DeclaringType.FullName + ", expecting Type: " +
                                                     typeof(T).FullName);
            }
        }
    }
}