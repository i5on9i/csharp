using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;


namespace FuturesTrader
{
    /**
    *  1. Receive tick data from the tick reader thread
    *  2. Check trade condition
    *  3. Execute trades (buy or sell [Trade Size: 1 lot])
    *      on each trading signals (Buy Signal, Sell Signal) according to the condition
    */

    /// <summary>
    /// 
    /// </summary>
    class StrategyExecutor
    {
        // Thread-safe, only for read operation
//        public DataTable orderDataTable { get; private set; }



        private readonly ManualResetEvent mSignal = new ManualResetEvent(false);


        

        private List<Order> orderList;

        private MainForm mainForm = null;

        private int averageTickCount;
        private decimal offset;
        private int tradeSize;
        private TimeSpan signalInterval;
        private int positionLimit;
        private decimal stopLoss;
        
        private int currentPosition;

		// averageTickCount-relative variables
		private decimal sumOfMids;
		private decimal averageMidPrice;


        private int [] sign = {1, -1};


        /*
            because of the memory, mid Queue size is restricted. 
            This value restricts the value of Average Tick Count(ATC) 
        */
        public static int maxMidQueueSize = 5000;   

        

        public StrategyExecutor(MainForm f, object[] inputs)
        {
            this.mainForm = f;  /* it's possible to access Form after it is disposed of
                                    However, that is the only case of click the
                                    'close' button.
                                 */
            
            // Try to maintain a duplicated data to speed up by around 7%
            orderList = new List<Order>();


            // inputs initialization
            averageTickCount = (int)inputs[(int)InputParameter.Index.AVGTICKCOUNT];
            offset = (decimal)inputs[(int)InputParameter.Index.OFFSET];
            tradeSize = (int)inputs[(int)InputParameter.Index.TRADESIZE];
            signalInterval = new TimeSpan(0, 0, (int)inputs[(int)InputParameter.Index.SIGNALINTERVAL]);
            positionLimit = (int)inputs[(int)InputParameter.Index.POSLIMIT];
            stopLoss  = (-1) * (long)inputs[(int)InputParameter.Index.STOPLOSS]
                        / Order.PRICEPERPOINT;


            // relevant variable
            currentPosition = 0;

			
            sumOfMids = -1;
			averageMidPrice = -1;

        }

