using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Diagnostics;


namespace FuturesTrader
{
    public delegate void dataReaderDelegate();

    public partial class MainForm : Form
    {

        private const int INPUT_COLUMN_VALUE = 1;

        private Thread mStrategyExecutorThread;
        private Thread mTickReaderThread;

		private TickReader mTickReader;
		private StrategyExecutor mStrategyExecutor;


		
        private DataTable orderDataTable;

		
        public static string INPUTFILE = "TickData.csv";

        ////////////////////////////
        // Shared objects
        
        // TickReader <---> StrategyExecutor        
        public static readonly object mLock = new object();
        public static readonly Queue<TickData> mQueue = new Queue<TickData>();
        public static readonly AutoResetEvent mSignal = new AutoResetEvent(false);
        

        // gvInput <---> StrategyExecutor
        public static readonly object mLockInput = new object();
        public static readonly Queue<InputParameter> mQueueInput = new Queue<InputParameter>();


        



        public MainForm()
        {

            
            InitializeComponent();


            Debug.WriteLine("{0} Thread = {1}", "main", Thread.CurrentThread.ManagedThreadId);


            //
            // Init()
            //
            if (!File.Exists(INPUTFILE))
            {
                string message = "Data file does not exist, Do you want to quit the application?";
                string caption = "Data file does not exist at the current folder";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(this, message, caption, buttons);

                if (result == DialogResult.Yes)
                {

                    // Closes the parent form.

                    this.Close();
                    
                }
            }
            


            
            ////////////////////////////
            // Make UI

            gvResultBoard.DoubleBuffered(true); // to get rid of flickering
            gvResultBoard.AutoResizeColumns();
            orderDataTable = createDataTable();
			setBoard(orderDataTable);	// bind to {@ref:gvResultBoard}


            // Make input data, initialize gvInput
            object[] inputValues = {500, 0.05m, 1, 60, 5, 1000000L};
            string[] inputUnits = {"", "Point", "Lot", "Second", "Lot", "Won"};

            initializeInputParameters(inputValues, inputUnits);
            
            

            ////////////////////////////////////////////
            // Run DataReader Thread
            //
            mTickReader = new TickReader();
            mTickReaderThread = new Thread(new ThreadStart(mTickReader.run));
            mTickReaderThread.Name = "TickReader";
            mTickReaderThread.IsBackground = true; /* A thread's foreground/background status
                                              * has no relation to its priority or
                                              * allocation of execution time. */

            mTickReaderThread.Start();
            


            ////////////////////////////////////////////
            // Run StrategyExecutor Thread
            //
            mStrategyExecutor = new StrategyExecutor(this, inputValues);
            mStrategyExecutorThread = new Thread(new ThreadStart(mStrategyExecutor.run));
            mStrategyExecutorThread.Name = "Executor";
            mStrategyExecutorThread.IsBackground = true;

            mStrategyExecutorThread.Start();

            
        }



		public void addOrder(Order o, int currentPosition)
        {
            // add row to datatable
            orderDataTable.Rows.Add(null,
                o.time,
                o.side,
                o.size,
                o.price,  // no cell format for this, so use sting here.
                o.midPrice, // X.fff
                o.averageMidPrice,  // X.fff
                currentPosition,
                null,	// changed position
                o.pnl * Order.PRICEPERPOINT,
                o.accPnl * Order.PRICEPERPOINT,
                (o.side == Order.Signal.BUY ? "BUY SIGNAL" : "SELL SIGNAL")   // trade reason
            );

			// to scroll to latest row
			gvResultBoard.FirstDisplayedScrollingRowIndex
				= orderDataTable.Rows.Count-1;

        }

        public void updateTotalPnl(List<Order> orderList)
        {
            // update the Total PnL value

            // orderList.Count
            // The why the orderList is used instead of the orderDataTable.Rows.Count is
            // the orderDataTable.Rows.Count can be changed depending on UI Property,
            // such as AllowUserToAddRows.
            // Thus using orderDataTable.Rows.Count can causes the Exception
            Order o;
            DataRowCollection rows = orderDataTable.Rows;
            for (int i = 0; i < orderList.Count; i++)
            {
                o = orderList[i];
                rows[i]["PnL"] = o.pnl * Order.PRICEPERPOINT;
                rows[i]["Total PnL"] = o.accPnl * Order.PRICEPERPOINT;
            }

        }

		
		public void updateNewPosition(int newPos)
        {
            DataRowCollection rows = orderDataTable.Rows;
            rows[rows.Count - 1]["New Position Limit"] = newPos;


        }

		public void enableStopButton(bool v)
        {
            btnStop.Enabled = v;
        }

		
		/// <summary>
		///  Write the out reason
		///  & Quit the {@ref:TickReader}
		///
		///  This is the final routine. 
        ///  After this function, values will not be changed.
		/// </summary>
		/// <param name="str"> string to display </param>
		public void tradeOut(String str)
        {
            DataRowCollection rows = orderDataTable.Rows;
			rows[rows.Count-1]["Trade Reason"] = str;

            // change fore-color to BLUE
            DataGridViewRowCollection r = gvResultBoard.Rows;
            r[rows.Count - 1].Cells["Trade Reason"].Style.ForeColor = Color.Blue;

			// Quit the TickReader
			mTickReader.requestStop();
            
        }




