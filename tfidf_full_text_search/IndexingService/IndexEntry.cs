namespace IndexingService
{
	public class IndexEntry
	{
		public float IDF { get; set; }
		public int[] PostingIds { get; set; }
		public float[] TFs { get; set; }
	}
}