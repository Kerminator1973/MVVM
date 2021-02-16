using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanknotesMVVM.Models
{
    class Currency
    {
        public Currency(int _currencyCode, String _name)
        {
            CurrencyCode = _currencyCode;
            CurrencyName = _name;
        }

        public int CurrencyCode { get; }

        public String CurrencyName { get; }
    }
}
