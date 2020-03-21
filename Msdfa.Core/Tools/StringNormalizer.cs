using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Tools
{
   public class StringNormalizer
   {
      public enum Mode
      {
         Default,
         UPPER_CASE_UNDERSCORED,
         camelCase,
         PascalCase
      }

      public static string SrcChars = "ąćęłńóśżź ĄĆĘŁŃÓŚŻŹ";
      public static string DstChars = "acelnoszz_ACELNOSZZ";
      public static Mode NormalizeMode = Mode.UPPER_CASE_UNDERSCORED;

      public static string Normalize(string inputString, Mode mode = Mode.Default)
      {
         if (mode == Mode.Default)mode = NormalizeMode;

         if (SrcChars.Length != DstChars.Length) throw new Exception("Różna długość ciągów do zmiany znaków");
         bool nextCharCased = true;

         if (mode == Mode.UPPER_CASE_UNDERSCORED) inputString = inputString.ToUpper().Replace(' ', '_');
         else inputString = inputString.Replace('_', ' ');
         
         if (mode == Mode.camelCase) nextCharCased = false;

         string output = "";

         foreach (var c in inputString)
         {
            var chr = c;
            if (c == ' ') nextCharCased = true;
            else
            {
               var pos = SrcChars.IndexOf(c);
               if (pos >= 0) chr = DstChars[pos];

               output += nextCharCased ? chr.ToString().ToUpper() : chr.ToString().ToLower();
               if (mode != Mode.UPPER_CASE_UNDERSCORED) nextCharCased = false;
            }
         }
         return output;
      }
   }
}
