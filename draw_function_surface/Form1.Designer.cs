namespace Horizon
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
			this.pnPlot = new System.Windows.Forms.PictureBox();
			this.tbFunction = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tbScale = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tbStep = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tbFront = new System.Windows.Forms.TextBox();
			this.tbBack = new System.Windows.Forms.TextBox();
			this.btnDraw = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// pnPlot
			// 
			this.pnPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pnPlot.Location = new System.Drawing.Point(10, 16);
			this.pnPlot.Name = "pnPlot";
			this.pnPlot.Size = new System.Drawing.Size(700, 522);
			this.pnPlot.TabIndex = 0;
			// 
			// tbFunction
			// 
			this.tbFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbFunction.Location = new System.Drawing.Point(802, 24);
			this.tbFunction.Name = "tbFunction";
			this.tbFunction.Size = new System.Drawing.Size(126, 22);
			this.tbFunction.TabIndex = 1;
			this.tbFunction.Text = "Math.Cos(x*y)*Math.Sin(x*y)";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(716, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Функция:";
			// 
			// tbScale
			// 
			this.tbScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbScale.Location = new System.Drawing.Point(802, 52);
			this.tbScale.Name = "tbScale";
			this.tbScale.Size = new System.Drawing.Size(126, 22);
			this.tbScale.TabIndex = 3;
			this.tbScale.Text = "0.0";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(716, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "Масштаб:";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(716, 83);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(36, 17);
			this.label3.TabIndex = 5;
			this.label3.Text = "Шаг:";
			// 
			// tbStep
			// 
			this.tbStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbStep.Location = new System.Drawing.Point(802, 83);
			this.tbStep.Name = "tbStep";
			this.tbStep.Size = new System.Drawing.Size(126, 22);
			this.tbStep.TabIndex = 6;
			this.tbStep.Text = "0.0";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(716, 113);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(54, 17);
			this.label4.TabIndex = 7;
			this.label4.Text = "Перед:";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(716, 143);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 17);
			this.label5.TabIndex = 8;
			this.label5.Text = "Зад:";
			// 
			// tbFront
			// 
			this.tbFront.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbFront.Location = new System.Drawing.Point(802, 113);
			this.tbFront.Name = "tbFront";
			this.tbFront.Size = new System.Drawing.Size(126, 22);
			this.tbFront.TabIndex = 9;
			this.tbFront.Text = "0.0";
			// 
			// tbBack
			// 
			this.tbBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbBack.Location = new System.Drawing.Point(802, 143);
			this.tbBack.Name = "tbBack";
			this.tbBack.Size = new System.Drawing.Size(126, 22);
			this.tbBack.TabIndex = 10;
			this.tbBack.Text = "0.0";
			// 
			// btnDraw
			// 
			this.btnDraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDraw.Location = new System.Drawing.Point(719, 177);
			this.btnDraw.Name = "btnDraw";
			this.btnDraw.Size = new System.Drawing.Size(208, 44);
			this.btnDraw.TabIndex = 11;
			this.btnDraw.Text = "Нарисовать";
			this.btnDraw.UseVisualStyleBackColor = true;
			this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(938, 548);
			this.Controls.Add(this.btnDraw);
			this.Controls.Add(this.tbBack);
			this.Controls.Add(this.tbFront);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tbStep);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbScale);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbFunction);
			this.Controls.Add(this.pnPlot);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pnPlot;
		private System.Windows.Forms.TextBox tbFunction;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbScale;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbStep;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbFront;
		private System.Windows.Forms.TextBox tbBack;
		private System.Windows.Forms.Button btnDraw;
	}
}

