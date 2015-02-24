namespace Task1
{
	public interface IInvertible<TIn, TOut>
	{
		TOut Fwd(TIn input);
		TIn Back(TOut output);
	}
}