namespace Insurance.Forms
{
	partial class LimitationsForm
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
			this.btnWithLimitations = new System.Windows.Forms.Button();
			this.btnWithoutLimitations = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnWithLimitations
			// 
			this.btnWithLimitations.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnWithLimitations.Location = new System.Drawing.Point(850, 277);
			this.btnWithLimitations.Name = "btnWithLimitations";
			this.btnWithLimitations.Size = new System.Drawing.Size(477, 358);
			this.btnWithLimitations.TabIndex = 17;
			this.btnWithLimitations.Text = "С ограничениями";
			this.btnWithLimitations.UseVisualStyleBackColor = true;
			this.btnWithLimitations.Click += new System.EventHandler(this.btnWithLimitations_Click);
			// 
			// btnWithoutLimitations
			// 
			this.btnWithoutLimitations.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnWithoutLimitations.Location = new System.Drawing.Point(124, 277);
			this.btnWithoutLimitations.Name = "btnWithoutLimitations";
			this.btnWithoutLimitations.Size = new System.Drawing.Size(487, 358);
			this.btnWithoutLimitations.TabIndex = 16;
			this.btnWithoutLimitations.Text = "Без ограничений";
			this.btnWithoutLimitations.UseVisualStyleBackColor = true;
			this.btnWithoutLimitations.Click += new System.EventHandler(this.btnWithoutLimitations_Click);
			// 
			// LimitationsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1451, 912);
			this.Controls.Add(this.btnWithLimitations);
			this.Controls.Add(this.btnWithoutLimitations);
			this.Name = "LimitationsForm";
			this.Text = "LimitationsForm";
			this.Controls.SetChildIndex(this.pnKeyboard, 0);
			this.Controls.SetChildIndex(this.btnWithoutLimitations, 0);
			this.Controls.SetChildIndex(this.btnWithLimitations, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnWithLimitations;
		private System.Windows.Forms.Button btnWithoutLimitations;
	}
}