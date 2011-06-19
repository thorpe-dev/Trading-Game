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
    public partial class Shop : UserControl, IScreen
    {
        public Shop()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(Shop_KeyDown);
        }

        public UIElement Element { get { return this; } }

        private void Shop_KeyDown(object sender, KeyEventArgs e)
        {
            MovementGrid movem = new MovementGrid(mainChar);
            movem.moveChar(e);
            e.Handled = true;
            if (Grid.GetColumn(mainChar) == 4 && Grid.GetRow(mainChar) == 6)
            {
                City tCity = new City(500 - 128, 300 - 32);
                ScreenManager.SetScreen(tCity);
                tCity.Focus();
            }
            
        }

    }

    public class MovementGrid
    {
        private Image mainChar;
        public MovementGrid(Image m)
        {
            mainChar = m;
        }

        public void moveChar(KeyEventArgs e)
        {

            if (e.Key == Key.Right)
                Grid.SetColumn(mainChar, Grid.GetColumn(mainChar)+1); 
            else if (e.Key == Key.Left)
                Grid.SetColumn(mainChar, Grid.GetColumn(mainChar)-1); 
            else if (e.Key == Key.Up)
                Grid.SetRow(mainChar, Grid.GetRow(mainChar)-1); 
            else if (e.Key == Key.Down)
                Grid.SetRow(mainChar, Grid.GetRow(mainChar)+1); 
        }
    }

}
