namespace Task1
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.lblPsnr = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.udQuality = new System.Windows.Forms.NumericUpDown();
			this.lblJpegDebug = new System.Windows.Forms.Label();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.открытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.сохранитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.преобразованияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.вЧёрнобелоеИзображениеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.yRGB3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.y0299R0587G0114BToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.уменьшениеКоличестваЦветовразрезаниемПоМедианеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.цветаToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.цветаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.цветовToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.цветовToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.включитьРазмываниеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.пожатьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.сжатьПроредивПоСхеме2h2vToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.сжатьПроредивПоСхеме2h1vToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.отменаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.всплескнутьИзображениеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.udQuality)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.lblPsnr, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.udQuality, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.lblJpegDebug, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 28);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(812, 495);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// lblPsnr
			// 
			this.lblPsnr.AutoSize = true;
			this.lblPsnr.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblPsnr.Location = new System.Drawing.Point(4, 0);
			this.lblPsnr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblPsnr.Name = "lblPsnr";
			this.lblPsnr.Size = new System.Drawing.Size(243, 46);
			this.lblPsnr.TabIndex = 0;
			this.lblPsnr.Text = "PSNR = ???";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(4, 50);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(274, 389);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.DragOver += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragOver);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox2.Location = new System.Drawing.Point(286, 50);
			this.pictureBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(522, 389);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox2.TabIndex = 2;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Click += new System.EventHandler(this.SwitchToNextChannelImage);
			// 
			// udQuality
			// 
			this.udQuality.Location = new System.Drawing.Point(286, 4);
			this.udQuality.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.udQuality.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.udQuality.Name = "udQuality";
			this.udQuality.Size = new System.Drawing.Size(160, 22);
			this.udQuality.TabIndex = 3;
			this.udQuality.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// lblJpegDebug
			// 
			this.lblJpegDebug.AutoSize = true;
			this.lblJpegDebug.Location = new System.Drawing.Point(4, 443);
			this.lblJpegDebug.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblJpegDebug.Name = "lblJpegDebug";
			this.lblJpegDebug.Size = new System.Drawing.Size(0, 17);
			this.lblJpegDebug.TabIndex = 4;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.преобразованияToolStripMenuItem,
            this.отменаToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(812, 28);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// файлToolStripMenuItem
			// 
			this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.открытьToolStripMenuItem,
            this.сохранитьToolStripMenuItem});
			this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
			this.файлToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
			this.файлToolStripMenuItem.Text = "Файл";
			// 
			// открытьToolStripMenuItem
			// 
			this.открытьToolStripMenuItem.Name = "открытьToolStripMenuItem";
			this.открытьToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
			this.открытьToolStripMenuItem.Text = "Открыть";
			this.открытьToolStripMenuItem.Click += new System.EventHandler(this.открытьToolStripMenuItem_Click);
			// 
			// сохранитьToolStripMenuItem
			// 
			this.сохранитьToolStripMenuItem.Name = "сохранитьToolStripMenuItem";
			this.сохранитьToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
			this.сохранитьToolStripMenuItem.Text = "Сохранить";
			this.сохранитьToolStripMenuItem.Click += new System.EventHandler(this.сохранитьToolStripMenuItem_Click);
			// 
			// преобразованияToolStripMenuItem
			// 
			this.преобразованияToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.вЧёрнобелоеИзображениеToolStripMenuItem,
            this.уменьшениеКоличестваЦветовразрезаниемПоМедианеToolStripMenuItem,
            this.пожатьToolStripMenuItem,
            this.сжатьПроредивПоСхеме2h2vToolStripMenuItem,
            this.сжатьПроредивПоСхеме2h1vToolStripMenuItem,
            this.всплескнутьИзображениеToolStripMenuItem});
			this.преобразованияToolStripMenuItem.Name = "преобразованияToolStripMenuItem";
			this.преобразованияToolStripMenuItem.Size = new System.Drawing.Size(142, 24);
			this.преобразованияToolStripMenuItem.Text = "Преобразования";
			// 
			// вЧёрнобелоеИзображениеToolStripMenuItem
			// 
			this.вЧёрнобелоеИзображениеToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.yRGB3ToolStripMenuItem,
            this.y0299R0587G0114BToolStripMenuItem});
			this.вЧёрнобелоеИзображениеToolStripMenuItem.Name = "вЧёрнобелоеИзображениеToolStripMenuItem";
			this.вЧёрнобелоеИзображениеToolStripMenuItem.Size = new System.Drawing.Size(497, 24);
			this.вЧёрнобелоеИзображениеToolStripMenuItem.Text = "В чёрно-белое изображение";
			// 
			// yRGB3ToolStripMenuItem
			// 
			this.yRGB3ToolStripMenuItem.Name = "yRGB3ToolStripMenuItem";
			this.yRGB3ToolStripMenuItem.Size = new System.Drawing.Size(291, 24);
			this.yRGB3ToolStripMenuItem.Text = "Y = (R + G + B)/3";
			this.yRGB3ToolStripMenuItem.Click += new System.EventHandler(this.yRGB3ToolStripMenuItem_Click);
			// 
			// y0299R0587G0114BToolStripMenuItem
			// 
			this.y0299R0587G0114BToolStripMenuItem.Name = "y0299R0587G0114BToolStripMenuItem";
			this.y0299R0587G0114BToolStripMenuItem.Size = new System.Drawing.Size(291, 24);
			this.y0299R0587G0114BToolStripMenuItem.Text = "Y = 0.299*R + 0.587*G + 0.114*B";
			this.y0299R0587G0114BToolStripMenuItem.Click += new System.EventHandler(this.y0299R0587G0114BToolStripMenuItem_Click);
			// 
			// уменьшениеКоличестваЦветовразрезаниемПоМедианеToolStripMenuItem
			// 
			this.уменьшениеКоличестваЦветовразрезаниемПоМедианеToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.цветаToolStripMenuItem1,
            this.цветаToolStripMenuItem,
            this.цветовToolStripMenuItem,
            this.цветовToolStripMenuItem1,
            this.включитьРазмываниеToolStripMenuItem});
			this.уменьшениеКоличестваЦветовразрезаниемПоМедианеToolStripMenuItem.Name = "уменьшениеКоличестваЦветовразрезаниемПоМедианеToolStripMenuItem";
			this.уменьшениеКоличестваЦветовразрезаниемПоМедианеToolStripMenuItem.Size = new System.Drawing.Size(497, 24);
			this.уменьшениеКоличестваЦветовразрезаниемПоМедианеToolStripMenuItem.Text = "Уменьшение количества цветов (разрезанием по медиане)";
			// 
			// цветаToolStripMenuItem1
			// 
			this.цветаToolStripMenuItem1.Name = "цветаToolStripMenuItem1";
			this.цветаToolStripMenuItem1.Size = new System.Drawing.Size(237, 24);
			this.цветаToolStripMenuItem1.Text = "2 цвета";
			this.цветаToolStripMenuItem1.Click += new System.EventHandler(this.цветаToolStripMenuItem1_Click);
			// 
			// цветаToolStripMenuItem
			// 
			this.цветаToolStripMenuItem.Name = "цветаToolStripMenuItem";
			this.цветаToolStripMenuItem.Size = new System.Drawing.Size(237, 24);
			this.цветаToolStripMenuItem.Text = "4 цвета";
			this.цветаToolStripMenuItem.Click += new System.EventHandler(this.цветаToolStripMenuItem_Click);
			// 
			// цветовToolStripMenuItem
			// 
			this.цветовToolStripMenuItem.Name = "цветовToolStripMenuItem";
			this.цветовToolStripMenuItem.Size = new System.Drawing.Size(237, 24);
			this.цветовToolStripMenuItem.Text = "8 цветов";
			this.цветовToolStripMenuItem.Click += new System.EventHandler(this.цветовToolStripMenuItem_Click);
			// 
			// цветовToolStripMenuItem1
			// 
			this.цветовToolStripMenuItem1.Name = "цветовToolStripMenuItem1";
			this.цветовToolStripMenuItem1.Size = new System.Drawing.Size(237, 24);
			this.цветовToolStripMenuItem1.Text = "16 цветов";
			this.цветовToolStripMenuItem1.Click += new System.EventHandler(this.цветовToolStripMenuItem1_Click);
			// 
			// включитьРазмываниеToolStripMenuItem
			// 
			this.включитьРазмываниеToolStripMenuItem.Name = "включитьРазмываниеToolStripMenuItem";
			this.включитьРазмываниеToolStripMenuItem.Size = new System.Drawing.Size(237, 24);
			this.включитьРазмываниеToolStripMenuItem.Text = "Включить размывание";
			this.включитьРазмываниеToolStripMenuItem.Click += new System.EventHandler(this.включитьРазмываниеToolStripMenuItem_Click);
			// 
			// пожатьToolStripMenuItem
			// 
			this.пожатьToolStripMenuItem.Name = "пожатьToolStripMenuItem";
			this.пожатьToolStripMenuItem.Size = new System.Drawing.Size(497, 24);
			this.пожатьToolStripMenuItem.Text = "Сжать с потерями (DCT + квантизация)";
			this.пожатьToolStripMenuItem.Click += new System.EventHandler(this.пожатьToolStripMenuItem_Click);
			// 
			// сжатьПроредивПоСхеме2h2vToolStripMenuItem
			// 
			this.сжатьПроредивПоСхеме2h2vToolStripMenuItem.Name = "сжатьПроредивПоСхеме2h2vToolStripMenuItem";
			this.сжатьПроредивПоСхеме2h2vToolStripMenuItem.Size = new System.Drawing.Size(497, 24);
			this.сжатьПроредивПоСхеме2h2vToolStripMenuItem.Text = "Сжать, проредив по схеме 2h2v";
			this.сжатьПроредивПоСхеме2h2vToolStripMenuItem.Click += new System.EventHandler(this.сжатьПроредивПоСхеме2h2vToolStripMenuItem_Click);
			// 
			// сжатьПроредивПоСхеме2h1vToolStripMenuItem
			// 
			this.сжатьПроредивПоСхеме2h1vToolStripMenuItem.Name = "сжатьПроредивПоСхеме2h1vToolStripMenuItem";
			this.сжатьПроредивПоСхеме2h1vToolStripMenuItem.Size = new System.Drawing.Size(497, 24);
			this.сжатьПроредивПоСхеме2h1vToolStripMenuItem.Text = "Сжать, проредив по схеме 2h1v";
			this.сжатьПроредивПоСхеме2h1vToolStripMenuItem.Click += new System.EventHandler(this.сжатьПроредивПоСхеме2h1vToolStripMenuItem_Click);
			// 
			// отменаToolStripMenuItem
			// 
			this.отменаToolStripMenuItem.Name = "отменаToolStripMenuItem";
			this.отменаToolStripMenuItem.Size = new System.Drawing.Size(74, 24);
			this.отменаToolStripMenuItem.Text = "Отмена";
			this.отменаToolStripMenuItem.Click += new System.EventHandler(this.отменаToolStripMenuItem_Click);
			// 
			// всплескнутьИзображениеToolStripMenuItem
			// 
			this.всплескнутьИзображениеToolStripMenuItem.Name = "всплескнутьИзображениеToolStripMenuItem";
			this.всплескнутьИзображениеToolStripMenuItem.Size = new System.Drawing.Size(497, 24);
			this.всплескнутьИзображениеToolStripMenuItem.Text = "Всплескнуть изображение";
			this.всплескнутьИзображениеToolStripMenuItem.Click += new System.EventHandler(this.всплескнутьИзображениеToolStripMenuItem_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(812, 523);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "Form1";
			this.Text = "Task 1";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.udQuality)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblPsnr;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem открытьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сохранитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem преобразованияToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem вЧёрнобелоеИзображениеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yRGB3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem y0299R0587G0114BToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem отменаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem уменьшениеКоличестваЦветовразрезаниемПоМедианеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem цветовToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem включитьРазмываниеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem цветаToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem цветаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem цветовToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem пожатьToolStripMenuItem;
		private System.Windows.Forms.NumericUpDown udQuality;
		private System.Windows.Forms.Label lblJpegDebug;
		private System.Windows.Forms.ToolStripMenuItem сжатьПроредивПоСхеме2h2vToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem сжатьПроредивПоСхеме2h1vToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem всплескнутьИзображениеToolStripMenuItem;
    }
}

