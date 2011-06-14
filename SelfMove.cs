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

        public override uint attack(Character attacker, Character defender)
        {
            attacker.applyEffect(move_effect);

            /* Will always return the same value */
            return 0;
        }
    }  
}
