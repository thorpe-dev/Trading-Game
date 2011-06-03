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
using System.Xml.Linq;
using System.Net.Browser;

namespace SilverlightApplication1
{
    public partial class MainPage : UserControl
    {
        public static Frame frame;

        public MainPage()
        {
            InitializeComponent();
            frame = MainFrame;
        }
    }
}
