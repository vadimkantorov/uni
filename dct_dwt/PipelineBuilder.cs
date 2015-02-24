namespace Task1
{
	public class PipelineBuilder<TIn, TOut>
	{
		public PipelineBuilder(IInvertible<TIn, TOut> initialPipeline)
		{
			currentPipeline = initialPipeline;
		}

		public TIn BackAndForth(TIn start)
		{
			return currentPipeline.Back(currentPipeline.Fwd(start));
		}
		
		public TOut Fwd(TIn start)
		{
			return currentPipeline.Fwd(start);
		}

		public PipelineBuilder<TIn, TNewOut> Append<TNewOut>(IInvertible<TOut, TNewOut> newElement)
		{
			return new PipelineBuilder<TIn, TNewOut>(
				new DummyInvertible<TIn, TNewOut>(
					input => newElement.Fwd(currentPipeline.Fwd(input)),
					output => currentPipeline.Back(newElement.Back(output))));
		}

		readonly IInvertible<TIn, TOut> currentPipeline;
	}
}