        public void run()
        {
            Debug.WriteLine("{0} Thread = {1}", "workThread", Thread.CurrentThread.ManagedThreadId);

            const int STATE_STOPTRADE = 0;
            const int STATE_RUNTRADE = 1;
            int state = STATE_RUNTRADE;


            ////////////////////////
            // init values for this thread
            //
            
            int tickStart = 0;          // ticks from the start of app.
            int tickInputChange = 0;   // ticks after input is changed

            Queue<decimal> mids = new Queue<decimal>();

            

            int availableTradeSize = 0;

            TickData currentTick;
            decimal currentMidPrice;
            decimal currentOffset;

            decimal currentPrice = 0;
            Order.Signal currentSignal = Order.Signal.NONE;

            //
            //  end of init
            /////////////////////////




            // for test
            int loopcount = 0;
            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            

            ////////////////////////////////////////////////////////////////
            // Mid Price = (Bid Price + Ask Price) / 2
            // Average Mid Price = Average Price of (last [Average Tick Count: 500] ticks’ Mid Prices)
            //                   = (sum of Mid-Prices during ATC / AverageTickCount)    # is it right how to get the average?
            //

            while (state == STATE_RUNTRADE)
            {
                
                
                /**
                 *  Set() and WaitOne()
                 *  Consider the case that the {@ref:TickReader} is blocked.
                 *  or {@ref:MainForm.mQueue} is empty.
                 *  Therefore, the MainForm.mSignal.WaitOne() is needed.
                 *
                 *  Need of loop for dequeue
                 *  It is possible for the numbers of calling Set()
                 *  and WaitOne() not to be same.
                 *  {@ref : http://msdn.microsoft.com/en-us/library/system.threading.autoresetevent.aspx}
                 *  Thus the loop for the dequeuing is needed.
                 */
                MainForm.mSignal.WaitOne();  // thread waits for the notice from others
                Debug.WriteLine("waitone");

                do  // loop for dequeueing all the ticks
                {
                    mSignal.WaitOne();  // This makes the {@StrategyExecutor} pause immediately
                    // this does not work properly.
                    

					processChangedInput();
                
                    currentTick = null; // init

                    // fetch the item,
                    // but only lock shortly
                    lock (MainForm.mLock)
                    {
                        if (MainForm.mQueue.Count > 0)
                            currentTick = MainForm.mQueue.Dequeue();
                    }

                    if (currentTick != null)
                    {

                        if (loopcount == 0)
                        {
                            stopwatch.Start();  // begin stopwatch
                        }


                        // do stuff
                        Debug.WriteLine("do stuff");
                        Debug.WriteLine(currentTick.time.Millisecond);

                        //
                        //
                        // 
                        //
                        //
                        
                        // init
                        tickStart += 1;          // ticks from the start of app.
                        tickInputChange += 1;
                        currentMidPrice = currentTick.midPrice;

                        ////////////
                        // Initialize the sumOfMids
                        //
                        if ((sumOfMids.Equals(-1)))
                        {
                            // Set or reset the sumOfMids
                            sumOfMids = getSumDuring(mids, averageTickCount);
                        }

                        ////////////
                        // Add current value
                        //
                        if (!sumOfMids.Equals(-1))
                        {// if the getSumDuring() is executed successfully.

                            // Add the current pick's mid-price
                            // to the sum of last mid-prices
                            int firstIndex = mids.Count - averageTickCount;
                            sumOfMids -= mids.ElementAt(firstIndex);
                            sumOfMids += currentMidPrice;

                            ////////////
                            // Average Mid Price Calculation
                            //
                            averageMidPrice = sumOfMids / averageTickCount;
                            Debug.WriteLine("tickStart = {0}", tickStart);
                            Debug.WriteLine("sumOfMids = {0}", sumOfMids);
                            Debug.WriteLine("Average Mid Price = {0}", averageMidPrice);
                        }

                        ////////////
                        // Enqueue the current Market Mid Price
                        //
                        mids.Enqueue(currentTick.midPrice);
                        if (mids.Count > maxMidQueueSize)
                        {
                            // to retain static size of queue
                            mids.Dequeue();
                        }



                        ////////////////////////////////////////////
                        // Profit and Loss
                        //
                        ////////////////////////////////////////////

                        // Update the Total Profit and Loss
                        calculateTotalPnL(currentMidPrice);

						

                        if (meetStopLoss())    // total loss is greater
                        {

							// stopTrading()
                            state = STATE_STOPTRADE;    // stop calculating
							
                            // Trade out, offset positions

                            //
                            // Clean All Postions
                            if(currentPosition != 0){
                                if(currentPosition > 0)
                                {
                                    // SELL signal is needed
                                    availableTradeSize = Math.Abs(currentPosition);
                                    currentSignal = Order.Signal.SELL;
                                    currentPrice = currentTick.bidPrice;
                                }else{
                                    // BUY signal is needed
                                    availableTradeSize = Math.Abs(currentPosition);
                                    currentSignal = Order.Signal.BUY;
                                    currentPrice = currentTick.askPrice;

                                }
                                
                                // makeAnOrder()
                                Order o = new Order()
                                {
                                    time = currentTick.time,
                                    side = currentSignal,
                                    size = availableTradeSize,
                                    price = currentPrice,
                                    midPrice = currentTick.midPrice,
                                    averageMidPrice = averageMidPrice
                                    
                                };

                                int sign = (o.side == Order.Signal.BUY ? 1 : -1);

                                o.pnl = (currentMidPrice - currentPrice) * sign * availableTradeSize;
                                o.accPnl = orderList[orderList.Count-1].accPnl + o.pnl;  // calculate the Total PnL for speed


                                // add to the orderList
                                addToOrderList(o);


                            }
                                

                            // Update UI to Stop Loss and
                            // Stop {@ref:TickReader}
                            try
                            {
                                mainForm.Invoke((MethodInvoker)delegate
                                {
                                    mainForm.tradeOut("Stop Loss");
                                });

                            }
                            catch (Exception e)
                            {
                                // Possible Exceptions as window is closed
                                // - ObjectDisposedException
                                // - InvalidOperationException
                                Console.WriteLine(e.Message);
                            }
                            
                            break;
                        }

						
                        ////////////////////////////////////////////
                        // Signals
                        //
                        // Buy Signal
                        //  Mid Price > Average Mid Price + Offset
                        // Sell Signal
                        //  Mid Price < Average Mid Price - Offset
                        ////////////////////////////////////////////
                        if (averageMidPrice != -1)
                        {
                            currentOffset = currentMidPrice - averageMidPrice;  // init

                            currentSignal = Order.Signal.NONE;
                            availableTradeSize = 0;
                            if (currentOffset > offset)
                            {
                                currentSignal = Order.Signal.BUY;
                                availableTradeSize = Math.Min(tradeSize, currentTick.askQuantity);
                                currentPrice = currentTick.askPrice;
                            }
                            else if (currentOffset < -offset)
                            {
                                currentSignal = Order.Signal.SELL;
                                availableTradeSize = Math.Min(tradeSize, currentTick.bidQuantity);
                                currentPrice = currentTick.bidPrice;

                            }


                            if (currentSignal != Order.Signal.NONE)
                            {// BUY or SELL signal occurs
                                if (isOrderSignal(currentSignal, currentTick.time, availableTradeSize))
                                {
                                    // makeAnOrder()
                                    Order order = new Order()
                                    {
                                        time = currentTick.time,
                                        side = currentSignal,
                                        size = availableTradeSize,
                                        price = currentPrice,
                                        midPrice = currentTick.midPrice,
                                        averageMidPrice = averageMidPrice
                                    };


                                    // add to the orderList
                                    addToOrderList(order);


                                    Debug.WriteLine("signal");

                                }

                            }
                        }


                        // for test
                        if (loopcount == 100)
                        {
                            // Stop timing
                            stopwatch.Stop();

                            // Write result
                            Debug.WriteLine("Time elapsed: {0}",
                                stopwatch.Elapsed);
                        }
						loopcount++;

                    }
                } while (currentTick != null); // loop until there are items to collect


            }   // end of while


			// Make the stop button disabled
            try
            {
                mainForm.Invoke((MethodInvoker)delegate
                {
                    mainForm.enableStopButton(false);
                });

            }
            catch (Exception e)
            {
                // Possible Exceptions as window is closed
                // - ObjectDisposedException
                // - InvalidOperationException
                Console.WriteLine(e.Message);
            }

            Debug.WriteLine("workerthread is finished");

        }



