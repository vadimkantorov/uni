using System;
using System.Collections.Generic;
using System.Drawing;
using DCIMAP.GANS.Basic;
using DCIMAP.GANS.SwarmIntellect;

namespace GAME
{
    public class GameBackground: Background<Coordinate2D, int>
    {
        private readonly Bitmap bitmap;
        private readonly string[][] eth;
        private readonly int fighterInTeam;
        private readonly int[,] map;

      public int[] deadCount;
      


        public Bitmap Bitmap
        {
            get { return bitmap; }
        }

        public int Width
        {
            get { return bitmap.Width; }
        }

        public int Height
        {
            get { return bitmap.Height; }
        }

        public string[][] Eth
        {
            get { return eth; }
        }

        public int FighterInTeam
        {
            get { return fighterInTeam; }
        }

        public bool Inside(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <summary>
        /// ¬идно ли из одной точки другую, с учетом угла обзора и длины.
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="length"></param>
        /// <param name="angle"></param>
        /// <param name="sight"></param>
        /// <returns></returns>
        public bool CanSee(int x0, int y0, int x1, int y1, double length, double angle, double sight)
        {
            if(Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2)) > length)
                return false;
            double a = Math.Atan2(y1 - y0, x1 - x0);
            while(a < 0)
                a += 2 * Math.PI;
            while(angle < 0)
                angle += 2 * Math.PI;
            double d1 = Math.Abs(a - angle);
            double d2 = (2 * Math.PI - Math.Max(a, angle)) + Math.Min(a, angle);
            if(Math.Min(d1, d2) > sight)
                return false;
            return CanSee(x0, y0, x1, y1);
        }

        /// <summary>
        /// ¬идно ли из точки (x0,y0) точку (x1,y1) (можно ли их соединить пр€мой)
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        public bool CanSee(int x0, int y0, int x1, int y1)
        {
            if(!Inside(x0, y0))
                return false;
            if(!Inside(x1, y1))
                return false;

            int dx = x1 - x0;
            int dy = y1 - y0;
            if(dx == 0 && dy == 0)
                return true;
            int cnt = Math.Max(Math.Abs(dx), Math.Abs(dy));
            double tx = (double) dx / cnt;
            double ty = (double) dy / cnt;
            double nx = x0;
            double ny = y0;
            for(int i = 0; i < cnt; i++)
            {
                nx += tx;
                ny += ty;
                if(map[(int) nx, (int) ny] == 1)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// ¬озвращает true если возможно дойти из точки x0,y0 в точку x1,y1 по правилам
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        public bool CanGo(int x0, int y0, int x1, int y1, double length)
        {
            if(!Inside(x0, y0))
                return false;
            if(!Inside(x1, y1))
                return false;
            if(Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2)) > length)
                return false;

            int xmin = Math.Min(x0, x1);
            int xmax = Math.Max(x0, x1);
            int ymin = Math.Min(y0, y1);
            int ymax = Math.Max(y0, y1);
            int dx = Math.Sign(x1 - x0);
            int dy = Math.Sign(y1 - y0);
            List<Point> list = new List<Point>();
            list.Add(new Point(x0, y0));
            int ptr = 0;
            Point target = new Point(x1, y1);
            while(ptr != list.Count)
            {
                if(list[ptr] == target)
                    return true;
                for(int k = 1; k <= 2; k++)
                {
                    Point p = new Point(list[ptr].X + dx * (k / 2), list[ptr].Y + dy * (k % 2));
                    if(p.X >= xmin && p.X <= xmax && p.Y >= ymin && p.Y <= ymax && map[p.X, p.Y] == 0
                        && !list.Contains(p))
                        list.Add(p);
                }
                ptr++;
            }
            return false;
        }

        public GameBackground(string fname, int fighterInTeam)
        {
            this.fighterInTeam = fighterInTeam;
            eth = new string[2][];
            eth[0]=new string[fighterInTeam];
            eth[1]=new string[fighterInTeam];
            deadCount = new int[2];

            FastBitmap bmp = FastBitmap.FromFile(fname);
            bitmap = bmp.ToBitmap();
            map = new int[bmp.Width, bmp.Height];
            for (int x = 0; x < bmp.Width; x++)
                for (int y = 0; y < bmp.Height; y++)
                {
                    map[x, y] = bmp.GetPixel(x, y).GetBrightness() < 0.5 ? 1 : 0;
                }
        }

        public override int GetPoint(Coordinate2D coordinate)
        {
            return map[coordinate.X, coordinate.Y];
        }

        protected override void Draw(Graphics where) {}
    }
}