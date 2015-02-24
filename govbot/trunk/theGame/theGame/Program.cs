using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace theGame
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            При старте, программа получает на вход 
             [номер команды - 0 или 1]
             [номер экземпляра, т.е. номер игрока в команде]
             */
            Adaptor adaptor = new Adaptor(Console.In, Console.Out);
            // имеем adaptor.team и adaptor.number
            char clazz = 'P';
            Fighter fighter = new Fighter(adaptor.team, clazz);
            adaptor.init(clazz); // мы в игре
            while (true)
            {
                fighter.go(adaptor); // задали действия
                adaptor.step();
            }
        }
    }
}
