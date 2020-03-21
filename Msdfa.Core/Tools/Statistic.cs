using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools
{
    public class Statistic
    {
        public static double GetStandardDeviation(List<double> items)
        {
            var average = items.Average();
            var sumOfSquaresOfDifferences = items.Select(val => (val - average) * (val - average)).Sum();
            var res = Math.Sqrt(sumOfSquaresOfDifferences / (items.Count));
            return res;
        }

        public static double GetOdchylenieMax(List<int> items)
        {
            var average = Math.Round(items.Average(), 2);
            var maxOdchylenie = items.Select(x => (x >= average ? x - average : average - x)).Max();
            var maxOdchylenieWsp = average == 0 ? 
                maxOdchylenie
                : maxOdchylenie / average;
            var rounded = Math.Round(maxOdchylenieWsp, 4);
            return rounded;
        }

        public static double GetOdchylenieMax_BazaBezPikow(List<int> items)
        {
            var average = GetSredniaBezPikow(items);
            var maxOdchylenie = items.Select(x => (x >= average ? x - average : average - x)).Max();
            var maxOdchylenieWsp = average == 0 ? maxOdchylenie : maxOdchylenie / average;
            var rounded = Math.Round(maxOdchylenieWsp, 4);
            return rounded;
        }

        public static double GetSredniaBezPikow(List<int> items)
        {
            double suma = items.Sum();
            if (items.Count == 0) return 0;
            if (items.Count < 3) return Math.Round(suma / items.Count, 4);

            var maxItem = items.Max();
            var minItem = items.Min();
            return Math.Round((suma - maxItem - minItem) / (items.Count - 2), 4);
        }

        public static string GetAlert(List<int> items)
        {
            var srednia = Math.Round(items.Average(), 2);
            var sredniaBP = GetSredniaBezPikow(items);
            var odchylenieMaxWsp = GetOdchylenieMax_BazaBezPikow(items);
            
            // Jeżeli pik nie przekracza 300% i 
            if (sredniaBP < 10)
            {
                if (odchylenieMaxWsp < 3) return null;
                if (sredniaBP * odchylenieMaxWsp < 10) return null;
                return $"Pik: {(odchylenieMaxWsp) * 100}%, Średnia: {srednia}, Średnia bez pików: {sredniaBP}";
            }

            if (odchylenieMaxWsp < 1) return null;
            return $"Pik: {(odchylenieMaxWsp) * 100}%, Średnia: {srednia}, Średnia bez pików: {sredniaBP}";
        }
    }
}
