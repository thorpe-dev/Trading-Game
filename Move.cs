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
    public abstract class Move
    {
        protected String p_name;

        protected Effect p_move_effect;

        protected uint p_mana_cost;

        protected float p_attackbonus;

        public float attackbonus { get { return p_attackbonus; } }

        public uint mana_cost { get { return p_mana_cost; } }

        public String name { get { return p_name; } }

        public Effect move_effect { get { return p_move_effect; } set { p_move_effect = value; } }


        public Move(String name, uint hr, float stm, float dm, float spm)
        {
            this.p_name = name;
            this.p_move_effect = new Effect();
            this.p_attackbonus = 1;
        }

        public abstract uint attack(Character attacker, Character defender);

    }
}
