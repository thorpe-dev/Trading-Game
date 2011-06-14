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
    abstract class Character
    {
        /* Each character has a die */

        public D20 dice;

        /* Every player and NPC should have these stats */
        protected uint p_health;


        protected uint p_strength;
        protected uint p_dexterity;
        protected uint p_speed;

        protected uint p_exp;

        protected Effect p_effect;

        /* List of things the character can do */
        protected List<Move> moves;


        /* Fields used for constructors  */

        public uint health { get { return p_health; } }

        public uint strength { get { return p_strength; } }
        public uint dexterity { get { return p_dexterity; } }
        public uint speed { get { return p_speed; } }
        public uint exp { get { return p_exp; } set { p_exp = value; } }

        public Effect effect { get { return p_effect; } }



        public Character()
        {
            p_effect = new Effect();
            dice = new D20();
            p_health = 50;
            p_strength = 10;
            p_dexterity = 10;
            p_speed = 10;
        }


        public void Attack(Move move, Character enemy)
        {
            uint damage = move.attack(this, enemy);
            if ((int)(this.health - damage) <= 0)
                enemy.die(this);
        }

        public void applyEffect(Effect effect)
        {
            this.p_effect.strength_mod *= effect.strength_mod;
            this.p_effect.dexterity_mod *= effect.dexterity_mod;
            this.p_effect.speed_mod *= effect.speed_mod;

            this.p_effect.health_restore += effect.health_restore;

        }

        public abstract void die(Character c)
        {

        }
    }
}
