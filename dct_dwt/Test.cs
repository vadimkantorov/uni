using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Task1
{
class P
{
	static void Main(string[] args)
	{
		var bmp = new Bitmap(args[0]);
		new Quantizer(bmp).Quantize(16);
	}
}
}