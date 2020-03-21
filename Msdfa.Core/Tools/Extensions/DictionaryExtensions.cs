using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools.Extensions
{
    public static class DictionaryExtensions
    {
        public static TResult GetOrDefault<TKey, TResult>(this Dictionary<TKey, TResult> dict, TKey key)
        {
            if (dict.ContainsKey(key))
                return dict[key];

            return default(TResult);
        }
    }
}
