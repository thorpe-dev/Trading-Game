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
    public class NPC:Character
    {
        protected String p_NPC_type;

        private int no_turns;

        protected float odds_of_appearance;


        public String NPC_type { get { return p_NPC_type; } }
        public float odds { get { return odds_of_appearance; } }


        public NPC(String type):base()
        {
            this.p_NPC_type = type;
            this.no_turns = 0;
        }

        public override void die(Character c)
        {
            //send XP increase to server - right now winning a battle increases exp by the NPCs max health each time
            c.exp += this.max_health;
        }

        public override Move getMove()
        {
            this.no_turns++;

            if (this.no_turns < 4)
                return earlyMove();
            else
                return lateMove();
        }

        public Move earlyMove()
        {
            Move max_str_pen;
            Move max_dex_pen;
            Move max_int_pen;
            Move max_spd_pen;
            Move max_mana_pen;

            Random rnd = new Random();
            int move_choice = rnd.Next(1, 5);

            IEnumerator<Move> i = this.moves.GetEnumerator();

            i.MoveNext();
            max_str_pen = max_dex_pen = max_int_pen = max_spd_pen = max_mana_pen = i.Current;

            while (i.MoveNext())
            {
                if (i.Current.move_effect.strength_mod < max_str_pen.move_effect.strength_mod && (i.Current is AttackMove) && (i.Current.mana_cost <= this.mana))
                    max_str_pen = i.Current;

                if (i.Current.move_effect.dexterity_mod < max_str_pen.move_effect.dexterity_mod && (i.Current is AttackMove) && (i.Current.mana_cost <= this.mana))
                    max_dex_pen = i.Current;

                if (i.Current.move_effect.intelligence_mod < max_str_pen.move_effect.intelligence_mod && (i.Current is AttackMove) && (i.Current.mana_cost <= this.mana))
                    max_int_pen = i.Current;

                if (i.Current.move_effect.speed_mod < max_str_pen.move_effect.speed_mod && (i.Current is AttackMove) && (i.Current.mana_cost <= this.mana))
                    max_spd_pen = i.Current;

                if (i.Current.move_effect.mana_restore < max_str_pen.move_effect.mana_restore && (i.Current is AttackMove) && (i.Current.mana_cost <= this.mana))
                    max_mana_pen = i.Current;
            }

            switch (move_choice)
            {
                case 1:
                    return max_str_pen;
                case 2:
                    return max_dex_pen;
                case 3:
                    return max_int_pen;
                case 4:
                    return max_spd_pen;
                default:
                    return max_mana_pen;

            }
        }

        public Move lateMove()
        {
            Move max_attack;
            Move max_health; // Most health restored to NPC
            Move min_health; // Most damage done to Player

            Random rnd = new Random();
            int move_choice = rnd.Next(1, 3);

            IEnumerator<Move> i = this.moves.GetEnumerator();

            i.MoveNext();

            max_attack = max_health = min_health = i.Current;

            while (i.MoveNext())
            {
                if ((i.Current is AttackMove) && (i.Current.attackbonus < max_attack.attackbonus))
                    max_attack = i.Current;

                if ((i.Current is SelfMove) && (i.Current.move_effect.health_restore > max_health.move_effect.health_restore))
                    max_health = i.Current;

                if ((i.Current is AttackMove) && i.Current.move_effect.health_restore < min_health.move_effect.health_restore)
                    min_health = i.Current;
            }

            switch (move_choice)
            {
                case 1:
                    return max_attack;
                case 2:
                    return max_health;
                default:
                    return min_health;

            }
        }
    }
}
