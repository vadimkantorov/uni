using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DCIMAP.GANS.SwarmIntellect;

namespace GAME
{
    internal class GameForm: Form
    {
        private readonly SolidBrush b = new SolidBrush(Color.Black);
        private readonly Font font = new Font("Arial", 12);
        private readonly Font font1 = new Font("Arial", 8);
        private readonly StringFormat format = new StringFormat();
        private readonly Color[] teams = new Color[] {Color.Red, Color.Blue};
        public GameBackground back;
        public Map2D<int> map2D;
        private Pen p = new Pen(Color.Black, 2);

        public GameForm(GameBackground back, Ether eth, Map2D<int> map2D)
        {
            this.map2D = map2D;
            this.back = back;

            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            


           ClientSize = new Size(800, 600);
           Timer t = new Timer();
           t.Interval = 10;
           t.Tick += new EventHandler(t_Tick);
           t.Start();
        

        }

        private Point arc(Point center, int M, double angle, double rad)
        {
            return new Point(
                (int) (center.X + M * rad * Math.Cos(angle)),
                (int) (center.Y + M * rad * Math.Sin(angle)));
        }

        public void Draw(Graphics g)
        {
            int M = 3;
            int O = 30;
            g.DrawImage(back.Bitmap, O, O, back.Bitmap.Width * M, back.Bitmap.Height * M);
            foreach (Fighter f in map2D.Objects)
            {
                if (f.Dead)
                    continue;
                Point p = new Point(f.Location.X * M + O + M / 2, f.Location.Y * M + O + M / 2);
                b.Color = teams[f.Team];
                g.DrawString(f.Letter, font, b, new Rectangle(p.X - 100, p.Y - 100, 200, 200), format);

                g.DrawLine(Pens.Gray, p, arc(p, M, f.Angle + f.SightAngle, f.SightLength));
                g.DrawLine(Pens.Gray, p, arc(p, M, f.Angle - f.SightAngle, f.SightLength));
                g.DrawString(f.HP.ToString() + "/" + f.Mana.ToString(), font1, b, new Rectangle(p.X - 100, p.Y - 100 + 10, 200, 200), format);
            }

            g.DrawString("Количество смертей: красные " + back.deadCount[0] + ", синие " + back.deadCount[1], font, Brushes.Black, 0, 0);
        }

        private void t_Tick(object sender, EventArgs e)
        {
            Invalidate();
            map2D.MakeTurn();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Draw(e.Graphics);
        }
    }
}