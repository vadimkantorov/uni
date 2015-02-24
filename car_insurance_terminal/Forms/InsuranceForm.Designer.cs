namespace Insurance.Forms
{
	partial class InsuranceForm
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnProceed = new System.Windows.Forms.Button();
			this.lblTitle = new System.Windows.Forms.Label();
			this.pnKeyboard = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnCancel.Location = new System.Drawing.Point(3, 826);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(770, 74);
			this.btnCancel.TabIndex = 12;
			this.btnCancel.Text = "Отмена";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnProceed
			// 
			this.btnProceed.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnProceed.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnProceed.Location = new System.Drawing.Point(779, 826);
			this.btnProceed.Name = "btnProceed";
			this.btnProceed.Size = new System.Drawing.Size(660, 74);
			this.btnProceed.TabIndex = 13;
			this.btnProceed.Text = "Продолжить";
			this.btnProceed.UseVisualStyleBackColor = true;
			this.btnProceed.Click += new System.EventHandler(this.btnProceed_Click);
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblTitle.Location = new System.Drawing.Point(560, 9);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(337, 46);
			this.lblTitle.TabIndex = 14;
			this.lblTitle.Text = "Название экрана";
			// 
			// pnKeyboard
			// 
			this.pnKeyboard.Location = new System.Drawing.Point(3, 91);
			this.pnKeyboard.Name = "pnKeyboard";
			this.pnKeyboard.Size = new System.Drawing.Size(1436, 729);
			this.pnKeyboard.TabIndex = 15;
			// 
			// InsuranceForm
			// 
			this.AcceptButton = this.btnProceed;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(1274, 912);
			this.Controls.Add(this.pnKeyboard);
			this.Controls.Add(this.lblTitle);
			this.Controls.Add(this.btnProceed);
			this.Controls.Add(this.btnCancel);
			this.Name = "InsuranceForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "InsuranceForm";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.InsuranceForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnProceed;
		private System.Windows.Forms.Label lblTitle;
		protected System.Windows.Forms.Panel pnKeyboard;


	}
}