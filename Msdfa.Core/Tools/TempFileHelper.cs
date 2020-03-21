using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools
{
    public static class TempFileHelper
    {
        public static string SaveInTempDirectory(string path, string fileName, byte[] data)
        {
            var pathTemp = System.IO.Path.GetTempPath();
            pathTemp += path;
            if (!Directory.Exists(pathTemp))
                Directory.CreateDirectory(pathTemp);

            var filePath = pathTemp + "\\" + fileName;
            File.WriteAllBytes(filePath, data);

            return filePath;
        }

        public static string GetTempFileName(string extension = null)
        {
            return $"{System.IO.Path.GetTempPath()}{Guid.NewGuid()}{extension}";
        }
    }
}