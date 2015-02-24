using System;

namespace Okulobot
{
	public class UnitProperties
	{
		public UnitProperties(char type)
		{
			TypeName = type;
			switch(type)
			{
				case 'P':
					Speed = 15;
					Damage = 70;
					Accuracy = 50;
					DamageRadius = 0;
					ShotCost = 3;
					break;
				case 'M':
					Speed = 12;
					Damage = 40;
					Accuracy = 100;
					DamageRadius = 1;
					ShotCost = 2;
					break;
				case 'S':
					Speed = 10;
					Damage = 100;
					Accuracy = 90;
					DamageRadius = 0;
					ShotCost = 5;
					break;
				case 'R':
					Speed = 10;
					Damage = 100;
					Accuracy = 90;
					DamageRadius = 5;
					ShotCost = 10;
					break;
				case 'F':
					Speed = 10;
					Damage = 100;
					Accuracy = 100;
					DamageRadius = 10;
					ShotCost = 20;
					break;
			}
			if(type == 'S')
			{
				SightAngle = Math.PI/12;
				SightRadius = 100;
			}
			else
			{
				SightAngle = Math.PI/4;
				SightRadius = 50;
			}
		}
		
		public char TypeName { get; set; }

		public int SightRadius { get; set; }

		public double SightAngle { get; set; }

		public int Speed { get; set; }

		public int ShotCost { get; set; }

		public int Damage { get; set; }

		public int DamageRadius { get; set; }

		public int Accuracy { get; set; }
	}
}