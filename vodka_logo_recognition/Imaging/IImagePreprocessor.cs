using System;
using System.Drawing;

namespace BoozeMaster
{
	public interface IImagePreprocessor
	{
		Bitmap Process(Bitmap bmp);
	}
}