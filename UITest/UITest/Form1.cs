using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UITest
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();

            string message = "You did not enter a server name. Cancel this operation?";
            string caption = "No Server Name Specified";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(this, message, caption, buttons);

            if (result == DialogResult.Yes)
            {

                // Closes the parent form.

                this.Close();

            }


            //dataGridView1.DoubleBuffered(true);
            //dataGridView1.AutoResizeColumns();
            //dataGridView1.AutoSizeColumnsMode =
            //    DataGridViewAutoSizeColumnsMode.DisplayedCells;

            dataGridView1.ColumnCount = 6;

            // Set the column header names.
            dataGridView1.Columns[0].Name = "Recipe";
            dataGridView1.Columns[1].Name = "Category";
            dataGridView1.Columns[2].Name = "Third";
            dataGridView1.Columns[3].Name = "Rating";

         


            

            string[] row0 = { "C# 3.0 Pocket Reference", "Albahari", "O'Reilly", "Sebastopol, CA", "2008" };
            string[] row1 = { "CLR via C#", "Richter", "Microsoft", "Redmond, WA", "2006" };
            string[] row2 = { "Mastering Regular Expressions", "Friedl", "O'Reilly", "Sebastopol, CA", "1997" };
            string[] row3 = { "C++ Primer", "Lippman, Lajoie", "Addison-Wesley", "Massachusetts", "1998" };
            string[] row4 = { "C++ Programming Style", "Cargill", "Addison-Wesley", "Massachusetts", "1992" };
            string[] row5 = { "The C Programming Language", "Kernighan, Ritchie", "Bell Labs", "USA", "1988" };

            dataGridView1.Rows.Add(row0);
            dataGridView1.Rows.Add(row1);
            dataGridView1.Rows.Add(row2);
            dataGridView1.Rows.Add(row3);
            dataGridView1.Rows.Add(row4);
            dataGridView1.Rows.Add(row5);

            Console.WriteLine(String.Format("{0:.00}", 15.626));
            Console.WriteLine("{0}", Math.Round(15.6,2));

            const int AVGTICKCOUNT = 1;
            const int OFFSET = 1 << 1;
            const int TRADESIZE = 1 << 2;
            const int SIGNALINTERVAL = 1 << 3;
            const int POSLIMIT = 1 << 4;
            const int STOPLOSS = 1 << 5;

            int input = POSLIMIT | TRADESIZE;
            Console.WriteLine("{0}", OFFSET);
            Console.WriteLine("{0}", OFFSET | TRADESIZE);
            for (int i = 1; i < (1 << 6); i <<= 1)
            {
                //Console.WriteLine("{0}", i);
                Console.WriteLine("{0}", input & i);

            }
            



            

            ////
            // how to change a cell's template
            //
            //DataGridViewCheckBoxCell CheckBoxCell = new DataGridViewCheckBoxCell();
            //CheckBoxCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //this.dataGridView1[0, 2] = CheckBoxCell;
            //this.dataGridView1[0, 2].Value = true;


#if false
            DataTable dt = new DataTable();
            dt.Columns.Add("name");
            for (int j = 0; j < 10; j++)
            {
                dt.Rows.Add("");
            }
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Columns[0].Width = 200;


            /*
             * First method : Convert to an existed cell type such ComboBox cell,etc
             */

            DataGridViewComboBoxCell ComboBoxCell = new DataGridViewComboBoxCell();
            ComboBoxCell.Items.AddRange(new string[] { "aaa", "bbb", "ccc" });
            this.dataGridView1[0, 0] = ComboBoxCell;
            this.dataGridView1[0, 0].Value = "bbb";

            DataGridViewTextBoxCell TextBoxCell = new DataGridViewTextBoxCell();
            this.dataGridView1[0, 1] = TextBoxCell;
            this.dataGridView1[0, 1].Value = "some text";

            DataGridViewCheckBoxCell CheckBoxCell = new DataGridViewCheckBoxCell();
            CheckBoxCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1[0, 2] = CheckBoxCell;
            this.dataGridView1[0, 2].Value = true;

            /*
             * Second method : Add control to the host in the cell
             */
            DateTimePicker dtp = new DateTimePicker();
            dtp.Value = DateTime.Now.AddDays(-10);
            //add DateTimePicker into the control collection of the DataGridView
            this.dataGridView1.Controls.Add(dtp);
            //set its location and size to fit the cell
            dtp.Location = this.dataGridView1.GetCellDisplayRectangle(0, 3, true).Location;
            dtp.Size = this.dataGridView1.GetCellDisplayRectangle(0, 3, true).Size;
