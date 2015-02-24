using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace BoozeMaster
{
	public class TrademarkRepository
	{
		public IList<string> AllTrademarkClasses { get; set; }

		public IList<ClassifiedTrademark> AllTrademarks { get; set; }

		public TrademarkRepository(string inputDirPath, IImagePreprocessor imagePreprocessor)
		{
			AllTrademarkClasses = new List<string>();
			AllTrademarks = new List<ClassifiedTrademark>();
			
			var inputDir = new DirectoryInfo(inputDirPath);
			var classes = inputDir
				.GetDirectories().Where(x => (x.Attributes & FileAttributes.Hidden) == 0)
				.Where(x => !x.Name.Contains("F"));

			foreach (var subdir in classes)
			{
				AllTrademarkClasses.Add(subdir.Name);

				var pics = subdir.GetFiles().Where(x => !x.Name.Contains("F"));
				foreach (var pic in pics)
				{
					var classified = new ClassifiedTrademark
					{
						Image = imagePreprocessor.Process(new Bitmap(pic.FullName)),
						TrademarkClass = AllTrademarkClasses.Count - 1,
						FilePath = pic.FullName
					};
					AllTrademarks.Add(classified);
				}
			}
		}
	}
}