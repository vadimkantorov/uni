using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace theGame
{
	class Adaptor
	{
		public static string version = "0.1";

		private TextReader input;								// Поток входящих сообщений
		private TextWriter output;								// Поток исходящих сообщений


		// Постоянные состояния
		public int team { get; private set; }					// Номер комманды
		public int number { get; private set; }					// Номер игрока в комманде

		
		// Состояние получаемое со "входа"
		public char clazz { get; private set; }					// Класс(тип) игрока
		public bool alive { get; private set; }					// Живой ли я
		public int angle { get; private set; }					// Угол обзора
		public int health { get; private set; }					// Текущие очки жизни
		public int energy { get; private set; }					// Текущие очки энергии

		public int unitsNum { get; private set; }				// Количество видимых юнитов
		public List<Unit> frendlyUnits { get; private set; }	// Список дружественных юнитов
		public List<Unit> enemyUnits { get; private set; }		// Список вражеских юнитов


		public int friends { get; private set; }				// Юнитов в комманде
		public List<String> messages;							// Сообщения от других членов комманды

		public int x;											// Координата X
		public int y;											// Координата Y


		// Состояния на "выход"(то что отдаём потом серверу)
		public bool tofire { get; set; }						// Будем ли стрелять
		public int fireX { get; private set; }					// Координата стрельбы X
		public int fireY { get; private set; }					// Координата стрельбы Y
		
		
		public int moveX { get; set; }							// Координата для передвижения по X
		public int moveY { get; set; }							// Координата для передвижения по Y
		public int angleNew { get; private set; }				// Новый угол
		public string say { get; set; }							// Сообщение остальным игрокам комманды

				


		/// <summary>
		/// Создаёт экземпляр класса адаптора входящих и исходящих потоков сообщений, 
		/// а так же проводит первичную инициализацию - получает номер комманды  номер игрока. 
		/// </summary>
		/// <param name="input">Поток входящих сообщений</param>
		/// <param name="output">Поток исходящих сообщений</param>
		public Adaptor(TextReader input, TextWriter output)
		{
			this.input = input;
			this.output = output;

			team = int.Parse(input.ReadLine().Trim());
			number = int.Parse(input.ReadLine().Trim());
		}

		/// <summary>
		/// Выстрелить в точку x, y
		/// </summary>
		/// <param name="x">Координата X</param>
		/// <param name="y">Координата Y</param>
		public void fireTo(int x, int y)
		{
			tofire = true;
			fireX = x;
			fireY = y;
		}

		/// <summary>
		/// Выбираем класс, которым мы играем и появляемся в игре.
		/// </summary>
		/// <param name="clazz">Класс персонажа</param>
		public void init(char clazz)
		{
			this.clazz = clazz;
			output.WriteLine(clazz.ToString());
			getData();
		}

		/// <summary>
		/// Перейти в точку x, y
		/// </summary>
		/// <param name="x">Координата X</param>
		/// <param name="y">Координата Y</param>
		public void moveTo(int x, int y)
		{
			moveX = x;
			moveY = y;
		}
		/// <summary>
		/// Новый угол, куда собираемся смотреть
		/// </summary>
		/// <param name="angle">Угол в градусах</param>
		public void turnTo(int angle)
		{
			angleNew = angle;
		}

		/// <summary>
		/// Производим ход, относительно текущего состояния и получаем новое состояние.
		/// </summary>
		public void step()
		{
			if (this.tofire)
			{
                // нужно ли провести выстрел (0 - нет, 1 - да)
				output.WriteLine("1");
			}
			else
			{
				output.WriteLine("0");
			}
            // [Если нужно выстрелить, то X-координата выстрела. Если нет, любое целое число.]
			output.WriteLine(fireX.ToString());
			output.WriteLine(fireY.ToString());
            // [Новая координата X, куда нужно передвинутся]
            output.WriteLine(moveX.ToString());
			output.WriteLine(moveY.ToString());
            // [Новый угол обзора, в градусах, целое число от 0 до 360]
			output.WriteLine(angleNew.ToString());
            // [Строка, что сказать в эфир]
            output.WriteLine(say);

			getData();
		}
		

		/// <summary>
		/// Получить данные из потока ввода и принять соответствующее состояние
		/// </summary>
		private void getData()
		{
			var tmp = input.ReadLine().Trim();
            // строка синхронизации Begin
            if (!tmp.Equals("Begin"))             
			{
				throw new GameParseException("Synchronization fail");
			}
			tmp = input.ReadLine().Trim();
            // [Статус: 1 - живой или 0 - мертвый.]
			if (tmp.Equals("1"))
			{
				alive = true;
			}
			else if (tmp.Equals("0"))
			{
                // Если игрок мертв, то все последующее не передается, и ответ от игрока не ожидается.]
				alive = false;
				getData();
				return;
			}
			else
			{
				throw new GameParseException("Protocol missmatch: not 1 or 0 in alive status");
			}
			try
			{
                // координаты, текущие угол обзора, очки здоровья и энергии 
				x = int.Parse(input.ReadLine().Trim()); 
				y = int.Parse(input.ReadLine().Trim());
				angle = int.Parse(input.ReadLine().Trim()); 
				health = int.Parse(input.ReadLine().Trim());
				energy = int.Parse(input.ReadLine().Trim());
                // юнитов видите
				unitsNum = int.Parse(input.ReadLine().Trim());
				frendlyUnits = new List<Unit>();
				enemyUnits = new List<Unit>();
				for (int i = 0; i < unitsNum; i++)
				{
                    // для каждого координаты
					var tx = int.Parse(input.ReadLine().Trim());
					var ty = int.Parse(input.ReadLine().Trim());
                    // команда (0 - ваша, 1 - чужая)
					var tteam = int.Parse(input.ReadLine().Trim());
					var tclazz = char.Parse(input.ReadLine().Trim());
					var thealth = int.Parse(input.ReadLine().Trim());

					if (tteam == 0)
					{
						frendlyUnits.Add(new Unit(tx, ty, tteam, tclazz, thealth));
					}
					else if (tteam == 1)
					{
						enemyUnits.Add(new Unit(tx, ty, tteam, tclazz, thealth));
					}
					else
					{
						throw new Exception("Incorrect unit's team");
					}
				}
                // количество юнитов в вашей команде
				friends = int.Parse(input.ReadLine().Trim());
				messages = new List<string>(friends);
				for (int i =0; i < friends; i++)
				{   
                    // сообщение от i-го юнита
					messages.Add(input.ReadLine().Trim());
				}
			}
			catch (Exception e)
			{
				throw new GameParseException("Protocol missmatch in coordinates: "+e.Message);
			}

			flushState();
		}

		/// <summary>
		/// Сбросить "выходное" состояние в ноль, то есть по-умолчанию мы не идём, не стреляем и не поворачиваемся.
		/// </summary>
		private void flushState()
		{
			fireX = 0;
			fireY = 0;
			tofire = false;

			moveTo(x, y);
			angleNew = angle;

			say = "";
		}
	}
}
