namespace FormatData
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
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.chkLineBreaks = new System.Windows.Forms.CheckBox();
            this.chkIncludeQuotes = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(12, 25);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(208, 700);
            this.txtInput.TabIndex = 0;
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(296, 25);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(208, 700);
            this.txtOutput.TabIndex = 1;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(217, 731);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 2;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // chkLineBreaks
            // 
            this.chkLineBreaks.AutoSize = true;
            this.chkLineBreaks.Checked = true;
            this.chkLineBreaks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLineBreaks.Location = new System.Drawing.Point(428, 731);
            this.chkLineBreaks.Name = "chkLineBreaks";
            this.chkLineBreaks.Size = new System.Drawing.Size(114, 17);
            this.chkLineBreaks.TabIndex = 3;
            this.chkLineBreaks.Text = "include line breaks";
            this.chkLineBreaks.UseVisualStyleBackColor = true;
            // 
            // chkIncludeQuotes
            // 
            this.chkIncludeQuotes.AutoSize = true;
            this.chkIncludeQuotes.Checked = true;
            this.chkIncludeQuotes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIncludeQuotes.Location = new System.Drawing.Point(298, 731);
            this.chkIncludeQuotes.Name = "chkIncludeQuotes";
            this.chkIncludeQuotes.Size = new System.Drawing.Size(95, 17);
            this.chkIncludeQuotes.TabIndex = 4;
            this.chkIncludeQuotes.Text = "include quotes";
            this.chkIncludeQuotes.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 765);
            this.Controls.Add(this.chkIncludeQuotes);
            this.Controls.Add(this.chkLineBreaks);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.txtInput);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.CheckBox chkLineBreaks;
        private System.Windows.Forms.CheckBox chkIncludeQuotes;
    }
}

