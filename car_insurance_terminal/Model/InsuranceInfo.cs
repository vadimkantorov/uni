namespace Insurance
{
	public class InsuranceInfo
	{
		public PersonalDetails PersonalDetails { get; private set; }

		public CarDetails CarDetails { get; private set; }

		public InsuranceDetails InsuranceDetails { get; private set; }

		public InsuranceInfo()
		{
			PersonalDetails = new PersonalDetails();
			CarDetails = new CarDetails();
			InsuranceDetails = new InsuranceDetails();
		}
	}
}