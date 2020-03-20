using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownDetector.Options
{
    public class PredictionAPI
    {
        public string Url { get; set; }
        public string Key { get; set; }
        public float Probability { get; set; }
    }
}
