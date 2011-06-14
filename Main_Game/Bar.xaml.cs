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
    public partial class Bar : UserControl, IScreen
    {
        public Bar()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(Bar_KeyDown);
        }

        private void Bar_KeyDown(object sender, KeyEventArgs e)
        {
            Movement movem = new Movement(32, mainChar);
            movem.moveChar(e);
            e.Handled = true;
            if (Canvas.GetTop(mainChar) == 270 + 32 && Canvas.GetLeft(mainChar) == 170)
            {
                City tCity = new City(500 + 4*32, 300 +6*32);
                ScreenManager.SetScreen(tCity);
                tCity.Focus();

            }
        }
        
        private void updateCharactersp()
        {
            //Main_Game.HttpConnection.httpLongPoll(resource, eventhandler, 1); //Tell Dave to sort out <1s  polling
            /* Check every period(0.2s?)
             * SELECT * FROM Character Locations 
             * -- Place sprites in those locations
             * -- Party update?
             * -- 
             * Form parties, interface on side of screen?
             * -- Invite anyone currently on screen?
             * -- If leave, quit party (set partyID to null?)
             * -- Party field, unique partyID in database?
             * -- 
             * Unable to right click on images.
             * Trade interface in bar?
             * -- Wherever it is:
             * -- Only need to know which characters are availabe
             * -- Bring up trade screen 
             * -- -- Design trade screen, grid, size of inventories with option for gold at the bottom
             * -- -- Open on both confirming trade request.
             * -- -- Sending trade request, tricky.
             * -- 
             * 
             */


        }


        public UIElement Element { get { return this; } }
    }
}
