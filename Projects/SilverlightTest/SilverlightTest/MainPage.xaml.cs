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
using System.Windows.Media.Imaging;

namespace SilverlightTest
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void but_stars_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bi3 = new BitmapImage(new Uri("/SilverlightTest;component/Images/RAWR.JPG", UriKind.Relative));
            img_elder_sign.Source = bi3;
            MessageBox.Show("RAWR!");
        }

        private void img_elder_sign_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }



    }
}