		////////////////////////////////////////////////////////////////////////
        //  Private Methods
        //



		/// <summary>
        /// This function creates the DataTable which is used as a DataSource
        /// of the DataGridView of {@ref:MainForm}
        /// </summary>
        /// <returns></returns>
        private DataTable createDataTable()
        {
            DataTable t = new DataTable();

            DataColumn col1 = new DataColumn();
            col1.DataType = typeof(int);
            col1.ColumnName = "No.";
            col1.AutoIncrement = true;
            col1.AutoIncrementSeed = 1; // start number
            col1.Unique = true;
            t.Columns.Add(col1);

            t.Columns.Add("Trade Time", typeof(DateTime));
            t.Columns.Add("Trade Side", typeof(Order.Signal));
            t.Columns.Add("Trade Size", typeof(int));
            t.Columns.Add("Trade Price", typeof(decimal));

            t.Columns.Add("Mid Price", typeof(decimal));
            t.Columns.Add("Average Mid Price", typeof(decimal));
            t.Columns.Add("Position", typeof(int));
			t.Columns.Add("New Position Limit", typeof(int));
            t.Columns.Add("PnL", typeof(long));
            t.Columns.Add("Total PnL", typeof(long));

            t.Columns.Add("Trade Reason", typeof(string));

            return t;
        }


        private void initializeInputParameters(object[] inputValues, string[] inputUnits)
        {


            gvInput.ColumnCount = 3;

            // Set the column header names.
            gvInput.Columns[0].Name = "Parameter";
            gvInput.Columns[1].Name = "Value";
            gvInput.Columns[2].Name = "Unit";


			// Set inputs on DataGridView
            int max = (int)InputParameter.Index.MAX;
            for (int i = 0; i < max; i++)
            {
                gvInput.Rows.Add(InputParameter.names[i],
                                    inputValues[i],
                                    inputUnits[i]);
            }


            // Read only : 1st, 3rd colums
            gvInput.Columns[0].ReadOnly = true;
            gvInput.Columns[2].ReadOnly = true;

            // Set cell values' types
            DataGridViewRowCollection r = gvInput.Rows;

            r[0].Cells[INPUT_COLUMN_VALUE].ValueType = typeof(int);
            r[1].Cells[INPUT_COLUMN_VALUE].ValueType = typeof(decimal);
            r[2].Cells[INPUT_COLUMN_VALUE].ValueType = typeof(int);
            r[3].Cells[INPUT_COLUMN_VALUE].ValueType = typeof(int);
            r[4].Cells[INPUT_COLUMN_VALUE].ValueType = typeof(int);
            r[5].Cells[INPUT_COLUMN_VALUE].ValueType = typeof(long);

            // For Stop Loss
            // set the currency format
            var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            format.CurrencySymbol = ""; // get ride of currency symbol
            r[5].DefaultCellStyle.FormatProvider = format;
            r[5].DefaultCellStyle.Format = "C";


        }

		
        private void setBoard(DataTable table)
        {
            gvResultBoard.DataSource = table;

            //
            // Cell Formatting
            //
            foreach (DataGridViewColumn c in gvResultBoard.Columns)
            {
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                switch (c.Name)
                {
                    case "No.": // TODO : chage to the enum
                        c.Width = 30;   // TODO : change to autosize
                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        break;
                    case "Trade Time":
                        c.Width = 90;
                        c.DefaultCellStyle.Format = "HH:mm:ss.fff";
                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "Trade Side":
                        c.Width = 85;
                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "Trade Size":
                        c.Width = 40;
                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        break;
                    case "Trade Price":
                        c.Width = 65;
                        c.DefaultCellStyle.Format = "N2";
                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        break;
                    case "Mid Price":
                        c.Width = 65;
                        c.DefaultCellStyle.Format = "N3";
                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        break;
                    case "Average Mid Price":
                        c.Width = 65;
                        c.DefaultCellStyle.Format = "N3";
                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        break;
                    case "Position":
					case "New Position Limit":
						c.Width = 55;
                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        break;
                    case "PnL":
                    case "Total PnL":
                        var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                        format.CurrencySymbol = ""; // get ride of currency symbol
                        c.DefaultCellStyle.FormatProvider = format;
                        c.DefaultCellStyle.Format = "C";

                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        cell.Style.ForeColor = Color.Blue;
                        break;


                    default:
                        // "PnL"
                        // "Trade Reason"
                        //  Alignment = MiddleLeft
                        break;
                }
                c.CellTemplate = cell;

            }   // end of switch
                
            return;
            
        }






