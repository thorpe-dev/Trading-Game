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
    public partial class sideBar : UserControl, IScreen
    {

        public int charHealthCurrent = 20;
        public int charHealthMax = 100;
        public int charMagicCurrent = 30;
        public int charMagicMax = 60;

        public sideBar()
        {
            InitializeComponent();
            setCharHealthCur(charHealthCurrent);
            setCharHealthMax(charHealthMax);
            setCharMagicCur(charMagicCurrent);
            setCharMagicMax(charMagicMax);

            btnEquip.IsChecked = true;
        }

        public UIElement Element { get { return this; } }

        public void setCharHealthCur(int hp)
        {
            txtHealthCurrent.Text = hp.ToString();
            prgCharHealth.Value = hp;
        }

        public void setCharHealthMax(int maxhp)
        {
            txtHealthMax.Text = maxhp.ToString();
            prgCharHealth.Maximum = maxhp;
        }

        public void setCharMagicCur(int magic)
        {
            txtMagicCurrent.Text = magic.ToString();
            prgCharMagic.Value = magic;
        }

        public void setCharMagicMax(int maxmagic)
        {
            txtMagicMax.Text = maxmagic.ToString();
            prgCharMagic.Maximum = maxmagic;
        }

        private void btnEquip_Checked(object sender, RoutedEventArgs e)
        {
            btnStats.IsChecked = false;
            recEquipBG.Visibility = Visibility.Visible;
            brdEquipped.Visibility = Visibility.Visible;
        }

        private void btnStats_Checked(object sender, RoutedEventArgs e)
        {
            btnEquip.IsChecked = false;
            brdEquipped.Visibility = Visibility.Collapsed;
            recEquipBG.Visibility = Visibility.Collapsed;

        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            MessageBox.Show("Row:" + btn.GetValue(Grid.RowProperty) + " Col: " + btn.GetValue(Grid.ColumnProperty) + "clicked");
        }

        private void btnEquipment_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            MessageBox.Show("Row:" + btn.GetValue(Grid.RowProperty) + " Col: " + btn.GetValue(Grid.ColumnProperty) + "clicked");
        }

    }

}
