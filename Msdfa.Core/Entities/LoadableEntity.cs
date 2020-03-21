using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Msdfa.Core.Entities
{
    public class LoadableEntity<TItem, TConnection>
        where TItem : class
        where TConnection : class
    {
        public Func<long?> ExpectedId { get; set; }
        private TItem _item;

        public TItem Item
        {
            get { return this.Get(); }
            set
            {
                if (this.ActionSetId != null) this.ActionSetId(value);
                else this.IdPropertyInfo.SetValue(_parent, this.FuncId(value));
                this._item = value;
            }

        }
        protected Func<TItem, long?> FuncId { get; set; }
        protected Func<TConnection, long, TItem> FuncGet { get; set; }

        /// <summary>
        /// W normalnych przypadkach nie trzeba wypełniać ActionSet. Refleksja będzie modyfikowała pole ID 
        /// przez rozpoznanie metody zczytującej ID pola z rodzica.
        /// W przypadkach nietrywialnych, można zaimplementować swoją metodę uaktualnienia (np. modyfikacja
        /// pola Towar w TowarDostawca powinna uaktualnić zarówno TowarId, jak i TowarKod)
        /// </summary>
        protected Action<TItem> ActionSetId { get; set; }

        private object _parent;
        protected PropertyInfo IdPropertyInfo { get; set; }

        public LoadableEntity(Expression<Func<long?>> expectedId, Func<TItem, long?> funcId, Func<TConnection, long, TItem> funcGet)
        {
            this.ExpectedId = expectedId.Compile();
            this.FuncId = funcId;
            this.FuncGet = funcGet;

            var memExp = (expectedId.Body as UnaryExpression).Operand as MemberExpression;
            if (memExp == null) throw new Exception("Błąd parsowania Expression");
            this.IdPropertyInfo = memExp.Member as PropertyInfo;
            this._parent = (memExp.Expression as ConstantExpression)?.Value;
        }

        public LoadableEntity(Expression<Func<long?>> expectedId, Func<TItem, long?> funcId, Func<TConnection, long, TItem> funcGet, Action<TItem> actionSetId)
        {
            this.ExpectedId = expectedId.Compile();
            this.FuncId = funcId;
            this.FuncGet = funcGet;
            this.ActionSetId = actionSetId;
        }

        public TItem Load(TConnection cnn)
        {
            if (cnn == null) throw new Exception("Unable to fetch entity, no connection given");

            var expectedId = this.ExpectedId();
            this._item = expectedId.HasValue ? this.FuncGet(cnn, expectedId.Value) : null;
            return this._item;
        }

        public TItem Get(TConnection cnn = null)
        {
            var expectedId = this.ExpectedId();
            if (expectedId == null) return null;

            if (this._item == null) return this.Load(cnn);
            if (!expectedId.Equals(this.FuncId(this._item))) return this.Load(cnn);

            return this._item;
        }

        public static implicit operator TItem(LoadableEntity<TItem, TConnection> item) => item.Item;
    }
}
