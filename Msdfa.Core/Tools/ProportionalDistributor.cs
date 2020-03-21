using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Tools
{
    public class ProportionalDistributor
    {
        private readonly decimal _proportionBaseTotal;
        private decimal _leftToDistribute;
        private decimal _totalToDistribute;
        private decimal _usedValue;

        public ProportionalDistributor(decimal proportionBase, decimal totalToDistribute)
        {
            _proportionBaseTotal = proportionBase;
            _leftToDistribute = totalToDistribute;
            _totalToDistribute = totalToDistribute;
        }

        public decimal GetProportionalTotalValue(decimal value, int round)
        {
            if (_proportionBaseTotal == 0) return 0;
            var temp = _leftToDistribute;
            var result = Math.Round((value)/ _proportionBaseTotal * _totalToDistribute, round);
            _usedValue += value;
            _leftToDistribute -= result;

            _leftToDistribute = Math.Max(_leftToDistribute, 0);
            return temp - _leftToDistribute;
        }
    }
}
