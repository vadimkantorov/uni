using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace theGame
{
    /// <summary>
    /// Игрок
    /// </summary>
	class Fighter
	{
        public int x { get; private set; }              // Текущие координаты игрока
		public int y { get; private set; }
        public int team { get; private set; }           // Команда
        public char clazz { get; private set; }         // Класс игрока: Pistol, Minigun, Sniper Rifle, Rocket Launcher, Fatman 
        public int health { get; private set; }         // Количество очков здоровья
        public int viewAngle { get; private set; }      // Угол обзора
        public int viewDistance { get; private set; }   // Дальность видимости и поражения оружием
        public int energy { get; private set; }         // Очки энергии
        public int shotCost { get; private set; }       // "Стоимость" выстрела в очках энергии
        public int demage { get; private set; }         // Повреждение (количество хитпоинтов, снятых у пострадавших от оружия)
        public int demageRadius { get; private set; }   // Радиус поражения (количество пораженных клеток вокруг точки, куда был произведен выстрел)
        public int shotPrecision { get; private set; }  // Точность выстрела
        public int speed { get; private set; }          // Скорость передвижения

        public Fighter(int tx, int ty, int tteam, char tclazz, int thealth) : this(tteam, tclazz) {
            this.health = thealth;
            this.x = tx;
            this.y = ty;
        }

        public Fighter(int team, char clazz)
		{
			this.team = team;
			this.clazz = clazz;
            /*
             * Хитпоинтов - 100 
             * Очков энергии - 20 
             */
            this.health = 100;
            this.energy = 20;
            int[] P = { 15, 70, 50, 1, 3 };
            int[] M = { 12, 40, 100, 1, 2 };
            int[] S = { 10, 100, 90, 1, 5 };
            int[] R = { 10, 100, 90, 5, 10 };
            int[] F = { 10, 100, 100, 10, 20 };
            Dictionary<char, int[]> fighterTypes = new Dictionary<char, int[]>();
            fighterTypes.Add('P', P);
            fighterTypes.Add('M', M);
            fighterTypes.Add('S', S);
            fighterTypes.Add('R', R);
            fighterTypes.Add('F', F);
            this.speed = fighterTypes[clazz][0];
            this.demage = fighterTypes[clazz][1];
            this.shotPrecision = fighterTypes[clazz][2];
            this.demageRadius = fighterTypes[clazz][3];
            this.shotCost = fighterTypes[clazz][4];
		}

        /// <summary>
        /// Отражаем координаты и угол для правой команды
        /// </summary>
        private void Reflect()
        {
            if (this.team == 1)
            {
                this.x = 199 - this.x;
                this.viewAngle = 180 - this.viewAngle;

            }
        }

        /// <summary>
        /// приниает состояние игры в виде адаптора
        /// возвращает набор действий (изменяя тот же адаптор)
        /// </summary>
        public void go(Adaptor adaptor) {
            this.viewAngle = adaptor.angle;
            this.x = adaptor.x;
            this.y = adaptor.y;
            //adaptor.fireTo();
            Reflect();
            this.x++;
            this.y--;
            this.viewAngle = (this.viewAngle + 10) % 360;
            Reflect();
            adaptor.moveTo(this.x,this.y);
            adaptor.turnTo(this.viewAngle);
            adaptor.say = "EnTaraAdun";
        }
	}
}