        private void OnDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Please Check the inputs \nError happened " + e.Context.ToString());

            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Commit error");
            }
            if (e.Context == DataGridViewDataErrorContexts.CurrentCellChange)
            {
                MessageBox.Show("Cell change");
            }
            if (e.Context == DataGridViewDataErrorContexts.Parsing)
            {
                MessageBox.Show("parsing error");
            }
            if (e.Context == DataGridViewDataErrorContexts.LeaveControl)
            {
                MessageBox.Show("leave control error");
            }

            if ((e.Exception) is ConstraintException)
            {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

                e.ThrowException = false;
            }
        }



        ////////////////////////////////////////////////////////////////////////
        //  Event Handlers - gvInput
        //
              

        

        private void Main_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvInput_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            OnDataError(sender, e);
        }

        
        /// <summary>
        ///  Input of {@ref:gvInput} errors are checked here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvInput_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            if (e.ColumnIndex != INPUT_COLUMN_VALUE)
            {
                return;
            }

            // O.K case
            String errorText = String.Empty;
            bool cancelFlag = false;


            long l;
            double d;
            int i;
            
            //
            // Here,
            // e.ColumnIndex is INPUT_COLUMN_VALUE(1)
            //
            if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
            {
                errorText = "Field cannot be empty";
                cancelFlag = true;
            }
            else
            {
                switch ((InputParameter.Index)e.RowIndex)
                {
                    case InputParameter.Index.AVGTICKCOUNT:

                        if (!int.TryParse(e.FormattedValue.ToString(), out i)
                            || (i <= 0))
                        {
                            // not parsable to int
                            errorText = "Average Tick Count must be a Natural number";
                            cancelFlag = true;
                        }
                        else if (i > StrategyExecutor.maxMidQueueSize)
                        {
                            // not parsable to int
                            errorText = "Average Tick Count must be under "
                                            + StrategyExecutor.maxMidQueueSize;
                            cancelFlag = true;
                        }
                        break;
                    case InputParameter.Index.OFFSET:
                        if (!double.TryParse(e.FormattedValue.ToString(), out d)
                            || (d <= 0))
                        {
                            // not parsable to double
                            errorText = "offset must be a positive real number";
                            cancelFlag = true;
                        }
                        break;
                    case InputParameter.Index.TRADESIZE:
					case InputParameter.Index.SIGNALINTERVAL:

                        if (!int.TryParse(e.FormattedValue.ToString(), out i)
                            || (i <= 0))
                        {
                            // not parsable to int
                            errorText = InputParameter.names[e.RowIndex]
                            				+" must be a Natural number";
                            cancelFlag = true;
                        }
                        break;
                        
                    case InputParameter.Index.POSLIMIT:
                        

                        if (!int.TryParse(e.FormattedValue.ToString(), out i)
                            || (i <= 0))
                        {
                            // not parsable to int
                            errorText = "Position Limit must be a Natural number";
                            cancelFlag = true;

                            // Must be more than orders' count

                        }

                        break;
                    case InputParameter.Index.STOPLOSS:
                        Console.WriteLine(e.GetType());
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        NumberStyles styles = NumberStyles.Integer | NumberStyles.AllowThousands;
                            
                        if (!long.TryParse(e.FormattedValue.ToString(), styles, provider, out l)
                            || (l <= 0))
                        {
                            // not parsable to long
                            errorText = "Stop Loss must be a Natural number";
                            cancelFlag = true;
                        }
                            
                        break;
                    default:
                        break;
                }// end of switch
            } // end of if

			// Set
            ((DataGridView)sender).Rows[e.RowIndex].ErrorText = errorText;
            e.Cancel = cancelFlag;

            
        }

        private void gvInput_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            // throw an message of input
            // notify the Strategy Executor thread of that input is changed.
            Console.WriteLine("validated");
        }


		/// <summary>
        /// 
		/// After cell value is changed, the value is enqueued to be used by 
        /// the thread {@ref:StrategyExecutor}
        /// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void gvInput_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            DataGridView dgv_Data = sender as DataGridView;
            
            if (e.ColumnIndex == INPUT_COLUMN_VALUE) // redundancy?
            {
                InputParameter item = new InputParameter();
                item.which = (InputParameter.Index)e.RowIndex;
                item.value = dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                lock (mLockInput)
                {
                    mQueueInput.Enqueue(item);
                }
            }
        }


        ////////////////////////////////////////////////////////////////////////////////
        // Event Handlers - Result Board 
        //

        private void gvResultBoard_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            OnDataError(sender, e);
        }



        ////////////////////////////////////////////////////////////////////////////////
        // Event Handlers - Buttons
        //

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Start
            mTickReader.resume();
            mStrategyExecutor.resume();
            
            // UI update
            btnStart.Enabled = false;
            btnStop.Enabled = true;

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // Stop the rountine
            mTickReader.pause();
            mStrategyExecutor.pause();

            // UI update
            btnStart.Enabled = true;
            btnStop.Enabled = false;


        }

        
    }
}
