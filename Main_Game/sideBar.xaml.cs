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
        public bool inBattle = false;
        public Character curCharacter;


        public sideBar()
        {
            InitializeComponent();
            curCharacter = Character.currentCharacter;
            updateEquipment();
            updateInventory();  
            curCharacter.calculateStats();
            updateStats();
            txtCharName.Text = curCharacter.name;
        }

        public UIElement Element { get { return this; } }

        //public void setCharHealthCur(int hp)
        //{
        //    txtHealthCurrent.Text = hp.ToString();
        //    prgCharHealth.Value = hp;
        //}

        //public void setCharHealthMax(int maxhp)
        //{
        //    txtHealthMax.Text = maxhp.ToString();
        //    prgCharHealth.Maximum = maxhp;
        //}

        //public void setCharMagicCur(int magic)
        //{
        //    txtMagicCurrent.Text = magic.ToString();
        //    prgCharMagic.Value = magic;
        //}

        //public void setCharMagicMax(int maxmagic)
        //{
        //    txtMagicMax.Text = maxmagic.ToString();
        //    prgCharMagic.Maximum = maxmagic;
        //}

        public void updateInventory()
        {
            int count = 0;

            List<ItemStack> its = new List<ItemStack>();
            List<Image> ims = new List<Image>();
            List<TextBlock> tbs = new List<TextBlock>();

            foreach (ItemStack it in curCharacter.inventory)
            {
                its.Add(it);
                count++;
            }

            foreach (Object i in grdInventory.Children)
            {

                // clear images
                Image im = i as Image;
                TextBlock tb = i as TextBlock;

                // If it's an image
                if (im != null)
                {
                    im.Source = new BitmapImage(new Uri("Images/Items/head.png", UriKind.Relative));
                    ims.Add(im);
                }
                // If it's a TextBlock
                else if (tb != null)
                {
                    tb.Text = " ";
                    tbs.Add(tb);
                }
            }


            for (int x = 0; x < count; x++)
            {
                ims[x].Source = new BitmapImage(its[x].item.icon);
                if (its[x].stackSize > 1)
                    tbs[x].Text = its[x].stackSize.ToString();
                else
                {
                    tbs[x].Text = "";
                }
            }

            grdInventory.UpdateLayout();

        }

        private void updateEquipment()
        {
            if (curCharacter.weapon != null)
                imgWeapon.Source = new BitmapImage(curCharacter.weapon.icon);
            if (curCharacter.helm != null)
                imgHelmet.Source = new BitmapImage(curCharacter.helm.icon);
            if (curCharacter.chest != null)
                imgChest.Source = new BitmapImage(curCharacter.chest.icon);
            if (curCharacter.legs != null)
                imgLegs.Source = new BitmapImage(curCharacter.legs.icon);
            if (curCharacter.boots != null)
                imgBoots.Source = new BitmapImage(curCharacter.boots.icon);
            if (curCharacter.gloves != null)
                imgGloves.Source = new BitmapImage(curCharacter.gloves.icon);
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            grdSideBar.Background = new SolidColorBrush(Colors.Red);
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            grdSideBar.Background = new SolidColorBrush(Colors.Blue);
        }

        private void equip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            switch (((Image)sender).Tag.ToString())
            {
                case "weapon":
                    if (curCharacter.weapon.dequip(curCharacter))
                        imgWeapon.Source = new BitmapImage(new Uri("Images/Items/weapon.png", UriKind.Relative));
                    break;
                case "chest":
                    if (curCharacter.chest.dequip(curCharacter))
                        imgChest.Source = new BitmapImage(new Uri("Images/Items/armour.png", UriKind.Relative));
                    break;
                case "legs":
                    if (curCharacter.legs.dequip(curCharacter))
                        imgLegs.Source = new BitmapImage(new Uri("Images/Items/legs.png", UriKind.Relative));
                    break;
                case "boots":
                    if (curCharacter.boots.dequip(curCharacter))
                        imgBoots.Source = new BitmapImage(new Uri("Images/Items/boots.png", UriKind.Relative));
                    break;
                case "gloves":
                    if (curCharacter.gloves.dequip(curCharacter))
                        imgGloves.Source = new BitmapImage(new Uri("Images/Items/gloves.png", UriKind.Relative));
                    break;
                case "helm":
                    if (curCharacter.helm.dequip(curCharacter))
                        imgHelmet.Source = new BitmapImage(new Uri("Images/Items/head.png", UriKind.Relative));
                    break;

            }
            updateInventory();
            updateStats();
        }

        private void inventory_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = (Image)sender;
            int pos = (int)img.GetValue(Grid.RowProperty) * 5 + (int)img.GetValue(Grid.ColumnProperty);

                curCharacter.inventory.ElementAt(pos).useItem(curCharacter, inBattle);
            
            updateInventory();
            updateEquipment();
            updateStats();
        }

        private void updateStats()
        {
            curCharacter.calculateStats();
            txtStrengthVal.Text = curCharacter.strength.ToString();
            txtAgilityVal.Text = curCharacter.agility.ToString();
            txtIntVal.Text = curCharacter.intelligence.ToString();
            txtSpeedVal.Text = curCharacter.speed.ToString();
            //setCharMagicMax(curCharacter.maxMana);
            //setCharHealthMax(curCharacter.maxHealth);
            //setCharHealthCur(curCharacter.currentHealth);
            //setCharMagicCur(curCharacter.currentMana);

            txtLevel.Text = "Level " + curCharacter.level.ToString();
            txtExp.Text = "Exp to next level: " + curCharacter.expToNext.ToString();
            txtGold.Text = "Gold: " + curCharacter.money.ToString();

        }

        private void inventory_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Drop
            var img = (Image)sender;
            int pos = (int)img.GetValue(Grid.RowProperty) * 5 + (int)img.GetValue(Grid.ColumnProperty);
            curCharacter.inventory.ElementAt(pos).dropItem(curCharacter.inventory);
            updateInventory();
            e.Handled = true;
        }
    }
}

