using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace GAME
{
    internal class ConsoleController: FighterController, IDisposable
    {
        private readonly string data;
        public readonly char Letter;
        private readonly Process p;

        public ConsoleController(string process, int team, int number)
        {
            data = "Файл: " + process + " Команда " + team + " Номер игрока " + number;
            try
            {
                p = new Process();
                p.StartInfo.FileName = process;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();
            }
            catch(Exception e)
            {
                throw new Exception("Не могу запустить процесс игрока." + data + ". " + e.Message);
            }

            try
            {
                Write(team);
                Write(number);
                p.StandardInput.Flush();
                //Thread.Sleep(100);
                Letter = p.StandardOutput.ReadLine()[0];
            }
            catch
            {
                throw new Exception("Не удается инициализироваться. " + data);
            }

            if(Fighter.GoodLetters.IndexOf(Letter) == -1)
                throw new Exception("Неверный тип игрока " + data);
        }


        private void Write(object s)
        {
            p.StandardInput.WriteLine(s.ToString());
            Console.WriteLine(s.ToString());
        }

        private string Read()
        {
            string s = null;
            while (s == null)
            {
                s = p.StandardOutput.ReadLine();
                Thread.Sleep(10);
            }
            Console.WriteLine(s);
            return s;
        }

        public override FighterControllerData MakeTurn(Fighter f, List<Fighter> units, string[] eth)
        {
            FighterControllerData data = new FighterControllerData();
            Write("Begin");
            if(f.Dead)
            {
                Write("0");
                return data;
            }
            Write("1");
            Write(f.Location.X);
            Write(f.Location.Y);
            Write((int) (f.Angle * 180 / Math.PI));
            Write(f.HP);
            Write(f.Mana);
            Write(units.Count);
            for(int j = 0; j < units.Count; j++)
            {
                Write(units[j].Location.X);
                Write(units[j].Location.Y);
                Write(units[j].Team == f.Team ? 0 : 1);
                Write(units[j].Letter);
                Write(units[j].HP);
            }
            Write(eth.Length);
            for(int i = 0; i < eth.Length; i++)
                Write(eth[i] == null ? "" : eth[i]);
            p.StandardInput.Flush();
          //  Thread.Sleep(200);
            Console.WriteLine("Reading...");
            try
            {
                data.shot = Read() == "1";
                data.shotPoint.X = int.Parse(Read());
                data.shotPoint.Y = int.Parse(Read());
                data.newLocation.X = int.Parse(Read());
                data.newLocation.Y = int.Parse(Read());
                data.newAngle = double.Parse(Read()) * Math.PI / 180;
                data.msg = Read();
            }
            catch
            {
                throw new Exception("Не могу разобрать ответ программы " + data);
            }

            if(data.shot)
                data.shot = Rnd.Next(100) < f.Precision;
            data.damage = f.Damage;
            return data;
        }

        #region IDisposable Members

        public void Dispose()
        {
            p.Kill();
        }

        #endregion
    }
}