using System.Linq;
using NUnit.Framework;

namespace IndexingService
{
	[TestFixture]
	public class MorphologyAnalyzer_Test
	{
		[Test]
		public void Test()
		{
			var ma = new MorphologyAnalyzer("192.168.0.102");
			var p = ma.Analyze("������");
			Assert.AreEqual(2, p.Length);
			CollectionAssert.AreEqual(new[]{"����", "�����"}, p.Select(x => x.Lemma).OrderBy(x => x).ToArray());
		}
	}
}