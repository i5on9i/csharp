namespace FuturesTrader
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.gvResultBoard = new System.Windows.Forms.DataGridView();
            this.gvInput = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gbResultBoard = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.gbInput = new System.Windows.Forms.GroupBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gvResultBoard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbResultBoard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.gbInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // gvResultBoard
            // 
            this.gvResultBoard.AllowUserToAddRows = false;
            this.gvResultBoard.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvResultBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvResultBoard.GridColor = System.Drawing.SystemColors.Control;
            this.gvResultBoard.Location = new System.Drawing.Point(3, 17);
            this.gvResultBoard.Name = "gvResultBoard";
            this.gvResultBoard.ReadOnly = true;
            this.gvResultBoard.RowHeadersVisible = false;
            this.gvResultBoard.RowTemplate.Height = 23;
            this.gvResultBoard.Size = new System.Drawing.Size(790, 221);
            this.gvResultBoard.TabIndex = 2;
            this.gvResultBoard.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.gvResultBoard_DataError);
            // 
            // gvInput
            // 
            this.gvInput.AllowUserToAddRows = false;
            this.gvInput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvInput.Location = new System.Drawing.Point(3, 17);
            this.gvInput.Name = "gvInput";
            this.gvInput.RowTemplate.Height = 23;
            this.gvInput.Size = new System.Drawing.Size(363, 160);
            this.gvInput.TabIndex = 3;
            this.gvInput.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvInput_CellValidated);
            this.gvInput.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.gvInput_CellValidating);
            this.gvInput.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvInput_CellValueChanged);
            this.gvInput.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.gvInput_DataError);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(1, 48);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbResultBoard);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1169, 241);
            this.splitContainer1.SplitterDistance = 796;
            this.splitContainer1.TabIndex = 5;
            // 
            // gbResultBoard
            // 
            this.gbResultBoard.Controls.Add(this.gvResultBoard);
            this.gbResultBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbResultBoard.Location = new System.Drawing.Point(0, 0);
            this.gbResultBoard.Name = "gbResultBoard";
            this.gbResultBoard.Size = new System.Drawing.Size(796, 241);
            this.gbResultBoard.TabIndex = 6;
            this.gbResultBoard.TabStop = false;
            this.gbResultBoard.Text = "Result Board";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.gbInput);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.btnStop);
            this.splitContainer2.Panel2.Controls.Add(this.btnStart);
            this.splitContainer2.Size = new System.Drawing.Size(369, 241);
            this.splitContainer2.SplitterDistance = 180;
            this.splitContainer2.TabIndex = 2;
            // 
            // gbInput
            // 
            this.gbInput.Controls.Add(this.gvInput);
            this.gbInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbInput.Location = new System.Drawing.Point(0, 0);
            this.gbInput.Name = "gbInput";
            this.gbInput.Size = new System.Drawing.Size(369, 180);
            this.gbInput.TabIndex = 6;
            this.gbInput.TabStop = false;
            this.gbInput.Text = "Inputs";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(47, 18);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(137, 18);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1174, 301);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "Trader";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvResultBoard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvInput)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gbResultBoard.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.gbInput.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gvResultBoard;
        private System.Windows.Forms.DataGridView gvInput;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox gbInput;
        private System.Windows.Forms.GroupBox gbResultBoard;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
    }
}

