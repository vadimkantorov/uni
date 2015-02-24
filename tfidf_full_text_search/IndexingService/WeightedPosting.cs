namespace IndexingService
{
	public class WeightedPosting
	{
		public int PostingId { get; set; }
		public int ExactWordMatchCount { get; set; }
		public float[] TFs { get; set; }
		public float[] IDFs { get; set; }
	}
}