using Msdfa.Core.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools
{
    public class TempDirectory
    {
        public string PathTemp;

        public TempDirectory()
        {
            var guid = Guid.NewGuid();
            // 2018.05.08 problem nie dziala na serwerze 1.31
            //var pathTemp = Path.GetTempPath();
            // tez czasami nie dziala ?
            //var pathTemp = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Temp";

            var pathTemp = $@"{AssemblyExtensions.GetAssemblyDirectory()}\Temp";

            if (pathTemp.Contains("#"))
                throw new Exception("Znak # jest niedozwolony w ścieżce");

            if (!Directory.Exists(pathTemp)) Directory.CreateDirectory(pathTemp);

            PathTemp = $@"{pathTemp}\{guid}\";
        }

        public TempDirectory(string pathTemp)
        {
            PathTemp = pathTemp;
        }

        public void SetTempName(string directoryName)
        {
            var guid = Guid.NewGuid();
            var pathTemp = $@"{AssemblyExtensions.GetAssemblyDirectory()}\{directoryName}";
            PathTemp = $@"{pathTemp}\{guid}\";
        }


        public void Create()
        {
            Directory.CreateDirectory(PathTemp);
        }

        public void Delete()
        {
            if (Directory.Exists(PathTemp)) Directory.Delete(PathTemp);
        }
    }
}
