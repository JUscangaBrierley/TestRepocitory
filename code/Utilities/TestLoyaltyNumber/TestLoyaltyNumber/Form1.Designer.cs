namespace TestLoyaltyNumber
{
    partial class Form1
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
            this.txtLoyaltyNumber = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblRawNumber = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblLID4 = new System.Windows.Forms.Label();
            this.lblLID8 = new System.Windows.Forms.Label();
            this.lblLID12 = new System.Windows.Forms.Label();
            this.lblLID14 = new System.Windows.Forms.Label();
            this.lblCalc4 = new System.Windows.Forms.Label();
            this.lblCalc8 = new System.Windows.Forms.Label();
            this.lblCalc12 = new System.Windows.Forms.Label();
            this.lblCalc14 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLoyaltyNumber
            // 
            this.txtLoyaltyNumber.Location = new System.Drawing.Point(345, 204);
            this.txtLoyaltyNumber.Name = "txtLoyaltyNumber";
            this.txtLoyaltyNumber.Size = new System.Drawing.Size(100, 20);
            this.txtLoyaltyNumber.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(345, 185);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Loyalty Number";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(345, 241);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 2;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(345, 276);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Raw Number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Check Digit 4";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Check Digit 8";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Check Digit 12";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(31, 207);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Check Digit 14";
            // 
            // lblRawNumber
            // 
            this.lblRawNumber.AutoSize = true;
            this.lblRawNumber.Location = new System.Drawing.Point(119, 34);
            this.lblRawNumber.Name = "lblRawNumber";
            this.lblRawNumber.Size = new System.Drawing.Size(0, 13);
            this.lblRawNumber.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(119, 89);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "From LoyaltyNumber";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(228, 89);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Calculated";
            // 
            // lblLID4
            // 
            this.lblLID4.AutoSize = true;
            this.lblLID4.Location = new System.Drawing.Point(119, 109);
            this.lblLID4.Name = "lblLID4";
            this.lblLID4.Size = new System.Drawing.Size(0, 13);
            this.lblLID4.TabIndex = 12;
            // 
            // lblLID8
            // 
            this.lblLID8.AutoSize = true;
            this.lblLID8.Location = new System.Drawing.Point(119, 142);
            this.lblLID8.Name = "lblLID8";
            this.lblLID8.Size = new System.Drawing.Size(0, 13);
            this.lblLID8.TabIndex = 13;
            // 
            // lblLID12
            // 
            this.lblLID12.AutoSize = true;
            this.lblLID12.Location = new System.Drawing.Point(119, 173);
            this.lblLID12.Name = "lblLID12";
            this.lblLID12.Size = new System.Drawing.Size(0, 13);
            this.lblLID12.TabIndex = 14;
            // 
            // lblLID14
            // 
            this.lblLID14.AutoSize = true;
            this.lblLID14.Location = new System.Drawing.Point(119, 207);
            this.lblLID14.Name = "lblLID14";
            this.lblLID14.Size = new System.Drawing.Size(0, 13);
            this.lblLID14.TabIndex = 15;
            // 
            // lblCalc4
            // 
            this.lblCalc4.AutoSize = true;
            this.lblCalc4.Location = new System.Drawing.Point(228, 109);
            this.lblCalc4.Name = "lblCalc4";
            this.lblCalc4.Size = new System.Drawing.Size(0, 13);
            this.lblCalc4.TabIndex = 16;
            // 
            // lblCalc8
            // 
            this.lblCalc8.AutoSize = true;
            this.lblCalc8.Location = new System.Drawing.Point(228, 142);
            this.lblCalc8.Name = "lblCalc8";
            this.lblCalc8.Size = new System.Drawing.Size(0, 13);
            this.lblCalc8.TabIndex = 17;
            // 
            // lblCalc12
            // 
            this.lblCalc12.AutoSize = true;
            this.lblCalc12.Location = new System.Drawing.Point(228, 173);
            this.lblCalc12.Name = "lblCalc12";
            this.lblCalc12.Size = new System.Drawing.Size(0, 13);
            this.lblCalc12.TabIndex = 18;
            // 
            // lblCalc14
            // 
            this.lblCalc14.AutoSize = true;
            this.lblCalc14.Location = new System.Drawing.Point(228, 207);
            this.lblCalc14.Name = "lblCalc14";
            this.lblCalc14.Size = new System.Drawing.Size(0, 13);
            this.lblCalc14.TabIndex = 19;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(345, 325);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 21;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(56, 325);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(283, 20);
            this.txtFileName.TabIndex = 20;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(345, 370);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 22;
            this.button1.Text = "Validate List";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(628, 437);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.lblCalc14);
            this.Controls.Add(this.lblCalc12);
            this.Controls.Add(this.lblCalc8);
            this.Controls.Add(this.lblCalc4);
            this.Controls.Add(this.lblLID14);
            this.Controls.Add(this.lblLID12);
            this.Controls.Add(this.lblLID8);
            this.Controls.Add(this.lblLID4);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblRawNumber);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLoyaltyNumber);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLoyaltyNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblRawNumber;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblLID4;
        private System.Windows.Forms.Label lblLID8;
        private System.Windows.Forms.Label lblLID12;
        private System.Windows.Forms.Label lblLID14;
        private System.Windows.Forms.Label lblCalc4;
        private System.Windows.Forms.Label lblCalc8;
        private System.Windows.Forms.Label lblCalc12;
        private System.Windows.Forms.Label lblCalc14;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button1;
    }
}

