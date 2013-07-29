using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Diagnostics;

namespace FuturesTrader
{
    /// <summary>
    /// 
    /// </summary>
    public class Order{

        public enum Signal{
            NONE = -1,
            BUY,
            SELL
        };

        public const decimal PRICEPERPOINT = 500000;
        /**
        Trade Time  "HH:mm:ss.fff"
        Trade Side  Buy, Sell
        Trade Size  “N0”　
        Trade Price "0.00"
        Mid Price   “0.000”
        Average Mid Price   “0.000”
        Position    “N0”
        Total PnL   "N0"
        Trade Reason    Buy Signal, Sell Signal, Stop Loss
         * */
        public DateTime time { get; set; }
        public Signal side { get; set; }
        public int size { get; set; }                // position used
        public decimal price { get; set; }           // unit: Point
        public decimal midPrice { get; set; }        // unit: Point
        public decimal averageMidPrice { get; set; } // unit: Point

        public int remainedPosition;    // is this needed?? maybe not


        public decimal pnl { get; set; }             // Profit & Loss, (unit: Point)
        public decimal accPnl { get; set; }          // accumulated PnL, (unit: Point)


    }

    

    
}
