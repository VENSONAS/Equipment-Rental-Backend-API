using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration
{
    public interface ICurrencyService
    {
        public Task<decimal> GetExchangeToAsync(string toCurrency);
        public Task<decimal> GetExchangeFromAsync(string fromCurrency);
    }
}
