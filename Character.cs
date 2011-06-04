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
        protected int health;


        protected int strength;
        protected int dexterity;
        protected int speed;

        protected List<Move> moves;
    }
}
