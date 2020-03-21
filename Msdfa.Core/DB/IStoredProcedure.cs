using System.Collections.Generic;

namespace Msdfa.DB
{
    public interface IStoredProcedure
    {
        IStoredProcedure Bind(string varName, object varValue);
        IStoredProcedure BindAll(Dictionary<string, object> values);

        void Execute();
    }
}