using Msdfa.Core.DB;

namespace Msdfa.Core.Base
{
    public interface IBaseTableGeneric
    {
        object GetId();
        void SetId(object id);
        void RefreshFromDB(IConnection cnn);
    }
}