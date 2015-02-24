using System;

namespace Task1
{
	public class DummyInvertible<TIn, TOut> : IInvertible<TIn, TOut>
	{
		public DummyInvertible(Func<TIn, TOut> fwd, Func<TOut, TIn> back)
		{
			this.fwd = fwd;
			this.back = back;
		}

		public TOut Fwd(TIn value)
		{
			return fwd(value);
		}

		public TIn Back(TOut value)
		{
			return back(value);
		}

		readonly Func<TIn, TOut> fwd;
		readonly Func<TOut, TIn> back;
	}
}