using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools.Extensions
{
    public static class DirectoryExtensions
    {
        /// <summary>
        /// Sprawdzenie czy aktualny proces programu ma prawo do zapisu w katalogu
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsAccesListFile(string pathDirectory)
        {
            try
            {
                if (!Directory.Exists(pathDirectory))
                    return false;
            }
            catch
            {
                return false;
            }

            /*
             * Uzytkownicy moga nie miec prawa do FileSystemRights.ReadPermissions
             * dlatego jest uzyte wylacznie test odczytu listy
             */

            try
            {
                var list = Directory.GetFiles(pathDirectory);
            }
            catch
            {
                return false;
            }


            /* 
            DirectorySecurity dSecurity = Directory.GetAccessControl(pathDirectory);

            foreach (FileSystemAccessRule rule in dSecurity.GetAccessRules(true, true, typeof(NTAccount)))
            {
                if (rule.FileSystemRights != FileSystemRights.ListDirectory 
                    && rule.FileSystemRights != FileSystemRights.FullControl)
                {
                    return false;
                }
            }
            */

            return true;
        }

    }
}
