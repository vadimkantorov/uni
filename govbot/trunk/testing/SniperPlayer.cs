using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

class T
{
    /// <summary>
    /// —писок точек, по которым будет следовать игрок
    /// </summary>
    static List<Point> wayPoint;
    /// <summary>
    ///  оманда игрока
    /// </summary>
    static int team;
    /// <summary>
    /// —осто€ние игрока (к какой точке он бежит)
    /// </summary>
    static int state;
    /// <summary>
    ///  оличество маршрутов
    /// </summary>
    const int WayCount = 5;

    /// <summary>
    /// ќтражаем координаты и угол дл€ правой команды
    /// </summary>
    static void Reflect(ref int x, ref int angle)
    {
        if (team == 1)
        {
            x = 199 - x;
            angle = 180 - angle;

        }
    }

    /// <summary>
    /// ¬спомогательна€ переменна€ дл€ верчени€ головой в конце маршрута
    /// </summary>
    static int alter = 1;

    /// <summary>
    /// —оздает маршрут из списка координат (введена дл€ удобства)
    /// </summary>
    /// <param name="pts"></param>
    static void Create(params int[] pts)
    {
        wayPoint=new List<Point>();
        for (int i = 0; i < pts.Length; i += 2)
        {
            wayPoint.Add(new Point(pts[i], pts[i + 1]));
        }
    }

    /// <summary>
    /// —оздает маршрут с заданным номером
    /// </summary>
    static void MakeWayPoint(int number)
    {
        switch (number)
        {
            case 0:
                Create(5, 137, 38, 138, 71, 143, 79, 143, 86, 132, 104,129, 80, 127);
                break;

            case 1:
                Create(8, 81, 20, 92, 19, 121, 42, 125, 99, 82, 139,122,176,121,177,88,194,66,194,51);
                break;
               
            case 2:
                Create(8, 81, 20, 92, 19, 121, 42, 125, 99, 82, 130, 47, 176, 47, 178, 81, 193, 78, 193, 40);
                break;
                
            case 3:
                Create(8, 81, 20, 92, 19, 121, 42, 125, 99, 82, 117, 81, 117, 90);
                break;

            case 4:
                Create(9, 27, 43, 27, 40, 15, 29, 14, 30, 4, 78, 7, 84, 67, 98, 90, 130, 47, 176, 47, 178, 81, 193, 78, 193, 40);
                break;
        }
    }


 

    /// <summary>
    /// –асчитывает новые координаты и новый угол поворота головы исход€ из старых координат
    /// </summary>
    static void Go(int x, int y, int angle, out int newx, out int newy, out int newangle)
    {
        //отражаем координаты, чтобы не рассматривать отдельно правую и левую команду
        Reflect(ref x, ref angle);
        //¬ыбираем точку, в которую идем сейчас
        Point target=wayPoint[state];
        if (target.X == x && target.Y == y)//≈сли в эту точку уже пришли
        {
            if (state==wayPoint.Count-1) //ћаршрут уже закончен, стоим и крутим головой
            {
                newx=x;
                newy=y;
                newangle=angle+alter*30;
                alter *= -1;
                Reflect(ref newx, ref newangle);
                return;
            }
            else //идем в следующую точку
            {
                state++;
                target=wayPoint[state];
            }
        }
        
        //рассчитываем смещение, которое нам нужно дл€ попадани€ в целевую точку
        int dx=target.X-x;
        int dy=target.Y-y;
        double norm=Math.Sqrt(dx*dx+dy*dy);
        if (norm > 10) //если до целевой точки больше рассто€ние, чем скорость (дл€ простоты у всех игроков 10) - то смещаемс€ на столько, на сколько можно
        {
            dx = (int)(dx * 10 / norm);
            dy = (int)(dy * 10 / norm);
        }
        newangle=(int)(Math.Atan2(dy,dx)*180/Math.PI); //смотрим всегда вперед, т.е. по ходу движени€
        newx=x+dx;
        newy=y+dy;
        Reflect(ref newx,ref newangle);
    }


    
    public static void Main()
    {
        
           state = 0;
           Random rnd = new Random();
           string lets = "PMSRF";

            //читаем команду и сбрасываем случайный класс
            team = int.Parse(Console.ReadLine());
            Console.ReadLine();
            Console.WriteLine(lets[2]);


            bool init = true; //означает, что нужно будет инициализировать маршрут
            
            try
            {
                while (true)
                {
                    //∆дем нового блока информации на вход. ќб€зательно следует использовать эту или подобную конструкции, по крайней мере, в шарпе. 
                    while (Console.ReadLine() == null) System.Threading.Thread.Sleep(10);

                    int alive = int.Parse(Console.ReadLine());


                    if (alive == 0)//если боец умер, то помечаем иницализацию маршрута и дальше ждем воскрешени€
                    {
                        state = 0;
                        init = true;
                        continue;
                    }

                    if (init) //инициализируем маршрут
                    {
                        MakeWayPoint(rnd.Next(WayCount));
                        init = false;
                    }

                    int x = int.Parse(Console.ReadLine()); //считываем координаты и угол
                    int y = int.Parse(Console.ReadLine());
                    int ang = int.Parse(Console.ReadLine());
                    Console.ReadLine(); //пропускаем данные о хитпоинтах и очках энергии
                    Console.ReadLine();
                    //„итаем видимых юнитов
                    int cnt = int.Parse(Console.ReadLine());
                    bool shot = false;
                    for (int i = 0; i < cnt; i++)
                    {
                        int ux = int.Parse(Console.ReadLine());
                        int uy = int.Parse(Console.ReadLine());
                        int ut = int.Parse(Console.ReadLine());
                        Console.ReadLine();
                        Console.ReadLine();
                        if (!shot && ut == 1) //стрел€ем в первого попавшегос€
                        {
                            Console.WriteLine("1");
                            Console.WriteLine(ux);
                            Console.WriteLine(uy);
                            shot = true;
                        }
                    }


                    //читаем сообщени€
                    cnt = int.Parse(Console.ReadLine());
                    for (int i = 0; i < cnt; i++) Console.ReadLine();

                    int nx, ny, na;
                    Go(x, y, ang, out nx, out ny, out na);
                    //пишем отказ от выстрела, если еще не написали
                    if (!shot) 
                        Console.Write("0\n0\n0\n");

                    //пишем новые координаты
                    Console.WriteLine(nx);
                    Console.WriteLine(ny);
                    Console.WriteLine(na);
                    Console.WriteLine("");
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

       
    }
}
