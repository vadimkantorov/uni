using System;

namespace Insurance
{
	public interface IController
	{
		void UpdateInsuranceInfo();

		Type GetNextControllerType();
		
		void Start(InsuranceInfo info);
		ScreenResult Result { get; }
	}
}