		/// <summary>
        ///   This function processes the change from the input UI which is 
        ///   delivered through the {@ref:MainForm.mQueueInput}
        /// </summary>
		private void processChangedInput()
		{
			////////////////////////////////////////////
            // Input setting
            //
            InputParameter changedInput;
            do  // loop for the inputs changed
            {
                changedInput = null;

                lock (MainForm.mLockInput)
                {
                    while (MainForm.mQueueInput.Count > 0)
                    {
                        changedInput = MainForm.mQueueInput.Dequeue();
                    }
                }

                if (changedInput != null)
                {
                    switch (changedInput.which)
                    {
                        case InputParameter.Index.AVGTICKCOUNT:
                            averageTickCount = (int)changedInput.value;
                            averageMidPrice = -1;
                            sumOfMids = -1;
                            break;
                        case InputParameter.Index.OFFSET:
                            offset = (decimal)changedInput.value;
                            break;
                        case InputParameter.Index.TRADESIZE:
                            tradeSize = (int)changedInput.value;
                            break;
                        case InputParameter.Index.SIGNALINTERVAL:
                            signalInterval = new TimeSpan(0, 0, (int)changedInput.value);
                            break;
                        case InputParameter.Index.POSLIMIT:
                            positionLimit = (int)changedInput.value;

                            if(orderList.Count >= 1)
                            {
                                try
                                {
                                    mainForm.Invoke((MethodInvoker)delegate
                                    {
                                        mainForm.updateNewPosition(positionLimit);
                                    });
                                }
                                catch (Exception e)
                                {
                                    // Possible Exceptions as window is closed
                                    // - ObjectDisposedException
                                    // - InvalidOperationException
                                    Console.WriteLine(e.Message);
                                }
                            }
                            
                            break;
                        case InputParameter.Index.STOPLOSS:
                            stopLoss = (decimal)((-1) * (long)changedInput.value / Order.PRICEPERPOINT);
                            break;
                    }
                }
            } while (changedInput != null);
			
		}

