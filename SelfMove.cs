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
    public class SelfMove:Move
    {

        public SelfMove(String name, uint hr, float stm, float dm, float spm)
            : base(name,hr,stm,dm,spm)
        {
            move_effect = new Effect(hr, stm, dm, spm);
        }

        public uint AttackMove(Character attacker, Character defender)
        {
            attacker.applyEffect(move_effect);

            /* Will always return the same value */
            return 0;
        }
    }


    public struct Effect
    {
        protected uint p_health_restore;

        protected float p_strength_mod;
        protected float p_dexterity_mod;
        protected float p_speed_mod;

        public uint health_restore { get { return p_health_restore; } set { p_health_restore = value; } }
        public float strength_mod { get { return p_strength_mod; } set { p_strength_mod = value; } }
        public float dexterity_mod { get { return p_dexterity_mod; } set { p_dexterity_mod = value; } }
        public float speed_mod { get { return p_speed_mod; } set { p_speed_mod = value; } }

        public Effect(uint hr, float stm, float dm, float spm)
        {
            p_health_restore = hr;
            p_strength_mod = stm;
            p_dexterity_mod = dm;
            p_speed_mod = spm;
        }

        public Effect()
        {
            p_health_restore = 0;
            p_strength_mod = 1;
            p_dexterity_mod = 1;
            p_speed_mod = 1;
        }
    }   
}
