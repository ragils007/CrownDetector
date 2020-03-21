using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Tools.ExpressionTrees
{
    public static class MemberExpressionExtensions
    {
        public static List<TAttribute> GetAttributeList<TAttribute>(this MemberExpression mExp)
            where TAttribute: Attribute
        {
            return MyAttributes.GetAttributeOfTypeList<TAttribute>(mExp);
        } 
    }
}
