using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UITest
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
#if true
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);



            Form f = new Form1();
            if (!f.IsDisposed)
            {
                Application.Run(f);
            }
        }
#endif
    }
}
