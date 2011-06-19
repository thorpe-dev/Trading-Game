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

namespace Main_Game
{
    public class D20
    {
        Random rnd;

        public D20()
        {
            rnd = new Random();
        }

        public int roll()
        {
            return rnd.Next(1, 21);
        }
    }
}
