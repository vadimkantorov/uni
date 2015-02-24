namespace ImgComp
{
	partial class MainForm
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
			this.pbLeft = new System.Windows.Forms.PictureBox();
			this.pbRight = new System.Windows.Forms.PictureBox();
			this.ofd = new System.Windows.Forms.OpenFileDialog();
			this.sfd = new System.Windows.Forms.SaveFileDialog();
			this.btnPsnr = new System.Windows.Forms.Button();
			this.btnSmartBw = new System.Windows.Forms.Button();
			this.btnDumbBw = new System.Windows.Forms.Button();
			this.btnBackAndForth888 = new System.Windows.Forms.Button();
			this.btnBackAndForth855 = new System.Windows.Forms.Button();
			this.btnBackAndForth388 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pbLeft)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbRight)).BeginInit();
			this.SuspendLayout();
			// 
			// pbLeft
			// 
			this.pbLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbLeft.Location = new System.Drawing.Point(12, 12);
			this.pbLeft.Name = "pbLeft";
			this.pbLeft.Size = new System.Drawing.Size(512, 512);
			this.pbLeft.TabIndex = 0;
			this.pbLeft.TabStop = false;
			this.pbLeft.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MouseClick);
			// 
			// pbRight
			// 
			this.pbRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbRight.Location = new System.Drawing.Point(547, 12);
			this.pbRight.Name = "pbRight";
			this.pbRight.Size = new System.Drawing.Size(512, 512);
			this.pbRight.TabIndex = 1;
			this.pbRight.TabStop = false;
			this.pbRight.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MouseClick);
			// 
			// ofd
			// 
			this.ofd.FileName = "image_Lena512rgb.png";
			this.ofd.Filter = "Несжатые изображения|*.png;*.bmp";
			// 
			// sfd
			// 
			this.sfd.FileName = "transformed.png";
			this.sfd.Filter = "Несжатые изображения|*.png; *.bmp";
			// 
			// btnPsnr
			// 
			this.btnPsnr.Location = new System.Drawing.Point(12, 549);
			this.btnPsnr.Name = "btnPsnr";
			this.btnPsnr.Size = new System.Drawing.Size(89, 45);
			this.btnPsnr.TabIndex = 2;
			this.btnPsnr.Text = "PSNR";
			this.btnPsnr.UseVisualStyleBackColor = true;
			this.btnPsnr.Click += new System.EventHandler(this.btnPsnr_Click);
			// 
			// btnSmartBw
			// 
			this.btnSmartBw.Location = new System.Drawing.Point(959, 549);
			this.btnSmartBw.Name = "btnSmartBw";
			this.btnSmartBw.Size = new System.Drawing.Size(100, 44);
			this.btnSmartBw.TabIndex = 3;
			this.btnSmartBw.Text = "Умное ЧБ";
			this.btnSmartBw.UseVisualStyleBackColor = true;
			this.btnSmartBw.Click += new System.EventHandler(this.btnSmartBw_Click);
			// 
			// btnDumbBw
			// 
			this.btnDumbBw.Location = new System.Drawing.Point(853, 549);
			this.btnDumbBw.Name = "btnDumbBw";
			this.btnDumbBw.Size = new System.Drawing.Size(100, 44);
			this.btnDumbBw.TabIndex = 4;
			this.btnDumbBw.Text = "Тупое ЧБ";
			this.btnDumbBw.UseVisualStyleBackColor = true;
			this.btnDumbBw.Click += new System.EventHandler(this.btnDumbBw_Click);
			// 
			// btnBackAndForth888
			// 
			this.btnBackAndForth888.Location = new System.Drawing.Point(747, 549);
			this.btnBackAndForth888.Name = "btnBackAndForth888";
			this.btnBackAndForth888.Size = new System.Drawing.Size(100, 44);
			this.btnBackAndForth888.TabIndex = 6;
			this.btnBackAndForth888.Text = "Туда-сюда";
			this.btnBackAndForth888.UseVisualStyleBackColor = true;
			this.btnBackAndForth888.Click += new System.EventHandler(this.btnBackAndForth888_Click);
			// 
			// btnBackAndForth855
			// 
			this.btnBackAndForth855.Location = new System.Drawing.Point(641, 549);
			this.btnBackAndForth855.Name = "btnBackAndForth855";
			this.btnBackAndForth855.Size = new System.Drawing.Size(100, 44);
			this.btnBackAndForth855.TabIndex = 7;
			this.btnBackAndForth855.Text = "Туда-сюда 8-5-5";
			this.btnBackAndForth855.UseVisualStyleBackColor = true;
			this.btnBackAndForth855.Click += new System.EventHandler(this.btnBackAndForth855_Click);
			// 
			// btnBackAndForth388
			// 
			this.btnBackAndForth388.Location = new System.Drawing.Point(547, 549);
			this.btnBackAndForth388.Name = "btnBackAndForth388";
			this.btnBackAndForth388.Size = new System.Drawing.Size(88, 44);
			this.btnBackAndForth388.TabIndex = 8;
			this.btnBackAndForth388.Text = "Туда-сюда 3-8-8";
			this.btnBackAndForth388.UseVisualStyleBackColor = true;
			this.btnBackAndForth388.Click += new System.EventHandler(this.btnBackAndForth388_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1072, 607);
			this.Controls.Add(this.btnBackAndForth388);
			this.Controls.Add(this.btnBackAndForth855);
			this.Controls.Add(this.btnBackAndForth888);
			this.Controls.Add(this.btnDumbBw);
			this.Controls.Add(this.btnSmartBw);
			this.Controls.Add(this.btnPsnr);
			this.Controls.Add(this.pbRight);
			this.Controls.Add(this.pbLeft);
			this.Name = "MainForm";
			this.Text = "MainForm";
			((System.ComponentModel.ISupportInitialize)(this.pbLeft)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbRight)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pbLeft;
		private System.Windows.Forms.PictureBox pbRight;
		private System.Windows.Forms.OpenFileDialog ofd;
		private System.Windows.Forms.SaveFileDialog sfd;
		private System.Windows.Forms.Button btnPsnr;
		private System.Windows.Forms.Button btnSmartBw;
		private System.Windows.Forms.Button btnDumbBw;
		private System.Windows.Forms.Button btnBackAndForth888;
		private System.Windows.Forms.Button btnBackAndForth855;
		private System.Windows.Forms.Button btnBackAndForth388;
	}
}

