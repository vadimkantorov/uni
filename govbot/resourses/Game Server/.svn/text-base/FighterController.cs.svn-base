using System;
using System.Collections.Generic;
using System.Drawing;

namespace GAME
{
    public class FighterControllerData
    {
        public int damage;
        public string msg = "";
        public double newAngle;
        public Point newLocation;
        public bool shot;
        public Point shotPoint;
    }

    public abstract class FighterController
    {
        public static Random Rnd = new Random();
        public abstract FighterControllerData MakeTurn(Fighter f, List<Fighter> units, string[] eth);
    }
}