using System.Drawing;
using AForge.Imaging.Filters;

namespace BoozeMaster
{
	public class AForgeImagePreprocessor : IImagePreprocessor
	{
		public AForgeImagePreprocessor( params IFilter[] filters)
		{
			filterSeq = filters.Length > 0 ? new FiltersSequence(filters) : null;
		}

		public Bitmap Process(Bitmap bmp)
		{
			return filterSeq != null ? filterSeq.Apply(bmp) : bmp;
		}

		readonly FiltersSequence filterSeq;
	}
}