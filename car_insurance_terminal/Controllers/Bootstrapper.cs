using System;
using Insurance.Controllers;

namespace Insurance
{
	public class Bootstrapper
	{
		readonly Type firstScreenType = typeof(LastNameController);
		IController current;
		InsuranceInfo info;

		public void Start()
		{
			Reset();
			DispatchCycle();
		}

		public void Reset()
		{
			info = new InsuranceInfo();
			MoveOn(firstScreenType);
		}

		void DispatchCycle()
		{
			while(true)
			{
				current.Start(info);
				switch (current.Result)
				{
					case ScreenResult.Proceed:
						current.UpdateInsuranceInfo();
						var next = current.GetNextControllerType();

						if (next != null)
							MoveOn(next);
						else
							Reset();
						break;
					case ScreenResult.Cancel:
						Reset();
						break;
					case ScreenResult.Close:
						return;
				}
			}
		}

		void MoveOn(Type next)
		{
			current = (IController)Activator.CreateInstance(next);
		}
	}
}