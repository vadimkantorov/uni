namespace Insurance.Forms
{
	partial class GenericForm
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
			this.tbMasked = new System.Windows.Forms.MaskedTextBox();
			this.pnKeyboard.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnKeyboard
			// 
			this.pnKeyboard.Controls.Add(this.tbMasked);
			// 
			// tbMasked
			// 
			this.tbMasked.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tbMasked.Location = new System.Drawing.Point(24, 26);
			this.tbMasked.Name = "tbMasked";
			this.tbMasked.Size = new System.Drawing.Size(950, 98);
			this.tbMasked.TabIndex = 0;
			this.tbMasked.Enter += new System.EventHandler(this.tbMasked_Enter);
			// 
			// GenericForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1476, 925);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "GenericForm";
			this.Text = "GenericForm";
			this.Load += new System.EventHandler(this.GenericForm_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GenericForm_FormClosing);
			this.pnKeyboard.ResumeLayout(false);
			this.pnKeyboard.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MaskedTextBox tbMasked;
	}
}