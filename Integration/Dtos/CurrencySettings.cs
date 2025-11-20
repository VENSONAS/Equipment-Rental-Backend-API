using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Dtos
{
    public class CurrencySettings
    {
        public string DefaultCurrency { get; set; }
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
    }
}
