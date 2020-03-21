using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools
{
    public static class OfficeTool
    {
        public static bool IsWordInstalled
        {
            get
            {
                return Type.GetTypeFromProgID("Word.Application") != null;
            }
        }

        public static bool IsExcellInstalled
        {
            get
            {
                return Type.GetTypeFromProgID("Excel.Application") != null;
            }
        }
    }
}
