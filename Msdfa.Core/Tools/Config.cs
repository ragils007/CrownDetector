using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Tools
{
    public class Config
    {
        private readonly string FileName;
        public bool LoadAllCategories { get; set; }
        public char SeparatorChar { get; set; }
        public char CommentChar { get; set; }
        public Dictionary<string, object> Data = new Dictionary<string, object>();

        public Config(string fileName = null, char separatorChar = '=', char commentChar = '#', bool loadAllCategories = true)
        {
            this.SeparatorChar = separatorChar;
            this.CommentChar = commentChar;
            this.LoadAllCategories = loadAllCategories;

            if (fileName != null)
            {
                this.FileName = fileName;
                this.Load(fileName);
            }
        }

        public void Load(string fileName, params string[] categories)
        {
            var f = new System.IO.StreamReader(fileName);

            this.Data.Clear();

            string line = "";
            string currentCategory = null;
            while ((line = f.ReadLine()) != null)
            {
                if (line.Length == 0) continue;
                if (line[0] == '[')
                {
                    currentCategory = line.Replace("[", "").Replace("]", "");
                    continue;
                }

                var separatorPos = line.IndexOf(this.SeparatorChar);
                if (line[0] == '#' || separatorPos <= 0 || separatorPos == line.Length) continue;

                if (currentCategory == null || categories.Contains(currentCategory) || (categories.Length == 0 && this.LoadAllCategories))
                {
                    var key = line.Substring(0, separatorPos).Trim();
                    var value = line.Substring(separatorPos + 1).Trim();
                    this.SetData(key, value);
                }
            }
            f.Close();

            if (!this.Data.Any() && categories.Any()) throw new Exception($"Nie odnalazłem żadnych ustawień dla kategorii: [{string.Join(",", categories)}]");
        }

        public void SetData(string key, object val)
        {
            if (this.Data.ContainsKey(key)) this.Data[key] = val;
            else this.Data.Add(key, val);
        }

        public string Get(string key)
        {
            return this.GetString(key);
        }

        public decimal GetDecimal(string key)
        {
            this.CheckKey(key);
            return decimal.Parse(this.Data[key].ToString());
        }

        public double GetDouble(string key)
        {
            this.CheckKey(key);
            return double.Parse(this.Data[key].ToString());
        }

        public int GetInt(string key)
        {
            this.CheckKey(key);
            return int.Parse(this.Data[key].ToString());
        }

        public string GetString(string key)
        {
            this.CheckKey(key);
            return this.Data[key].ToString();
        }

        private void CheckKey(string key)
        {
            if (this.Data.ContainsKey(key)) return;

            if (this.FileName == null) throw new Exception($"Nie odlanazłem oczekiwanego pola: {key}");
            throw new Exception($"[{this.FileName}] nie odlanazłem oczekiwanego pola: '{key}'");
        }

        // Nullables
        public decimal? GetNullableDecimal(string key)
        {
            if (this.Data.ContainsKey(key) == false) return null;
            return decimal.Parse(this.Data[key].ToString());
        }

        public double? GetNullableDouble(string key)
        {
            if (this.Data.ContainsKey(key) == false) return null;
            return double.Parse(this.Data[key].ToString());
        }

        public int? GetNullableInt(string key)
        {
            if (this.Data.ContainsKey(key) == false) return null;
            return int.Parse(this.Data[key].ToString());
        }

        public string GetNullableString(string key)
        {
            if (this.Data.ContainsKey(key) == false) return null;
            return this.Data[key].ToString();
        }

        public List<string> GetListString(string key, char splitChar = ',')
        {
            var str = this.GetNullableString(key);
            if (str == null || string.IsNullOrEmpty(str.Trim())) return null;
            return str.Split(splitChar).ToList();
        }
    }
}