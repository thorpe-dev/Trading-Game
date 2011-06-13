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


        private void Shop_KeyDown(object sender, KeyEventArgs e)
        {
            Movement movem = new Movement(32, mainChar);
            movem.moveChar(e);
            e.Handled = true;
            if (Canvas.GetTop(mainChar) == 270 + 32 && Canvas.GetLeft(mainChar) == 170)
            {
                City tCity = new City(500 - 128, 300 - 32);
                ScreenManager.SetScreen(tCity);
                tCity.Focus();

            }
        }
        /////////////////////////////////////////////////
        /* This will be nice and static I fucking hope */
        /////////////////////////////////////////////////
        public UIElement Element { get { return this; } }

    }

}
