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
    public class NPC:Character
    {
        protected String p_NPC_type;

        protected float odds_of_appearance;


        public String NPC_type { get { return p_NPC_type; } }
        public float odds { get { return odds_of_appearance; } }


        public NPC(String type):base()
        {
            this.p_NPC_type = type;
        }

        public override void die(Character c)
        {
            
        }

        public override Move getMove()
        {
            Random rnd = new Random();

            return this.moves[rnd.Next(0, this.moves.Count)];
        }
    }
}
