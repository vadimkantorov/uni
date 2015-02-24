namespace Ellipse
{
	partial class Ellipse
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
			this.tbA = new System.Windows.Forms.TextBox();
			this.tbB = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbDx = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbDy = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.btnRedraw = new System.Windows.Forms.Button();
			this.pnPlot = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pnPlot)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(675, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(20, 17);
			this.label1.TabIndex = 1;
			this.label1.Text = "a:";
			// 
			// tbA
			// 
			this.tbA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbA.Location = new System.Drawing.Point(696, 12);
			this.tbA.Name = "tbA";
			this.tbA.Size = new System.Drawing.Size(91, 22);
			this.tbA.TabIndex = 2;
			this.tbA.Text = "50";
			// 
			// tbB
			// 
			this.tbB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbB.Location = new System.Drawing.Point(696, 40);
			this.tbB.Name = "tbB";
			this.tbB.Size = new System.Drawing.Size(91, 22);
			this.tbB.TabIndex = 4;
			this.tbB.Text = "50";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(675, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(20, 17);
			this.label2.TabIndex = 3;
			this.label2.Text = "b:";
			// 
			// tbDx
			// 
			this.tbDx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDx.Location = new System.Drawing.Point(696, 68);
			this.tbDx.Name = "tbDx";
			this.tbDx.Size = new System.Drawing.Size(91, 22);
			this.tbDx.TabIndex = 6;
			this.tbDx.Text = "0";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(675, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(26, 17);
			this.label3.TabIndex = 5;
			this.label3.Text = "dx:";
			// 
			// tbDy
			// 
			this.tbDy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDy.Location = new System.Drawing.Point(696, 96);
			this.tbDy.Name = "tbDy";
			this.tbDy.Size = new System.Drawing.Size(91, 22);
			this.tbDy.TabIndex = 8;
			this.tbDy.Text = "0";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(675, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(27, 17);
			this.label4.TabIndex = 7;
			this.label4.Text = "dy:";
			// 
			// btnRedraw
			// 
			this.btnRedraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRedraw.Location = new System.Drawing.Point(675, 132);
			this.btnRedraw.Name = "btnRedraw";
			this.btnRedraw.Size = new System.Drawing.Size(112, 33);
			this.btnRedraw.TabIndex = 9;
			this.btnRedraw.Text = "Перерисовать";
			this.btnRedraw.UseVisualStyleBackColor = true;
			this.btnRedraw.Click += new System.EventHandler(this.btnRedraw_Click);
			// 
			// pnPlot
			// 
			this.pnPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pnPlot.Location = new System.Drawing.Point(12, 12);
			this.pnPlot.Name = "pnPlot";
			this.pnPlot.Size = new System.Drawing.Size(657, 586);
			this.pnPlot.TabIndex = 10;
			this.pnPlot.TabStop = false;
			// 
			// Ellipse
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(799, 610);
			this.Controls.Add(this.pnPlot);
			this.Controls.Add(this.btnRedraw);
			this.Controls.Add(this.tbDy);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tbDx);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tbB);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbA);
			this.Controls.Add(this.label1);
			this.Name = "Ellipse";
			this.Text = "Ellipse";
			((System.ComponentModel.ISupportInitialize)(this.pnPlot)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbA;
		private System.Windows.Forms.TextBox tbB;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbDx;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbDy;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnRedraw;
		private System.Windows.Forms.PictureBox pnPlot;
	}
}

