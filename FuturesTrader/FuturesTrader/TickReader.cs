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
    class TickReader
    {
		// Volatile is used as hint to the compiler that this data
    	// member will be accessed by multiple threads.
		private volatile bool shouldStop = false;

		private readonly ManualResetEvent mSignal = new ManualResetEvent(false);

		 
        public TickReader()
        {
        }


        /**
         * DataReader Thread
         * 
         * Parse and Make a {@ref:TickData}
         * 
         *  
         */
        public void run()
        {

            Debug.WriteLine("{0} Thread = {1}", "data reader", Thread.CurrentThread.ManagedThreadId);

            // StreamReader is faster than File.ReadAllLines
            try
            {
                var reader = new StreamReader(File.OpenRead(MainForm.INPUTFILE));
            
            

                if (!reader.EndOfStream)
                    reader.ReadLine();  // skip the first line

                while (!shouldStop && !reader.EndOfStream)
                {
				    mSignal.WaitOne();
				
                    string line = reader.ReadLine();

                    // test code
                    // test blocking
                    //for (int i = 0; i < 1000; i++)
                    //    Debug.WriteLine(i);   

                    string[] values = line.Split(',');

                    TickData td = new TickData(values);

				    //
                    // you need to make sure only
                    // one thread can access the list
                    // at a time
                    lock (MainForm.mLock)
                    {
                        MainForm.mQueue.Enqueue(td);
                    }
                    Debug.WriteLine("enqueued");


                    ////////////////////////
                    // do publishing
                    //
                    // any thread can do Set() to make a ticket,
                    // notify the waiting thread
                    //
                    // Check the comment of WaitOne in {@ref:StrategyExecutor}
                    MainForm.mSignal.Set();  // Set():post <--> WaitOne():obtain
                    Debug.WriteLine("set");

                }

			    // UI update
			    // Stop button to disable
            }

            catch (FileNotFoundException e)
            {
                Console.WriteLine("TickData File does not exist : {0}", e.Message);
            }
			
            return;

        }


		public void requestStop()
		{
			shouldStop = true;
		}


		public void resume()
		{
			mSignal.Set();
		}


		public void pause()
		{
            mSignal.Reset();
		}


	
    }

}
