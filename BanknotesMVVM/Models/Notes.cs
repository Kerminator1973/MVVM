using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanknotesMVVM.Models
{
    class Notes
    {
        public Notes(int _denomination, int _count, int _currencyCode, String _name)
        {
            Currency = new Currency(_currencyCode, _name);
            Denomination = _denomination;
            Count = _count;
        }

        // Валюта
        public Currency Currency { get; set; }

        // Номинал купюры
        public int Denomination { get; }

        // Количество купюр этого номинала
        public int Count { get; }

        // Вычисляемое свойство для получения общей суммы 
        // принятых наличных по конкретному номиналу
        public int TotalAmount
        {
            get {
                return Denomination * Count;
            }
        }
    }
}
