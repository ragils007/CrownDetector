using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools
{
    public class Validator
    {
        public static bool ValidateMail(string mail, bool allowEmpty = true)
        {
            try
            {
                if (string.IsNullOrEmpty(mail)) return allowEmpty;

                if (mail.Contains(" ")) return false;

                var m = new MailAddress(mail);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool ValidateWWW(string www)
        {
            if (string.IsNullOrEmpty(www)) return true;

            Uri uriResult;
            if (www.StartsWith("http") == false) www = $"http://{www}";

            return Uri.TryCreate(www, UriKind.Absolute, out uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static bool ValidateNIP(string nip, string country = "PL")
        {
            if (country != "PL") return (nip.Length > 0);

            if (nip.ToUpper().StartsWith("PL")) nip = nip.Substring(2);
            nip = string.Join("", nip.Where(x => !new[] { ' ', '.', '-' }.Contains(x)));

            if (nip.Length != 10) return false;

            var wagi = new[] { 6, 5, 7, 2, 3, 4, 5, 6, 7, 0 };
            var suma = nip.Zip(wagi, (cyfra, waga) => (cyfra - '0') * waga).Sum();

            return (suma % 11) == (nip[9] - '0');
        }

        public static bool ValidateRegon(string RegonValidate, string kraj = "PL")
        {
            if (string.IsNullOrWhiteSpace(RegonValidate) || kraj != "PL") return true;

            byte[] weights;
            ulong regon = ulong.MinValue;
            byte[] digits;

            if (ulong.TryParse(RegonValidate, out regon).Equals(false)) return false;
            switch (RegonValidate.Length)
            {
                case 7:
                    weights = new byte[] { 2, 3, 4, 5, 6, 7 };
                    break;
                case 9:
                    weights = new byte[] { 8, 9, 2, 3, 4, 5, 6, 7 };
                    break;
                case 14:
                    weights = new byte[] { 2, 4, 8, 5, 0, 9, 7, 3, 6, 1, 2, 4, 8 };
                    break;

                default:
                    return false;
            }
            string sRegon = RegonValidate;
            digits = new byte[sRegon.Length];
            for (int i = 0; i < sRegon.Length; i++)
            {
                if (byte.TryParse(sRegon[i].ToString(), out digits[i]).Equals(false)) return false;
            }

            int checksum = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                checksum += weights[i] * digits[i];
            }

            return (checksum % 11 % 10).Equals(digits[digits.Length - 1]);
        }

        public static bool ValidateBankAccountIBAN(string bankAccount)
        {
            bankAccount = bankAccount.ToUpper(); //IN ORDER TO COPE WITH THE REGEX BELOW
            if (String.IsNullOrEmpty(bankAccount))
                return false;
            else if (System.Text.RegularExpressions.Regex.IsMatch(bankAccount, "^[A-Z0-9]"))
            {
                bankAccount = bankAccount.Replace(" ", String.Empty);
                string bank =
                    bankAccount.Substring(4, bankAccount.Length - 4) + bankAccount.Substring(0, 4);
                int asciiShift = 55;
                StringBuilder sb = new StringBuilder();
                foreach (char c in bank)
                {
                    int v;
                    if (Char.IsLetter(c)) v = c - asciiShift;
                    else v = int.Parse(c.ToString());
                    sb.Append(v);
                }
                string checkSumString = sb.ToString();
                int checksum = int.Parse(checkSumString.Substring(0, 1));
                for (int i = 1; i < checkSumString.Length; i++)
                {
                    int v = int.Parse(checkSumString.Substring(i, 1));
                    checksum *= 10;
                    checksum += v;
                    checksum %= 97;
                }
                return checksum == 1;
            }
            else return false;
        }

        public static bool ValidatePhoneNumber(string phoneNumber)
        {
            var numeric = string.Join("", phoneNumber.Where(x => !new[] { ' ', '+', '-', '(', ')' }.Contains(x)));
            return ValidateNumericString(numeric);
        }

        public static bool ValidateNumericString(string numeric)
        {
            foreach (var item in numeric)
            {
                if (!char.IsDigit(item)) return false;
            }
            return true;
        }
    }
}