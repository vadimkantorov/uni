using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Task1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			преобразованияToolStripMenuItem.Enabled = false;
			сохранитьToolStripMenuItem.Enabled = false;
		}

		private void pictureBox1_DragOver(object sender, DragEventArgs e)
		{
			MessageBox.Show("OK");
		}

		private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var openDialog = new OpenFileDialog();
			if (openDialog.ShowDialog() == DialogResult.OK)
			{
				pictureBox1.Image = originalImage = new Bitmap(openDialog.FileName);
				SetCurrentImage(new Bitmap(openDialog.FileName), true);
				undoStack.Clear();
				преобразованияToolStripMenuItem.Enabled = true;
				сохранитьToolStripMenuItem.Enabled = true;
			}
		}

		private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var saveDialog = new SaveFileDialog();
			if (saveDialog.ShowDialog() == DialogResult.OK)
				currentImage.Save(saveDialog.FileName);
		}

		private void отменаToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Undo();
		}

		private void yRGB3ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			const double oneThird = 1D/3;
			TransformImage(GetGrayscaleTransformer(oneThird, oneThird, oneThird));
		}

		private void цветовToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			Quantize(16);
		}

		private void цветаToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Quantize(4);
		}

		private void цветаToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			Quantize(2);
		}

		private void цветовToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Quantize(8);
		}

		private void включитьРазмываниеToolStripMenuItem_Click(object sender, EventArgs e)
		{
			enableDithering ^= true;
			((ToolStripMenuItem)sender).Text = enableDithering ? "Выключить размывание" : "Включить размывание";
		}

		private void пожатьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			JpegCompress(1, 1);
		}

		private void сжатьПроредивПоСхеме2h2vToolStripMenuItem_Click(object sender, EventArgs e)
		{
			JpegCompress(2, 2);
		}

		private void сжатьПроредивПоСхеме2h1vToolStripMenuItem_Click(object sender, EventArgs e)
		{
			JpegCompress(2, 1);
		}

		private void Quantize(int colorCount)
		{
			var ditherer = enableDithering ? (IDitherer) new FloydDitherer() : new DummyDitherer();
			SetCurrentImage(new Quantizer(currentImage, ditherer).Quantize(colorCount), true);
		}

		private void y0299R0587G0114BToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TransformImage(GetGrayscaleTransformer(0.299, 0.587, 0.114));
		}

		private void всплескнутьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
		{
			const int dwtSteps = 2;
			var p = new PipelineBuilder<Bitmap, IEnumerable<Matrix>>(new Bitmap2Matrices())
				.Append(new Dwt(dwtSteps))
				.Append(new WaveletDebugger());
			SetCurrentImage(p.BackAndForth(currentImage), true);
		}

		private void Undo()
		{
			if (undoStack.Count > 0)
				SetCurrentImage(undoStack.Pop(), false);
		}

		private void SetCurrentImage(Bitmap newImage, bool updateUndoStack)
		{
			if (updateUndoStack && currentImage != null)
			{
				undoStack.Push(currentImage);
				channelImages.Clear();
			}
			currentImage = newImage;
			pictureBox2.Image = newImage;
			UpdatePsnr();
		}

		private void TransformImage(Transformer f)
		{
			var newImage = new Bitmap(currentImage.Width, currentImage.Height);
			for (int x = 0; x < newImage.Width; x++)
				for (int y = 0; y < newImage.Height; y++)
				{
					var oldPixel = currentImage.GetPixel(x, y);
					newImage.SetPixel(x, y, f(oldPixel));
				}
			SetCurrentImage(newImage, true);
		}

		private Transformer GetGrayscaleTransformer(double rCoef, double gCoef, double bCoef)
		{
			return color => 
				{
					var luma = (int)(color.R * rCoef + color.G * gCoef + color.B * bCoef);
					return Color.FromArgb(luma, luma, luma);
				};
		}

		private void UpdatePsnr()
		{
			Func<double, double> sqr = x => x*x;
			double mse = 0.0;
			for (int x = 0; x < originalImage.Width; x++)
				for (int y = 0; y < currentImage.Height; y++)
				{
					var c1 = originalImage.GetPixel(x, y);
					var c2 = currentImage.GetPixel(x, y);
					Func<byte, byte, double> sqDiff = (a, b) => sqr(a - b);
					mse += sqDiff(c1.R, c2.R) + sqDiff(c1.G, c2.G) + sqDiff(c1.B, c2.B);
				}
			mse /= originalImage.Width*originalImage.Height*3;
			if(mse == 0)
			{
				lblPsnr.Text = "Картинки одинаковые!";
				return;
			}
			lblPsnr.Text = "PSNR = " + (10 * Math.Log10(sqr(255)/mse)).ToString("F3");
		}

		private void JpegCompress(int h, int v)
		{
			var pipeline = shortPipeline
				.Append<IEnumerable<Matrix>>(new Downsampler(h, v))
				.Append(new Dct())
				.Append(new JpegQuantizer(Gamma))
				.Append(new DummyInvertible<IEnumerable<Matrix>, IEnumerable<Matrix>>(
					x =>
					{
						var allCoefs = x.SelectMany(y => y);
						var nonZeroCoefs = allCoefs.Where(y => y != 0);
						double nonZeroCoefRatio = nonZeroCoefs.Count() / (double)allCoefs.Count();
						lblJpegDebug.Text = new StringBuilder()
							.AppendFormat("Доля ненулевых коэффициентов = {0:F3}\n", nonZeroCoefRatio)
							.AppendFormat("γ = {0:F3}", Gamma)
							.ToString();
						return x;
					},
					x => x));
			SetCurrentImage(pipeline.BackAndForth(currentImage), true);
		}
		
		private void SwitchToNextChannelImage(object sender, EventArgs e)
		{
			if (channelImages.Count == 0)
			{
				var pipeline = shortPipeline.Append(new ChannelDecomposer());
				channelImages.Add(currentImage);
				channelImages.AddRange(pipeline.Fwd(currentImage));
			}
			channelImages = channelImages.Skip(1).Concat(channelImages.Take(1)).ToList();
			SetCurrentImage(channelImages.First(), false);
		}

		private double Gamma
		{
			get
			{
				double quality = (double)udQuality.Value;
				return quality < 50 ? 50.0 / quality : (100 - quality) / 50.0;
			}
		}

		private readonly PipelineBuilder<Bitmap, IEnumerable<Matrix>> shortPipeline =
			new PipelineBuilder<Bitmap, IEnumerable<Matrix>>(new Bitmap2Matrices())
			.Append(new Rgb2YCrCb());
		private Bitmap originalImage;
		private Bitmap currentImage;
		private List<Bitmap> channelImages = new List<Bitmap>();
		private readonly Stack<Bitmap> undoStack = new Stack<Bitmap>();
		private bool enableDithering;

		private delegate Color Transformer(Color oldColor);
	}
}