#endif




        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            Console.WriteLine("cell validating");
            const int INPUT_AVGTICKCOUNT = 0;
            const int INPUT_OFFSET = 1;
            const int INPUT_TRADESIZE = 2;
            const int INPUT_SIGNALINTERVAL = 3;
            const int INPUT_POSLIMIT = 4;
            const int INPUT_STOPLOSS = 5;

            long l;
            double d;
            int i;
            DataGridView dgv_Data = sender as DataGridView;
            if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
            {
                dgv_Data.Rows[e.RowIndex].ErrorText = "Field cannot be empty";
                e.Cancel = true;
            }
            else
            {
                switch (e.RowIndex)
                {
                    case INPUT_AVGTICKCOUNT:
                        if (!int.TryParse(e.FormattedValue.ToString(), out i)
                            || (i <= 0))
                        {
                            // not parsable to int
                            dgv_Data.Rows[e.RowIndex].ErrorText = "Average Tick Count must be a Natural number";
                            e.Cancel = true;
                        }
                        break;
                    case INPUT_OFFSET:
                        if (!double.TryParse(e.FormattedValue.ToString(), out d)
                            || (d <= 0))
                        {
                            // not parsable to int
                            dgv_Data.Rows[e.RowIndex].ErrorText = "offset must be a positive real number";
                            e.Cancel = true;
                        }
                        break;
                    case INPUT_TRADESIZE:
                        if (!int.TryParse(e.FormattedValue.ToString(), out i)
                            || (i <= 0))
                        {
                            // not parsable to int
                            dgv_Data.Rows[e.RowIndex].ErrorText = "Trade size must be a Natural number";
                            e.Cancel = true;
                        }
                        break;
                    case INPUT_SIGNALINTERVAL:
                        if (!int.TryParse(e.FormattedValue.ToString(), out i)
                            || (i <= 0))
                        {
                            // not parsable to int
                            dgv_Data.Rows[e.RowIndex].ErrorText = "Signal Interval must be a Natural number";
                            e.Cancel = true;
                        }
                        break;
                    case INPUT_POSLIMIT:
                        if (!int.TryParse(e.FormattedValue.ToString(), out i)
                            || (i <= 0))
                        {
                            // not parsable to int
                            dgv_Data.Rows[e.RowIndex].ErrorText = "Position Limit must be a Natural number";
                            e.Cancel = true;

                            // Must be more than orders' count
                            
                        }
                        
                        break;
                    case INPUT_STOPLOSS:
                        if (!long.TryParse(e.FormattedValue.ToString(), out l)
                            || (l <= 0))
                        {
                            // not parsable to int
                            dgv_Data.Rows[e.RowIndex].ErrorText = "Stop Loss must be a Natural number";
                            e.Cancel = true;

                        }
                        break;
                    default:
                        // O.K case
                        dgv_Data.Rows[e.RowIndex].ErrorText = String.Empty;
                        e.Cancel = false;
                        break;
                }
                
            }
            

        }

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine("validated");
        }

       

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {

            dataGridView1.RowsDefaultCellStyle.SelectionBackColor = Color.BlanchedAlmond;
            if(e.RowIndex > -1)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
            }
            

        }
    }
}
