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
    public abstract class Character
    {
        /* Each character has a die */

        public D20 dice;

        /* Every player and NPC should have these stats */
        protected uint max_health;
        protected uint p_health;

        protected uint max_mana;
        protected uint p_mana;


        protected uint p_strength;
        protected uint p_dexterity;
        protected uint p_speed;
        protected uint p_intelligence;

        protected uint p_exp;

        protected Effect p_effect;
        protected Weapon p_weapon;

        /* List of things the character can do */
        protected List<Move> moves;


        /* Fields used for constructors  */

        public uint health { get { return p_health; } set { p_health = value; } }

        public uint strength { get { return (uint)Math.Floor((float)p_strength * p_effect.strength_mod); } }
        public uint dexterity { get { return (uint)Math.Floor((float)p_dexterity * p_effect.dexterity_mod); } }
        public uint speed { get { return (uint)Math.Floor((float)p_speed * p_effect.speed_mod); } }
        public uint intelligence { get { return (uint)Math.Floor((float)p_speed * p_effect.intelligence_mod); } }

        public uint mana { get { return p_mana; } set { p_mana = value; } }

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
                enemy.health = 0;

            this.health -= damage;
        }

        public void applyEffect(Effect effect)
        {
            this.p_effect.strength_mod *= effect.strength_mod;
            this.p_effect.dexterity_mod *= effect.dexterity_mod;
            this.p_effect.speed_mod *= effect.speed_mod;

            if (this.p_health + effect.health_restore < this.max_health)
                this.p_health += effect.health_restore;
            else
                this.p_health = this.max_health;

            if (this.p_mana + effect.mana_restore < this.max_mana)
                this.p_mana += effect.mana_restore;
            else
                this.p_mana = this.max_mana;

        }

        public abstract void die(Character c);

        public abstract Move getMove();
    }
}
