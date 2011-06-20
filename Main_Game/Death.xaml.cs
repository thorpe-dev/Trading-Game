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
    public partial class DeathWindow : ChildWindow
    {

        public DeathWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenManager.SetScreen(new Tavern(false, 0));
            DialogResult = true;
        }
    }
}

