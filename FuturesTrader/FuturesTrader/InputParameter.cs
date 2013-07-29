using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesTrader
{
    public class InputParameter
    {
        public enum Index
        {
            NONE = -1,
            AVGTICKCOUNT,
            OFFSET,
            TRADESIZE,
            SIGNALINTERVAL,
            POSLIMIT,
            STOPLOSS,
            MAX
        };

        public static string[] names =
		{

			"Average Tick Count",
			"Offset",
			"Trade Size",
			"Signal Interval",
			"Position Limit",
			"Stop Loss"
		};
        
        public Object value;
        public InputParameter.Index which;


    }
}