        /// <summary>
        ///   add the order to {@ref:orderList} and 
        ///   invoke the {@ref:MainForm.addOrder}
        /// </summary>
        /// <param name="o"></param>
        private void addToOrderList(Order o)
        {
            orderList.Add(o);

            currentPosition += o.size * (o.side == Order.Signal.BUY ? 1 : -1);


            try
            {
                mainForm.Invoke((MethodInvoker)delegate
                {
                    mainForm.addOrder(o, currentPosition);
                });

            }
            catch (Exception e)
            {
                // Possible Exceptions as window is closed
                // - ObjectDisposedException
                // - InvalidOperationException
                Console.WriteLine(e.Message);
            }

            Debug.WriteLine("addToOrderList");

            
            return;

        }



        /// <summary>
        ///     Calculate all the total Profit and Loss of every rows
        /// </summary>
        /// <param name="currentMidPrice"></param>
        private void calculateTotalPnL(decimal currentMidPrice)
        {

            if (orderList.Count <= 0)
            {
                // do nothing
                return;
            }

            int [] sign = {1, -1};

            Order pre;
            Order o = orderList[0];

			// TODO : test of o.size
            // first index
            o.pnl = (currentMidPrice - o.price) * sign[(int)o.side] * o.size;
            o.accPnl = o.pnl;

            // the rest index
            for (int i = 1; i < orderList.Count; i++)
            {
                o = orderList[i];
                pre = orderList[i - 1];

                o.pnl = (currentMidPrice - o.price) * sign[(int)o.side] * o.size;
                o.accPnl = pre.accPnl + o.pnl;  // calculate the Total PnL for speed

            }

           
            // Update the {@ref:StrategyExecutor.orderTable} value
            try
            {
                mainForm.Invoke((MethodInvoker)delegate
                {
                    mainForm.updateTotalPnl(orderList);
                });
                
            }
            catch (Exception e)
            {
                // Possible Exceptions as window is closed
                // - ObjectDisposedException
                // - InvalidOperationException
                Console.WriteLine(e.Message);
            }
            return;
        }

        
        /// <summary>
        ///     Check whether or not the signal can be executed.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="signalTime"></param>
        /// <returns>
        ///     true : first signal
        ///             or previous-signal != current-signal
        ///             or signal-interval has been passed
        /// </returns>
        private bool isOrderSignal(Order.Signal s, DateTime signalTime, int tradeSize)
        {

            // namh
            int lookAheadPosition = currentPosition
                            + (tradeSize * (s == Order.Signal.BUY ? 1 : -1));
            if(Math.Abs(lookAheadPosition) <= positionLimit)  // TODO : < or <= ??
            {
                int lastIndex = orderList.Count - 1;
                if (lastIndex < 0                       // first signal
                    || orderList[lastIndex].side != s    // different from previous signal
                    || (signalTime - orderList[lastIndex].time >= signalInterval))    // once a signal-interval(60s)
                {
                    return true;
                }
            }
            else
            {// case of position limit

                // Now, this application do not have to deal with rejected trade
                
            }
                

            return false;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <returns></returns>
        private bool meetStopLoss()
        {
            if (orderList.Count > 0)
                return (orderList[orderList.Count - 1].accPnl <= stopLoss);

            return false;
        }



        /// <summary>
        ///     This function calculates and returns the sum of mid prices
        ///     during the period from the lastest tick.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="period"></param>
        /// <returns>
        ///     '-1' : period > 0 && preIndexOfStart < -1
        ///     sum : otherwise
        /// </returns>
        private decimal getSumDuring(Queue<decimal> data, int period)
        {
            /**
             //# TODO 
             //   # overflow would occur, you must deal with this
             //   # think about over 500
             //   # (a+b)/2 = a/2 + b/2
             **/

            decimal sum = 0;

            int last = data.Count - 1;               // last element
            int preIndexOfStart = last - period;    // before-index of the start index
            if (period > 0
                && preIndexOfStart < -1)    // the amount of mid values are not enough to be cacluated
            {
                return -1;
            }

            for (int i = last; i > preIndexOfStart; i--)
            {
                sum += data.ElementAt(i);
            }
            return sum;
        }
        




        /// <summary>
        /// This function resume the main loop of this thread
        /// , which is for fetching the data from the {@ref:MainForm.mQueue}
        /// </summary>
        public void resume()
        {
            mSignal.Set();
        }


        /// <summary>
        /// This function pauses the main loop of this thread
        /// , which is for fetching the data from the {@ref:MainForm.mQueue}
        /// </summary>
        public void pause()
        {
            mSignal.Reset();
        }


    }   // end of class
}
