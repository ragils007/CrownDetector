using System;
using System.Linq;

namespace Msdfa.Core.Tools
{
    public class ConvertEnum
    {
        public static TDst Translate<TSrc, TDst>(TSrc itemIn, bool compareId = false)
           where TSrc : struct
           where TDst : struct
        {
            var TypeS = typeof(TSrc).Name;
            var TypeD = typeof(TDst).Name;

            if (!typeof(TSrc).IsEnum || !typeof(TDst).IsEnum) throw new Exception("Not an enum!");

            var strSrc = itemIn.ToString();
            var valSrc = Convert.ToInt64(itemIn);

            var items = Enum.GetValues(typeof(TDst)).Cast<object>();

            foreach (var item in Enum.GetValues(typeof(TDst)).Cast<object>().Where(item => strSrc == item.ToString()))
            {
                if (compareId && (long)item != valSrc) continue;
                return (TDst)item;
            }
            throw new Exception(string.Format("Unable to translate [({0}){1}] to type [{2}] (compare Id: {3})", valSrc, strSrc, typeof(TDst).Name, compareId ? "on" : "off"));
        }
    }
}
