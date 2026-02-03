using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Services
{
    class CurrencyService
    {
        private Dictionary<(string, string), decimal> rates = new()
        {
            { ("USD", "EUR"), 0.85m },
            { ("EUR", "USD"), 1.18m }
        };

        public decimal? GetRate(string from, string to)
        {
            if (rates.TryGetValue((from, to), out decimal rate))
                return rate;

            return null;
        }
    }
}
