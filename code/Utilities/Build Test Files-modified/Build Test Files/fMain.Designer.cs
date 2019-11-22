namespace BuildTestFiles
{
    partial class fMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnGenerateFile = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtStartingTxnDate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtStartingTxnNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtStartingLoyaltyNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGenerateBulkTlog = new System.Windows.Forms.Button();
            this.btnClose2 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtNumberOfTxns = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtNumberOfTxnsPerLoyaltyNumber = new System.Windows.Forms.TextBox();
            this.chkGenerateEnrollment = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(509, 119);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 5;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(45, 119);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(456, 20);
            this.txtFileName.TabIndex = 4;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(478, 165);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(104, 24);
            this.btnClose.TabIndex = 19;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(21, 178);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 22;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnGenerateFile
            // 
            this.btnGenerateFile.Location = new System.Drawing.Point(352, 165);
            this.btnGenerateFile.Name = "btnGenerateFile";
            this.btnGenerateFile.Size = new System.Drawing.Size(120, 24);
            this.btnGenerateFile.TabIndex = 23;
            this.btnGenerateFile.TabStop = false;
            this.btnGenerateFile.Text = "Generate File";
            this.btnGenerateFile.Click += new System.EventHandler(this.btnGenerateFile_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1028, 600);
            this.tabControl1.TabIndex = 24;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.btnGenerateFile);
            this.tabPage1.Controls.Add(this.txtFileName);
            this.tabPage1.Controls.Add(this.btnBrowse);
            this.tabPage1.Controls.Add(this.btnClose);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1020, 574);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Excel Config Files";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(42, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(460, 20);
            this.label1.TabIndex = 24;
            this.label1.Text = "Generate Test Files from Excel Spreadsheet Config files";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chkGenerateEnrollment);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.txtNumberOfTxnsPerLoyaltyNumber);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.txtNumberOfTxns);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.txtStartingTxnDate);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.txtStartingTxnNumber);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.txtStartingLoyaltyNumber);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.btnGenerateBulkTlog);
            this.tabPage2.Controls.Add(this.btnClose2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1020, 574);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Bulk Tlog File";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(402, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(138, 13);
            this.label6.TabIndex = 34;
            this.label6.Text = "Default is 1 year from today.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(148, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Starting Transaction Date";
            // 
            // txtStartingTxnDate
            // 
            this.txtStartingTxnDate.Location = new System.Drawing.Point(296, 175);
            this.txtStartingTxnDate.Name = "txtStartingTxnDate";
            this.txtStartingTxnDate.Size = new System.Drawing.Size(100, 20);
            this.txtStartingTxnDate.TabIndex = 32;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(148, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(142, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Starting Transaction Number";
            // 
            // txtStartingTxnNumber
            // 
            this.txtStartingTxnNumber.Location = new System.Drawing.Point(296, 144);
            this.txtStartingTxnNumber.Name = "txtStartingTxnNumber";
            this.txtStartingTxnNumber.Size = new System.Drawing.Size(100, 20);
            this.txtStartingTxnNumber.TabIndex = 30;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(148, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Starting Loyalty Number";
            // 
            // txtStartingLoyaltyNumber
            // 
            this.txtStartingLoyaltyNumber.Location = new System.Drawing.Point(296, 116);
            this.txtStartingLoyaltyNumber.Name = "txtStartingLoyaltyNumber";
            this.txtStartingLoyaltyNumber.Size = new System.Drawing.Size(100, 20);
            this.txtStartingLoyaltyNumber.TabIndex = 28;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(147, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(192, 20);
            this.label2.TabIndex = 27;
            this.label2.Text = "Generate bulk tlog file ";
            // 
            // btnGenerateBulkTlog
            // 
            this.btnGenerateBulkTlog.Location = new System.Drawing.Point(550, 340);
            this.btnGenerateBulkTlog.Name = "btnGenerateBulkTlog";
            this.btnGenerateBulkTlog.Size = new System.Drawing.Size(120, 24);
            this.btnGenerateBulkTlog.TabIndex = 26;
            this.btnGenerateBulkTlog.TabStop = false;
            this.btnGenerateBulkTlog.Text = "Generate File";
            this.btnGenerateBulkTlog.Click += new System.EventHandler(this.btnGenerateBulkTlog_Click);
            // 
            // btnClose2
            // 
            this.btnClose2.Location = new System.Drawing.Point(676, 340);
            this.btnClose2.Name = "btnClose2";
            this.btnClose2.Size = new System.Drawing.Size(104, 24);
            this.btnClose2.TabIndex = 25;
            this.btnClose2.Text = "Close";
            this.btnClose2.Click += new System.EventHandler(this.btnClose2_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(148, 215);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 36;
            this.label7.Text = "Number of Txns";
            // 
            // txtNumberOfTxns
            // 
            this.txtNumberOfTxns.Location = new System.Drawing.Point(296, 212);
            this.txtNumberOfTxns.Name = "txtNumberOfTxns";
            this.txtNumberOfTxns.Size = new System.Drawing.Size(100, 20);
            this.txtNumberOfTxns.TabIndex = 35;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(402, 122);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(173, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Number of Txns per LoyaltyNumber";
            // 
            // txtNumberOfTxnsPerLoyaltyNumber
            // 
            this.txtNumberOfTxnsPerLoyaltyNumber.Location = new System.Drawing.Point(581, 119);
            this.txtNumberOfTxnsPerLoyaltyNumber.Name = "txtNumberOfTxnsPerLoyaltyNumber";
            this.txtNumberOfTxnsPerLoyaltyNumber.Size = new System.Drawing.Size(100, 20);
            this.txtNumberOfTxnsPerLoyaltyNumber.TabIndex = 37;
            // 
            // chkGenerateEnrollment
            // 
            this.chkGenerateEnrollment.AutoSize = true;
            this.chkGenerateEnrollment.Checked = true;
            this.chkGenerateEnrollment.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGenerateEnrollment.Location = new System.Drawing.Point(296, 255);
            this.chkGenerateEnrollment.Name = "chkGenerateEnrollment";
            this.chkGenerateEnrollment.Size = new System.Drawing.Size(163, 17);
            this.chkGenerateEnrollment.TabIndex = 39;
            this.chkGenerateEnrollment.Text = "Generate POS Enrollment file";
            this.chkGenerateEnrollment.UseVisualStyleBackColor = true;
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 612);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblStatus);
            this.Name = "fMain";
            this.Text = "Generate Test Files";
            this.Load += new System.EventHandler(this.fMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnGenerateFile;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtStartingTxnDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtStartingTxnNumber;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtStartingLoyaltyNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnGenerateBulkTlog;
        private System.Windows.Forms.Button btnClose2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtNumberOfTxnsPerLoyaltyNumber;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtNumberOfTxns;
        private System.Windows.Forms.CheckBox chkGenerateEnrollment;
    }
}

