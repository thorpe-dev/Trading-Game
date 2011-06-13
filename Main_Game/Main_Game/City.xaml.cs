using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Main_Game
{
    public partial class City : UserControl, IScreen
    {
        private int step = 32;
        
        public City(int Left, int Top)
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(city_KeyDown);
            Canvas.SetLeft(mainChar, Left);
            Canvas.SetTop(mainChar, Top);
        }
        
        public UIElement Element { get { return this; } }

        private void city_KeyDown(object sender, KeyEventArgs e)
        {
            Movement move = new Movement(step, mainChar);
            move.moveChar(e);
            e.Handled = true;
            if (Canvas.GetTop(mainChar) == 300 - 2*step && Canvas.GetLeft(mainChar) == 500 - 4*step)
            {
                Shop tShop = new Shop();
                ScreenManager.SetScreen(tShop);
                tShop.Focus();

            }
            else if (Canvas.GetTop(mainChar) == 300 + 5*step && Canvas.GetLeft(mainChar) == 500 + 4*step)
            {
                Bar tBar = new Bar();
                ScreenManager.SetScreen(tBar);
                tBar.Focus();
            }        
        }


       
    }
     
    public class Movement
    {
        
        private int step;
        private Image mainChar;
        public Movement(int s, Image m)
        {
            step = s;
            mainChar = m;
        }

        public void moveChar(KeyEventArgs e)
        {

            if (e.Key == Key.Right)
                Canvas.SetLeft(mainChar, Canvas.GetLeft(mainChar) + step);
            else if (e.Key == Key.Left)
                Canvas.SetLeft(mainChar, Canvas.GetLeft(mainChar) - step);
            else if (e.Key == Key.Up)
                Canvas.SetTop(mainChar, Canvas.GetTop(mainChar) - step);
            else if (e.Key == Key.Down)
                Canvas.SetTop(mainChar, Canvas.GetTop(mainChar) + step);
        }
    }
    


}
