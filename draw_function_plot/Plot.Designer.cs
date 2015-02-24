namespace ComputerGraphics
{
	partial class Plot
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
			this.pnPlot = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tbFunc = new System.Windows.Forms.TextBox();
			this.tbX1 = new System.Windows.Forms.TextBox();
			this.tbX2 = new System.Windows.Forms.TextBox();
			this.btnDraw = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// pnPlot
			// 
			this.pnPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pnPlot.Location = new System.Drawing.Point(12, 13);
			this.pnPlot.Name = "pnPlot";
			this.pnPlot.Size = new System.Drawing.Size(761, 641);
			this.pnPlot.TabIndex = 0;
			this.pnPlot.Paint += new System.Windows.Forms.PaintEventHandler(this.pnPlot_Paint);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(787, 65);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(47, 17);
			this.label1.TabIndex = 1;
			this.label1.Text = "X min:";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(787, 123);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "X max:";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(787, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 17);
			this.label3.TabIndex = 3;
			this.label3.Text = "f(x):";
			// 
			// tbFunc
			// 
			this.tbFunc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbFunc.Location = new System.Drawing.Point(790, 33);
			this.tbFunc.Name = "tbFunc";
			this.tbFunc.Size = new System.Drawing.Size(122, 22);
			this.tbFunc.TabIndex = 4;
			this.tbFunc.Text = "x*Math.Sin(x)";
			// 
			// tbX1
			// 
			this.tbX1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbX1.Location = new System.Drawing.Point(790, 85);
			this.tbX1.Name = "tbX1";
			this.tbX1.Size = new System.Drawing.Size(122, 22);
			this.tbX1.TabIndex = 5;
			this.tbX1.Text = "-30";
			// 
			// tbX2
			// 
			this.tbX2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbX2.Location = new System.Drawing.Point(790, 143);
			this.tbX2.Name = "tbX2";
			this.tbX2.Size = new System.Drawing.Size(122, 22);
			this.tbX2.TabIndex = 6;
			this.tbX2.Text = "30";
			// 
			// btnDraw
			// 
			this.btnDraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDraw.Location = new System.Drawing.Point(790, 181);
			this.btnDraw.Name = "btnDraw";
			this.btnDraw.Size = new System.Drawing.Size(121, 38);
			this.btnDraw.TabIndex = 7;
			this.btnDraw.Text = "Перерисовать";
			this.btnDraw.UseVisualStyleBackColor = true;
			this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
			// 
			// Plot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(924, 666);
			this.Controls.Add(this.btnDraw);
			this.Controls.Add(this.tbX2);
			this.Controls.Add(this.tbX1);
			this.Controls.Add(this.tbFunc);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pnPlot);
			this.Name = "Plot";
			this.Text = "Plot";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel pnPlot;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbFunc;
		private System.Windows.Forms.TextBox tbX1;
		private System.Windows.Forms.TextBox tbX2;
		private System.Windows.Forms.Button btnDraw;
	}
}

