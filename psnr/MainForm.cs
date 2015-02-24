using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImgComp
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			transforms = new ImageTransforms();
		}

		void MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				LoadImage((PictureBox) sender);
			else
				SaveImage((PictureBox)sender);
		}

		void LoadImage(PictureBox pb)
		{
			if (ofd.ShowDialog() == DialogResult.OK)
				pb.Image = new Bitmap(ofd.FileName);
		}

		void SaveImage(PictureBox pb)
		{
			if(sfd.ShowDialog() == DialogResult.OK)
				pb.Image.Save(sfd.FileName);
		}

		private void btnPsnr_Click(object sender, EventArgs e)
		{
			btnPsnr.Text = string.Format("PSNR:{0}{1:F2}", Environment.NewLine,
			                             transforms.Psnr((Bitmap) pbLeft.Image, (Bitmap) pbRight.Image));
		}

		void btnDumbBw_Click(object sender, EventArgs e)
		{
			pbRight.Image = transforms.ToBlackAndWhite((Bitmap) pbRight.Image, true);
		}

		void btnSmartBw_Click(object sender, EventArgs e)
		{
			pbRight.Image = transforms.ToBlackAndWhite((Bitmap)pbRight.Image, false);
		}

		void btnBackAndForth888_Click(object sender, EventArgs e)
		{
			pbRight.Image = transforms.ToYuvAndBack((Bitmap)pbRight.Image,8, 5);
		}

		void btnBackAndForth855_Click(object sender, EventArgs e)
		{
			pbRight.Image = transforms.ToYuvAndBack((Bitmap)pbRight.Image, 8,5);
		}

		void btnBackAndForth388_Click(object sender, EventArgs e)
		{
			pbRight.Image = transforms.ToYuvAndBack((Bitmap)pbRight.Image, 3, 8);
		}

		readonly ImageTransforms transforms;
	}
}
