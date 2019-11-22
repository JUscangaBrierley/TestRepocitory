namespace FormatDBScript
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtScriptName = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.chkLoyaltyMember = new System.Windows.Forms.CheckBox();
            this.chkMemberDetails = new System.Windows.Forms.CheckBox();
            this.chkVirtualCard = new System.Windows.Forms.CheckBox();
            this.chkTxnHeader = new System.Windows.Forms.CheckBox();
            this.chkTxnDetailItem = new System.Windows.Forms.CheckBox();
            this.chkPointTransaction = new System.Windows.Forms.CheckBox();
            this.chkMemberReceipts = new System.Windows.Forms.CheckBox();
            this.chkMemberBraSummary = new System.Windows.Forms.CheckBox();
            this.chkWriteMasterOnly = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 205);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path and Filename of Script";
            // 
            // txtScriptName
            // 
            this.txtScriptName.Location = new System.Drawing.Point(32, 221);
            this.txtScriptName.Name = "txtScriptName";
            this.txtScriptName.Size = new System.Drawing.Size(470, 20);
            this.txtScriptName.TabIndex = 1;
            this.txtScriptName.Text = "C:\\DevRoot\\SQL Scripts\\AE\\Download from production";
            this.txtScriptName.TextChanged += new System.EventHandler(this.txtScriptName_TextChanged);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(275, 247);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 2;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // chkLoyaltyMember
            // 
            this.chkLoyaltyMember.AutoSize = true;
            this.chkLoyaltyMember.Checked = true;
            this.chkLoyaltyMember.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLoyaltyMember.Location = new System.Drawing.Point(32, 26);
            this.chkLoyaltyMember.Name = "chkLoyaltyMember";
            this.chkLoyaltyMember.Size = new System.Drawing.Size(97, 17);
            this.chkLoyaltyMember.TabIndex = 3;
            this.chkLoyaltyMember.Text = "LoyaltyMember";
            this.chkLoyaltyMember.UseVisualStyleBackColor = true;
            // 
            // chkMemberDetails
            // 
            this.chkMemberDetails.AutoSize = true;
            this.chkMemberDetails.Checked = true;
            this.chkMemberDetails.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMemberDetails.Location = new System.Drawing.Point(32, 49);
            this.chkMemberDetails.Name = "chkMemberDetails";
            this.chkMemberDetails.Size = new System.Drawing.Size(96, 17);
            this.chkMemberDetails.TabIndex = 4;
            this.chkMemberDetails.Text = "MemberDetails";
            this.chkMemberDetails.UseVisualStyleBackColor = true;
            // 
            // chkVirtualCard
            // 
            this.chkVirtualCard.AutoSize = true;
            this.chkVirtualCard.Checked = true;
            this.chkVirtualCard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVirtualCard.Location = new System.Drawing.Point(32, 72);
            this.chkVirtualCard.Name = "chkVirtualCard";
            this.chkVirtualCard.Size = new System.Drawing.Size(77, 17);
            this.chkVirtualCard.TabIndex = 5;
            this.chkVirtualCard.Text = "VirtualCard";
            this.chkVirtualCard.UseVisualStyleBackColor = true;
            // 
            // chkTxnHeader
            // 
            this.chkTxnHeader.AutoSize = true;
            this.chkTxnHeader.Checked = true;
            this.chkTxnHeader.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTxnHeader.Location = new System.Drawing.Point(32, 95);
            this.chkTxnHeader.Name = "chkTxnHeader";
            this.chkTxnHeader.Size = new System.Drawing.Size(79, 17);
            this.chkTxnHeader.TabIndex = 6;
            this.chkTxnHeader.Text = "TxnHeader";
            this.chkTxnHeader.UseVisualStyleBackColor = true;
            // 
            // chkTxnDetailItem
            // 
            this.chkTxnDetailItem.AutoSize = true;
            this.chkTxnDetailItem.Checked = true;
            this.chkTxnDetailItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTxnDetailItem.Location = new System.Drawing.Point(32, 118);
            this.chkTxnDetailItem.Name = "chkTxnDetailItem";
            this.chkTxnDetailItem.Size = new System.Drawing.Size(91, 17);
            this.chkTxnDetailItem.TabIndex = 7;
            this.chkTxnDetailItem.Text = "TxnDetailItem";
            this.chkTxnDetailItem.UseVisualStyleBackColor = true;
            // 
            // chkPointTransaction
            // 
            this.chkPointTransaction.AutoSize = true;
            this.chkPointTransaction.Checked = true;
            this.chkPointTransaction.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPointTransaction.Location = new System.Drawing.Point(32, 141);
            this.chkPointTransaction.Name = "chkPointTransaction";
            this.chkPointTransaction.Size = new System.Drawing.Size(106, 17);
            this.chkPointTransaction.TabIndex = 8;
            this.chkPointTransaction.Text = "PointTransaction";
            this.chkPointTransaction.UseVisualStyleBackColor = true;
            // 
            // chkMemberReceipts
            // 
            this.chkMemberReceipts.AutoSize = true;
            this.chkMemberReceipts.Location = new System.Drawing.Point(32, 164);
            this.chkMemberReceipts.Name = "chkMemberReceipts";
            this.chkMemberReceipts.Size = new System.Drawing.Size(106, 17);
            this.chkMemberReceipts.TabIndex = 9;
            this.chkMemberReceipts.Text = "MemberReceipts";
            this.chkMemberReceipts.UseVisualStyleBackColor = true;
            // 
            // chkMemberBraSummary
            // 
            this.chkMemberBraSummary.AutoSize = true;
            this.chkMemberBraSummary.Location = new System.Drawing.Point(187, 26);
            this.chkMemberBraSummary.Name = "chkMemberBraSummary";
            this.chkMemberBraSummary.Size = new System.Drawing.Size(123, 17);
            this.chkMemberBraSummary.TabIndex = 10;
            this.chkMemberBraSummary.Text = "MemberBraSummary";
            this.chkMemberBraSummary.UseVisualStyleBackColor = true;
            // 
            // chkWriteMasterOnly
            // 
            this.chkWriteMasterOnly.AutoSize = true;
            this.chkWriteMasterOnly.Location = new System.Drawing.Point(417, 164);
            this.chkWriteMasterOnly.Name = "chkWriteMasterOnly";
            this.chkWriteMasterOnly.Size = new System.Drawing.Size(110, 17);
            this.chkWriteMasterOnly.TabIndex = 11;
            this.chkWriteMasterOnly.Text = "Write Master Only";
            this.chkWriteMasterOnly.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 334);
            this.Controls.Add(this.chkWriteMasterOnly);
            this.Controls.Add(this.chkMemberBraSummary);
            this.Controls.Add(this.chkMemberReceipts);
            this.Controls.Add(this.chkPointTransaction);
            this.Controls.Add(this.chkTxnDetailItem);
            this.Controls.Add(this.chkTxnHeader);
            this.Controls.Add(this.chkVirtualCard);
            this.Controls.Add(this.chkMemberDetails);
            this.Controls.Add(this.chkLoyaltyMember);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.txtScriptName);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtScriptName;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.CheckBox chkLoyaltyMember;
        private System.Windows.Forms.CheckBox chkMemberDetails;
        private System.Windows.Forms.CheckBox chkVirtualCard;
        private System.Windows.Forms.CheckBox chkTxnHeader;
        private System.Windows.Forms.CheckBox chkTxnDetailItem;
        private System.Windows.Forms.CheckBox chkPointTransaction;
        private System.Windows.Forms.CheckBox chkMemberReceipts;
        private System.Windows.Forms.CheckBox chkMemberBraSummary;
        private System.Windows.Forms.CheckBox chkWriteMasterOnly;
    }
}

