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

namespace Main_Game
{
    public partial class sideBar : UserControl, IScreen
    {

        public int charHealthCurrent = 20;
        public int charHealthMax = 100;
        public int charMagicCurrent = 30;
        public int charMagicMax = 60;
        public bool inBattle = false;
        public Character curCharacter = Character.currentCharacter;


        public sideBar()
        {
            InitializeComponent();
            setCharHealthCur(charHealthCurrent);
            setCharHealthMax(charHealthMax);
            setCharMagicCur(charMagicCurrent);
            setCharMagicMax(charMagicMax);

            //Initialise inventory
            //Initialise equipment

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
            recEquipBG.Visibility = Visibility.Visible;
            brdEquipped.Visibility = Visibility.Visible;
        }

        private void btnStats_Checked(object sender, RoutedEventArgs e)
        {
            brdEquipped.Visibility = Visibility.Collapsed;
            recEquipBG.Visibility = Visibility.Collapsed;
        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
           // MessageBox.Show("Row:" + btn.GetValue(Grid.RowProperty) + " Col: " + btn.GetValue(Grid.ColumnProperty) + "clicked");
            int pos = (int)btn.GetValue(Grid.RowProperty) * 5 +  (int)btn.GetValue(Grid.ColumnProperty);
            if (curCharacter.inventory.ElementAt(pos).useItem(curCharacter, inBattle))
            {
                updateEquipment();
                updateInventory();
            }
        }

        private void btnEquipment_Click(object sender, RoutedEventArgs e)
        {
            
            var btn = (Button)sender;
            switch (btn.Tag.ToString()){
                case "weapon":
                    if (!curCharacter.weapon.dequip(curCharacter))
                        imgWeapon.Source = new BitmapImage(new Uri("Images/Items/weapon.png", UriKind.Relative));
                    break;
                case "chest":
                    if (!curCharacter.chest.dequip(curCharacter))
                        imgChest.Source = new BitmapImage(new Uri("Images/Items/armour.png", UriKind.Relative));
                    break;
                case "legs":
                    if (!curCharacter.legs.dequip(curCharacter))
                        imgLegs.Source = new BitmapImage(new Uri("Images/Items/legs.png", UriKind.Relative));
                    break;
                case "boots":
                    if (!curCharacter.boots.dequip(curCharacter))
                        imgBoots.Source = new BitmapImage(new Uri("Images/Items/boots.png", UriKind.Relative));
                    break;
                case "gloves":
                    if (!curCharacter.gloves.dequip(curCharacter))
                        imgGloves.Source = new BitmapImage(new Uri("Images/Items/gloves.png", UriKind.Relative));
                    break;
                case "helm":
                    if (!curCharacter.helm.dequip(curCharacter))
                        imgHelmet.Source = new BitmapImage(new Uri("Images/Items/head.png", UriKind.Relative));
                    break;
            }
        }
        private void updateInventory()
        {
            int locCount = 0;
            foreach (ItemStack it in curCharacter.inventory)
            {
                var itemImage = (from d in grdInventory.Children
                               where (Grid.GetColumn(d as FrameworkElement) == locCount % 5 && Grid.GetRow(d as FrameworkElement) == locCount / 4)
                               select d).FirstOrDefault();
                //(itemImage as Image).Source = it.icon;
                locCount += 1;
            }
        }

        private void updateEquipment()
        {
            
            imgWeapon.Source = new BitmapImage(curCharacter.weapon.icon);
            imgHelmet.Source = new BitmapImage(curCharacter.helm.icon);
            imgChest.Source = new BitmapImage(curCharacter.chest .icon);
            imgLegs.Source = new BitmapImage(curCharacter.legs.icon);
            imgBoots.Source = new BitmapImage(curCharacter.boots.icon);
            imgGloves.Source = new BitmapImage(curCharacter.gloves.icon);
            
        }

    }

}
