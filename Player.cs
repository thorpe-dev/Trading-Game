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
using System.Collections.Generic;

namespace Trading_Project
{
    public class Player:Character
    {
        protected String p_name;

        public String name { get { return p_name; } }
        protected Move[] preferredMoves;


        public Player(String name):base()
        {
            this.p_name = name;
            preferredMoves = new Move[4];
        }

        public override void die(Character c)
        {
            //send XP increase to server - right now winning a battle increases exp by 1 each time
            c.exp += 1;
        }

        public override Move getMove()
        {
            Random rnd = new Random();

            return this.preferredMoves[rnd.Next(0, this.preferredMoves.Length)];
        }
    }
}
