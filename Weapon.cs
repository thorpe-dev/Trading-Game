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
    public class Weapon
    {
        protected float p_attack_bonus;

        protected Effect p_weapon_effect;

        public float attack_bonus { get { return p_attack_bonus; } }
        public Effect weapon_effect { get { return p_weapon_effect; } }

        public Weapon()
        {
            p_attack_bonus = 1;
            p_weapon_effect = new Effect();
        }

        public Weapon(float ab, uint hr, uint mr, float sm, float dm, float spm, float im)
        {
            p_attack_bonus = ab;

            p_weapon_effect = new Effect(hr, mr, sm, dm, spm, im);

        }
    }
}
