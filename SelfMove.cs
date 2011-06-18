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

        public SelfMove(String name, uint hr, uint mr, float stm, float dm, float spm, float im)
            : base(name,hr,stm,dm,spm)
        {
            move_effect = new Effect(hr, mr, stm, dm, spm, im);
        }

        public override uint attack(Character attacker, Character defender)
        {
            if (this.mana_cost < attacker.mana)
                attacker.applyEffect(move_effect);

            /* Will always return the same value */
            return 0;
        }
    }  
}
