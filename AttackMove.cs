﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Math;

namespace Trading_Project
{
    public class AttackMove:Move
    {
        protected float p_attackbonus;

        protected Effect move_effect;

        public float attackbonus { get { return p_attackbonus; } }


        public AttackMove(String name, float attack_bonus, uint hr, float stm, float dm, float spm)
            : base(name,hr,stm,dm,spm)
        {
            this.p_attackbonus = attack_bonus;
        }

        public override uint attack(Character attacker, Character defender)
        {
            int hit = attacker.dice.roll();
            uint damage = 0;

            if (hit == 1)
                return damage;
            else
            {
                defender.applyEffect(move_effect);
                damage = (uint)Math.Floor(p_attackbonus * (float)attacker.strength * attacker.effect.strength_mod);

                return damage;
            }
            
        }

    }
}
