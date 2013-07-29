using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace FuturesTrader
{
    public class TickData
    {
        public const int INDEX_SYMBOL = 0;
        public const int INDEX_TIME = 1;
        public const int INDEX_BIDPRICE = 2;
        public const int INDEX_ASKPRICE = 3;
        public const int INDEX_BIDQUANTITY = 4;
        public const int INDEX_ASKQUANTITY = 5;
        
        public string symbol;   // is this needed??
        public DateTime time;
        public decimal bidPrice;    // decimal is too big or not ?? it's bad for speed ?
        public decimal askPrice;    /* TODO : check later which one will be used, int, float, double, decimal
                                        float is not accurate. */
        public int bidQuantity;
        public int askQuantity;
        public decimal midPrice;

        public TickData(string[] values)
        {
            ////////////////////////
            // Parse & Set
            //
            string format = "HH:mm:ss.fff";  // 09:00:00.490
            if (!DateTime.TryParseExact(
                    values[TickData.INDEX_TIME],    // input
                    format,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out time))
            {
                // error handling
                Console.WriteLine("Not a date");
            }

            symbol = values[INDEX_SYMBOL];
            bidPrice = decimal.Parse(values[INDEX_BIDPRICE]);
            askPrice = decimal.Parse(values[INDEX_ASKPRICE]);
            bidQuantity = int.Parse(values[INDEX_BIDQUANTITY]);
            askQuantity = int.Parse(values[INDEX_ASKQUANTITY]);
            midPrice = (bidPrice + askPrice) / 2;
            

        }

    }


}
