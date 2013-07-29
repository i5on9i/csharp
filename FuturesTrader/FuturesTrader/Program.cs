using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FuturesTrader
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form f = new MainForm();
            if(!f.IsDisposed)
                Application.Run(f);
        }
    }
}
