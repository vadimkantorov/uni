using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DCIMAP.GANS.SwarmIntellect;

namespace GAME
{
    internal static class Program
    {
        private static void Start(string[] args)
        {
            if(args.Length != 4)
                throw new Exception("Запускать с параметрами: [файл карты] [к-во игроков в одной команде] [программа 1] [программа 2]");
            if(!File.Exists(args[0]))
                throw new Exception("Некорректный файл карты");
            if(!File.Exists(args[2]))
                throw new Exception("Некорректный файл первой программы");
            if(!File.Exists(args[3]))
                throw new Exception("Некорректный файл второй программы");

            int cnt;
            try
            {
                cnt = int.Parse(args[1]);
            }
            catch
            {
                throw new Exception("Неверное количество игроков");
            }
            GameBackground back = new GameBackground(args[0], cnt);
            Ether eth = new ListEther();
            eth["0"] = new string[cnt];
            eth["1"] = new string[cnt];

            // creating map with Color in Background's cells and LiteListCells in map's cells
            Map2D<int> map2D =
                new Map2D<int>(
                // its size is simmilar to image size
                    back.Width, back.Height,
                // delegate for creating new cells
                    delegate(Map2D<int> map, Coordinate2D coord) { return new ListCell<Coordinate2D, Metric2D, int>(map, coord); },
                // backround of this map is based on image
                    back,
                // ether of this map (usualy used ListEther)
                    eth);

            List<ConsoleController> l1 = new List<ConsoleController>();
            for(int j = 0; j < 2; j++)
                for(int i = 0; i < cnt; i++)
                {
                    ConsoleController c = new ConsoleController(args[j + 2], j, i);
                    Fighter f = Fighter.CreateFighter(map2D, j, i, c.Letter, c);
                    map2D.Add(new Coordinate2D(f.Location.X, f.Location.Y), f);
                    l1.Add(c);
                }

            Application.Run(new GameForm(back, eth, map2D));

            foreach(ConsoleController c in l1)
                c.Dispose();

            return;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                Start(args);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

            //            GameMap map = new GameMap("..\\..\\world.bmp");
            //GameForm form = new GameForm(map);
            //Application.Run(form);
            /*
            GameBackground b = new GameBackground("test.bmp");
            FastBitmap bmp = FastBitmap.FromFile("test.bmp");
            for (int x=0;x<bmp.Width;x++)
                for (int y = 0; y < bmp.Height; y++)
                {
                    if (b.CanSee(bmp.Width / 2, bmp.Height / 2, x, y, 20,6,Math.PI/3) && bmp.GetPixel(x,y).GetBrightness()>0.1)
                        bmp.SetPixel(x, y, Color.Yellow);
                }
            Bitmap b1 = bmp.ToBitmap();
            PictureBox box = new PictureBox();
            box.Image = b1;
            box.Dock = DockStyle.Fill;
            Form form = new Form();
            form.ClientSize = b1.Size;
            form.Controls.Add(box);
            Application.Run(form);*/
        }
    }
}