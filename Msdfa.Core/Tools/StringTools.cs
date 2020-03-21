using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Msdfa.Core.Tools
{
	public static class StringTools
	{
		public static string PlNormalizeToEN(string txt)
		{
			char c = '?';

			for (int i = 0; i < txt.Length; i++)
			{
				c = txt[i];

				switch ((int)c)
				{
					case (char)'ą':
						c = 'a';
						break;

					case (char)'Ą':
						c = 'A';
						break;

					case (char)'ć':
						c = 'c';
						break;

					case (char)'Ć':
						c = 'C';
						break;

					case (char)'ę':
						c = 'e';
						break;

					case (char)'Ę':
						c = 'E';
						break;

					case (char)'ł':
						c = 'l';
						break;

					case (char)'Ł':
						c = 'L';
						break;

					case (char)'ń':
						c = 'n';
						break;

					case (char)'Ń':
						c = 'N';
						break;

					case (char)'ś':
						c = 's';
						break;

					case (char)'Ś':
						c = 'S';
						break;

					case (char)'ó':
						c = 'o';
						break;

					case (char)'Ó':
						c = 'O';
						break;

					case (char)'ż':
						c = 'z';
						break;

					case (char)'Ż':
						c = 'Z';
						break;

					case (char)'ź':
						c = 'z';
						break;

					case (char)'Ź':
						c = 'Z';
						break;
				}

				txt = txt.Replace(txt[i], c);
			}

			return txt;
		}

		public static string CzNormalizeToEN(string txt)
		{
			char c = '?';

			for (int i = 0; i < txt.Length; i++)
			{
				c = txt[i];

				switch ((int)c)
				{
					case 0x158:// ě czasami jest rowne 344 ???
						c = (char)'e';
						break;
					case 0x2020: // Š czasami jest 8220
						c = (char)'S';
						break;
					case 0x150: // Ň czasami jest 336
						c = (char)'N';
						break;
					case 0x147:
						c = (char)'D';
						break;
					case 0x162: // Ů czasami jest 354
						c = (char)'U';
						break;
					case 0xD8: // ě
						c = (char)'e';
						break;
					case 0xE7: // š
						c = (char)'s';
						break;
					case 0x9F: // č
						c = (char)'c';
						break;
					case 0xFD: // ř
						c = (char)'r';
						break;
					case 0xA7: // ž
						c = (char)'z';
						break;
					case 0xEC: // ý
						c = (char)'y';
						break;
					case 0xA0: // á
						c = (char)'a';
						break;
					case 0xA1: // í
						c = (char)'i';
						break;
					case 0x82: // é
						c = (char)'e';
						break;
					case 0xA3: // ú
						c = (char)'u';
						break;
					case 0x85: // ů
						c = (char)'u';
						break;
					case 0x9C: // ť
						c = (char)'t';
						break;
					case 0xB7: // Ě
						c = (char)'E';
						break;
					case 0xE6: // Š
						c = (char)'S';
						break;
					case 0xAC: // Č
						c = (char)'C';
						break;
					case 0xFC: // Ř
						c = (char)'R';
						break;
					case 0xA6: // Ž
						c = (char)'Z';
						break;
					case 0xED: // Ý
						c = (char)'Y';
						break;
					case 0xB5: // Á
                        c = (char)'A';
						break;
					case 0xD6: // Í
						c = (char)'I';
						break;
					case 0x90: // É
						c = (char)'E';
						break;
					case 0xE9: // Ú
						c = (char)'U';
						break;
					case 0xDE: // Ů
						c = (char)'U';
						break;
					case 0x9B: // Ť
						c = (char)'T';
						break;
				}

				txt = txt.Replace(txt[i], c);
			}

			return txt;
		}

		public static string ConvertFromKeszhToCZ(string temp)
		{
			// czeskie znaki narodowe

			char c = '?';

			for (int i = 0; i < temp.Length; i++)
			{
				c = temp[i];

				switch ((int)c)
				{
					case 0x158:// ě czasami jest rowne 344 ???
						c = (char)'ě';
						break;
					case 0x2020: // Š czasami jest 8220
						c = (char)'Š';
						break;
					case 0x150: // Ň czasami jest 336
						c = (char)'Ň';
						break;
					case 0x162: // Ů czasami jest 354
						c = (char)'Ů';
						break;

					case 0xD8: // ě
						c = (char)'ě';
						break;
					case 0xE7: // š
						c = (char)'š';
						break;
					case 0x9F: // č
						c = (char)'č';
						break;
					case 0xFD: // ř
						c = (char)'ř';
						break;
					case 0xA7: // ž
						c = (char)'ž';
						break;
					case 0xEC: // ý
						c = (char)'ý';
						break;
					case 0xA0: // á
						c = (char)'á';
						break;
					case 0xA1: // í
						c = (char)'í';
						break;
					case 0x82: // é
						c = (char)'é';
						break;
					case 0xA3: // ú
						c = (char)'ú';
						break;
					case 0x85: // ů
						c = (char)'ů';
						break;
					case 0x9C: // ť
						c = (char)'ť';
						break;
					case 0xB7: // Ě
						c = (char)'Ě';
						break;
					case 0xE6: // Š
						c = (char)'Š';
						break;
					case 0xAC: // Č
						c = (char)'Č';
						break;
					case 0xFC: // Ř
						c = (char)'Ř';
						break;
					case 0xA6: // Ž
						c = (char)'Ž';
						break;
					case 0xED: // Ý
						c = (char)'Ý';
						break;
					case 0xB5: // Á
						c = (char)'Á';
						break;
					case 0xD6: // Í
						c = (char)'Í';
						break;
					case 0x90: // É
						c = (char)'É';
						break;
					case 0xE9: // Ú
						c = (char)'Ú';
						break;
					case 0xDE: // Ů
						c = (char)'Ů';
						break;
					case 0x9B: // Ť
						c = (char)'Ť';
						break;
				}

				temp = temp.Replace(temp[i], c);

			}

			return temp;
		}

		public static string ConvertFromKeszhBaseToUTF(string temp)
		{
			var returnValue = temp.ToArray<char>();

			char c = '?';

			for (int i = 0; i < returnValue.Length; i++)
			{
				c = returnValue[i];

				switch ((int)c)
				{
					case 0xB5: //Á	
						c = (char)'Á';	
						break;
					case 0xA0: //á	
						c = (char)'á';	
						break;
					case 0xAC: //Č	
						c = (char)'Č';	
						break;
					case 0x98: //č	
						c = (char)'č';	
						break;
					case 0xd2: //Ď	
						c = (char)'Ď';	
						break;
					case 0xD4: //ď	
						c = (char)'ď';	
						break;
					case (byte)0x90: //É	
						c = (char)'É';	
						break;
					case (byte)0x82: //é	
						c = (char)'é';	
						break;
					case (byte)0xB7: //Ě	
						c = (char)'Ě';	
						break;
					case (byte)0xD8: //ě	
						c = (char)'ě';	
						break;
					case (byte)0xD6: //Í	
						c = (char)'Í';	
						break;
					case (byte)0xA1: //í	
						c = (char)'í';	
						break;
					case (byte)0xD5: //Ň	
						c = (char)'Ň';	
						break;
					case (byte)0xE5: //ň	
						c = (char)'ň';	
						break;
					case (byte)0xD3: //Ó	
						c = (char)'Ó';	
						break;

					// znak zamieniany na duzy
					//case (byte)0xD3: //ó	
					//	c = (char)'ó';	
					//	break;

					case (byte)0xFC: //Ř	
						c = (char)'Ř';	
						break;
					case (byte)0xFD: //ř	
						c = (char)'ř';	
						break;
					case (byte)0x86: //Š	
						c = (char)'Š';	
						break;
					case (byte)0xE7: //š	
						c = (char)'š';	
						break;
					case (byte)0x9B: //Ť	
						c = (char)'Ť';	
						break;
					case (byte)0x8D: //ť	
						c = (char)'ť';	
						break;
					case (byte)0xE9: //Ú	
						c = (char)'Ú';	
						break;
					case (byte)0xAB: //ú	
						c = (char)'ú';	
						break;
					case (byte)0xDE: //Ů	
						c = (char)'Ů';	
						break;
					case (byte)0x85: //ů	
						c = (char)'ů';	
						break;
					case (byte)0xED: //Ý	
						c = (char)'Ý';	
						break;
					case (byte)0xEC: //ý	
						c = (char)'ý';	
						break;
					case (byte)0xA6: //Ž	
						c = (char)'Ž';	
						break;
					case (byte)0xA7: //ž	
						c = (char)'ž';	
						break;
				}
				returnValue[i] =  c;
			}

			return new string( returnValue);
		}

		public static string ConvertFromUTFToKeshBase(string temp)
		{
			var returnValue = temp.ToArray<char>();

			char c = '?';

			for (int i = 0; i < returnValue.Length; i++)
			{
				c = returnValue[i];

				switch ((int)c)
				{
					case (char)'Á': //Á
						c = (char)(byte)0xB5;
						break;
					case (char)'á': //á
						c = (char)(byte)0xA0;
						break;
					case (char)'Č': //Č
						c = (char)(byte)0xAC;
						break;
					case (char)'č': //č
						c = (char)(byte)0x98;
						break;
					case (char)'Ď': //Ď
						c = (char)0xd2;
						break;
					case (char)'ď': //ď
						c = (char)(byte)0xD4;
						break;
					case (char)'É': //É
						c = (char)(byte)0x90;
						break;
					case (char)'é': //é
						c = (char)(byte)0x82;
						break;
					case (char)'Ě': //Ě
						c = (char)(byte)0xB7;
						break;
					case (char)'ě': //ě
						c = (char)(byte)0xD8;
						break;
					case (char)'Í': //Í
						c = (char)(byte)0xD6;
						break;
					case (char)'í': //í
						c = (char)(byte)0xA1;
						break;
					case (char)'Ň': //Ň
						c = (char)(byte)0xD5;
						break;
					case (char)'ň': //ň
						c = (char)(byte)0xE5;
						break;
					case (char)'Ó': //Ó
						c = (char)(byte)0xD3;
						break;
					case (char)'ó': //ó
						c = (char)(byte)0xD3;
						break;
					case (char)'Ř': //Ř
						c = (char)(byte)0xFC;
						break;
					case (char)'ř': //ř
						c = (char)(byte)0xFD;
						break;
					case (char)'Š': //Š
						c = (char)(byte)0x86;
						break;
					case (char)'š': //š
						c = (char)(byte)0xE7;
						break;
					case (char)'Ť': //Ť
						c = (char)(byte)0x9B;
						break;
					case (char)'ť': //ť
						c = (char)(byte)0x8D;
						break;
					case (char)'Ú': //Ú
						c = (char)(byte)0xE9;
						break;
					case (char)'ú': //ú
						c = (char)(byte)0xAB;
						break;
					case (char)'Ů': //Ů
						c = (char)(byte)0xDE;
						break;
					case (char)'ů': //ů
						c = (char)(byte)0x85;
						break;
					case (char)'Ý': //Ý
						c = (char)(byte)0xED;
						break;
					case (char)'ý': //ý
						c = (char)(byte)0xEC;
						break;
					case (char)'Ž': //Ž
						c = (char)(byte)0xA6;
						break;
					case (char)'ž': //ž
						c = (char)(byte)0xA7;
						break;
                    case (char)'Ä': //ž
                        c = (char)0x17D;
                        break;
				}

				returnValue[i] =  c;
			}

			return new string(returnValue);
		}

		public static string ConvertFromSQLClientToUTF(string temp)
		{
			var returnValue = temp.ToArray<char>();

			char c = '?';

			for (int i = 0; i < returnValue.Length; i++)
			{
				c = returnValue[i];

				switch ((int)c)
				{
					// czeskie znaki narodowe

					case 0xB5: //Á	
						c = (char)'Á';
						break;
					case 0xA0: //á	
						c = (char)'á';
						break;
					case 0xAC: //Č	
						c = (char)'Č';
						break;
					case 0x98: //č	
						c = (char)'č';
						break;
					case 0x147: //Ď	
						c = (char)'Ď';
						break;
					case 0xD4: //ď	
						c = (char)'ď';
						break;
					case 0x90: //É	
						c = (char)'É';	
						break;
					case 0x201A: //é	
						c = (char)'é';	
						break;
					case 0xB7: //Ě	
						c = (char)'Ě';	
						break;
					case 0x158: //ě	
						c = (char)'ě';	
						break;
					case 0xD6: //Í	
						c = (char)'Í';	
						break;
					case 0x2C7: //í	
						c = (char)'í';	
						break;
					case 0x150: //Ň	
						c = (char)'Ň';	
						break;
					case 0x13A: //ň	
						c = (char)'ň';	
						break;		
					case (byte)0xD3: //Ó	
						c = (char)'Ó';	
						break;

				// znak zamieniany na duzy
				//case (byte)0xD3: //ó	
				//	c = (char)'ó';	
				//	break;

					case (byte)0xFC: //Ř	
						c = (char)'Ř';	
						break;
					case (byte)0xFD: //ř	
						c = (char)'ř';	
						break; 
					case 0x2020: //Š	
						c = (char)'Š';	
						break;
					case 0xE7: //š	
						c = (char)'š';	
						break;

					case 0x203A: //Ť	
						c = (char)'Ť';	
						break;
					case 0x164: //ť	
						c = (char)'ť';	
						break;
					case 0xE9: //Ú	
						c = (char)'Ú';	
						break;
					case 0xAB: //ú	
						c = (char)'ú';	
						break;
					case 0x162: //Ů	
						c = (char)'Ů';	
						break;
					case 0x2026: //ů	
						c = (char)'ů';	
						break;
					case 0xED: //Ý	
						c = (char)'Ý';	
						break;
					case 0x11B: //ý	
						c = (char)'ý';	
						break;
					case 0xA6: //Ž	
						c = (char)'Ž';	
						break;
					case 0xA7: //ž	
						c = (char)'ž';	
						break;

					// znaki krzaki
					case 337: //§
						c = (char)'§';
						break;
					case 259: //Đ	
						c = (char)'Đ';
						break;
					
				}
				returnValue[i] = c;
			}

			return new string(returnValue);
		}

		public static string ConvertFromUTFToSQLClient(string temp)
		{
			var returnValue = temp.ToArray<char>();

			char c = '?';

			for (int i = 0; i < returnValue.Length; i++)
			{
				c = returnValue[i];

				switch ((int)c)
				{
					case 0xC1: //Á
						c = (char)0xB5;
						break;
					case 225: //á
						c = (char)0xA0;
						break;
					case 268: //Č
						c = (char)0xAC;
						break;
					case 269: //č
						c = (char)0x98;
						break;
					case 270: //Ď
						c = (char)0x147;
						break;
					case 271: //ď
						c = (char)0xD4;
						break;
					case 201: //É
						c = (char)0x90;
						break;
					case 233: //é
						c = (char)0x201A;
						break;
					case 282: //Ě
						c = (char)0xB7;
						break;
					case 283: //ě
						c = (char)0x158;
						break;
					case 205: //Í
						c = (char)0xD6;
						break;
					case 237: //í
						c = (char)0x2C7;
						break;
					case 327: //Ň
						c = (char)0x150;
						break;
					case 328: //ň
						c = (char)0x13A;
						break;
					case 211: //Ó
						c = (char)0xD3;
						break;
					case 243: //Ó
						c = (char)0xD3;
						break;
					case 344: //Ř
						c = (char)0xFC;
						break;
					case 345: //ř
						c = (char)0xFD;
						break;
					case 352: //Š
						c = (char)0x2020;
						break;
					case 353: //š
						c = (char)0xE7;
						break;
					case 356: //Ť
						c = (char)0x203A;
						break;
					case 357: //ť
						c = (char)0x164;
						break;
					case 218: //Ú
						c = (char)0xE9;
						break;
					case 250: //ú
						c = (char)0xAB;
						break;
					case 366: //Ů
						c = (char)0x162;
						break;
					case 367: //ů
						c = (char)0x2026;
						break;
					case 221: //Ý
						c = (char)0xED;
						break;
					case 253: //ý
						c = (char)0x11B;
						break;
					case 381: //Ž
						c = (char)0xA6;
						break;
					case 382: //ž
						c = (char)0xA7;
						break;

					// znaki krzaki
					case 167: //§
						c = (char)337;
						break;
					case 272: //Đ	
						c = (char)'ă';
						break;
				}

				returnValue[i] = c;
			}

			return new string(returnValue);
		}

        /// <summary>
        /// Zwraca indeksy par w podanym stringu.
        /// Czyli dla wywołania GetPairs("xx(xxx(xxx))", '(', ')') zwróci [2, 11] [6, 10]
        /// </summary>
        /// <param name="text"></param>
        /// <param name="character"></param>
        /// <returns></returns>
	    public static List<KeyValuePair<int?, int?>> GetPairs(string text, char openingChar, char closingChar)
	    {
	        var result = new List<KeyValuePair<int?, int?>>();
            var stack = new Stack<int>();

	        for (int i = 0; i < text.Length; i++)
	        {
	            if (text[i] == openingChar)
                    stack.Push(i);

	            if (text[i] == closingChar)
	            {
	                if (stack.Count > 0)
                        result.Add(new KeyValuePair<int?, int?>(stack.Pop(), i));
                    else
                        result.Add(new KeyValuePair<int?, int?>(null, i));
	            }
	        }

            var rest = stack.Count;
	        while (rest > 0)
	        {
	            result.Add(new KeyValuePair<int?, int?>(stack.Pop(), null));
                rest--;
	        }

            return result;
	    }

	    public static List<int> IndexesOf(this string text, string value, StringComparison stringComparison)
	    {
	        var indexesOf = new List<int>();
	        var iof = 0;
	        while (true)
	        {
	            iof = text.IndexOf(value, iof + 1, stringComparison);
                if(iof == -1) return indexesOf;
                indexesOf.Add(iof);
	        }
	    }

	    public static List<int> IndexesOf(this string text, string value)
	    {
	        return IndexesOf(text, value, StringComparison.CurrentCulture);
	    }

	    public static List<KeyValuePair<int, int>> IndexesOfKeyValuePairs(this string text, string value)
	    {
            var iofs = text.IndexesOf(value, StringComparison.InvariantCultureIgnoreCase);
            var iofskvps = new List<KeyValuePair<int, int>>();
            for (var i = 0; i < iofs.Count - 1; i = i + 2)
            {
                iofskvps.Add(new KeyValuePair<int, int>(iofs[i], iofs[i + 1]));
            }
            return iofskvps;
        }

	    public static string ReplaceFromTo(this string text, string value, int indexFrom, int indexTo)
	    {
            var sb = new StringBuilder(text);
	        sb.Remove(indexFrom, indexTo);  
	        sb.Insert(indexFrom, value);

	        return sb.ToString();
	    }

    }
}
