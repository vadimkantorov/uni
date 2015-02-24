using System;
using System.IO;
using System.Resources;

namespace Okulobot
{
	class Program
    {
		private IOUtils io;

		static void Main(string[] args)
        {
			new Program(Console.In, Console.Out).Start();
		}

		private Program(TextReader input, TextWriter output)
		{
			io = new IOUtils(input, output);
		}

		private void Start()
		{
			var fighterFactory = new FighterFactory();

			var initData = io.ScanInitData();

			var state = new State(initData);
			Fighter fighter = fighterFactory.Create(initData);

			io.PrintNewbornFighter(fighter);

			//if(initData.Number == 0 && initData.Team == 1)
			  //  System.Diagnostics.Debugger.Break();

			Reality reality;
            while((reality = io.ScanReality(fighter, initData.Team == 1)) != null )
			{
				fighter.Log.Write("Получены координаты: " + fighter.Coordinates + "; Угол: " + fighter.Angle.ToDegrees());

				state.Update(reality);
                var move = fighter.MakeMove(state);
				if (move != null)
				{
					state.AfterNewMove();

					io.PrintNewMove(move, initData.Team == 1);
				}
			}
		}
	}
}
