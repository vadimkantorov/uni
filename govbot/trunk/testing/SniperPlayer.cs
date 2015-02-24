using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

class T
{
    /// <summary>
    /// ������ �����, �� ������� ����� ��������� �����
    /// </summary>
    static List<Point> wayPoint;
    /// <summary>
    /// ������� ������
    /// </summary>
    static int team;
    /// <summary>
    /// ��������� ������ (� ����� ����� �� �����)
    /// </summary>
    static int state;
    /// <summary>
    /// ���������� ���������
    /// </summary>
    const int WayCount = 5;

    /// <summary>
    /// �������� ���������� � ���� ��� ������ �������
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
    /// ��������������� ���������� ��� �������� ������� � ����� ��������
    /// </summary>
    static int alter = 1;

    /// <summary>
    /// ������� ������� �� ������ ��������� (������� ��� ��������)
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
    /// ������� ������� � �������� �������
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
    /// ����������� ����� ���������� � ����� ���� �������� ������ ������ �� ������ ���������
    /// </summary>
    static void Go(int x, int y, int angle, out int newx, out int newy, out int newangle)
    {
        //�������� ����������, ����� �� ������������� �������� ������ � ����� �������
        Reflect(ref x, ref angle);
        //�������� �����, � ������� ���� ������
        Point target=wayPoint[state];
        if (target.X == x && target.Y == y)//���� � ��� ����� ��� ������
        {
            if (state==wayPoint.Count-1) //������� ��� ��������, ����� � ������ �������
            {
                newx=x;
                newy=y;
                newangle=angle+alter*30;
                alter *= -1;
                Reflect(ref newx, ref newangle);
                return;
            }
            else //���� � ��������� �����
            {
                state++;
                target=wayPoint[state];
            }
        }
        
        //������������ ��������, ������� ��� ����� ��� ��������� � ������� �����
        int dx=target.X-x;
        int dy=target.Y-y;
        double norm=Math.Sqrt(dx*dx+dy*dy);
        if (norm > 10) //���� �� ������� ����� ������ ����������, ��� �������� (��� �������� � ���� ������� 10) - �� ��������� �� �������, �� ������� �����
        {
            dx = (int)(dx * 10 / norm);
            dy = (int)(dy * 10 / norm);
        }
        newangle=(int)(Math.Atan2(dy,dx)*180/Math.PI); //������� ������ ������, �.�. �� ���� ��������
        newx=x+dx;
        newy=y+dy;
        Reflect(ref newx,ref newangle);
    }


    
    public static void Main()
    {
        
           state = 0;
           Random rnd = new Random();
           string lets = "PMSRF";

            //������ ������� � ���������� ��������� �����
            team = int.Parse(Console.ReadLine());
            Console.ReadLine();
            Console.WriteLine(lets[2]);


            bool init = true; //��������, ��� ����� ����� ���������������� �������
            
            try
            {
                while (true)
                {
                    //���� ������ ����� ���������� �� ����. ����������� ������� ������������ ��� ��� �������� �����������, �� ������� ����, � �����. 
                    while (Console.ReadLine() == null) System.Threading.Thread.Sleep(10);

                    int alive = int.Parse(Console.ReadLine());


                    if (alive == 0)//���� ���� ����, �� �������� ������������ �������� � ������ ���� �����������
                    {
                        state = 0;
                        init = true;
                        continue;
                    }

                    if (init) //�������������� �������
                    {
                        MakeWayPoint(rnd.Next(WayCount));
                        init = false;
                    }

                    int x = int.Parse(Console.ReadLine()); //��������� ���������� � ����
                    int y = int.Parse(Console.ReadLine());
                    int ang = int.Parse(Console.ReadLine());
                    Console.ReadLine(); //���������� ������ � ���������� � ����� �������
                    Console.ReadLine();
                    //������ ������� ������
                    int cnt = int.Parse(Console.ReadLine());
                    bool shot = false;
                    for (int i = 0; i < cnt; i++)
                    {
                        int ux = int.Parse(Console.ReadLine());
                        int uy = int.Parse(Console.ReadLine());
                        int ut = int.Parse(Console.ReadLine());
                        Console.ReadLine();
                        Console.ReadLine();
                        if (!shot && ut == 1) //�������� � ������� �����������
                        {
                            Console.WriteLine("1");
                            Console.WriteLine(ux);
                            Console.WriteLine(uy);
                            shot = true;
                        }
                    }


                    //������ ���������
                    cnt = int.Parse(Console.ReadLine());
                    for (int i = 0; i < cnt; i++) Console.ReadLine();

                    int nx, ny, na;
                    Go(x, y, ang, out nx, out ny, out na);
                    //����� ����� �� ��������, ���� ��� �� ��������
                    if (!shot) 
                        Console.Write("0\n0\n0\n");

                    //����� ����� ����������
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
