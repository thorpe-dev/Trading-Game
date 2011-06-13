using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Trading_Project
{
    public class Battle
    {
        protected Character char_1;
        protected Character char_2;


        public Battle(Character p1, Character p2)
        {
            char_1 = p1;
            char_2 = p2;

            startBattle();
        }

        public void startBattle()
        {
            if (this.char_1.dice.roll() * this.char_1.speed * this.char_1.effect.speed_mod <
                this.char_2.dice.roll() * this.char_2.speed * this.char_2.effect.speed_mod)
                this.char_2.Attack(getMove(this.char_2), char_2);
        }

        public Move getMove(Character c)
        {
            return null;
        }
    }
}
