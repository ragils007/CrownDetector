using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools.Extensions
{
    /// <summary>
    /// Helper methods for the lists.
    /// </summary>
    public static class ListExtensions
    {
        public static List<List<T>> SplitByItemsCount<T>(this List<T> source, int splitSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / splitSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
