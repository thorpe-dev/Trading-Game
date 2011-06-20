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
                    im.Source = new BitmapImage(new Uri("Images/Items/InventoryBack.png", UriKind.Relative));

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
                Item i = its[x].item;
                ims[x].Source = new BitmapImage(i.icon);
                ToolTip tip = new ToolTip();
                tip.Background = new SolidColorBrush(Colors.Brown);
                tip.Content = i.getDescriptionText();
                ToolTipService.SetToolTip(ims[x], tip);
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
            {
                imgWeapon.Source = new BitmapImage(curCharacter.weapon.icon);
                ToolTip tip = new ToolTip();
                tip.Background = new SolidColorBrush(Colors.Brown);
                tip.Content = curCharacter.weapon.getDescriptionText();
                ToolTipService.SetToolTip(imgWeapon, tip);
            }
            if (curCharacter.helm != null)
            {
                imgHelmet.Source = new BitmapImage(curCharacter.helm.icon);
                ToolTip tip = new ToolTip();
                tip.Background = new SolidColorBrush(Colors.Brown);
                tip.Content = curCharacter.helm.getDescriptionText();
                ToolTipService.SetToolTip(imgHelmet, tip);
            }
            if (curCharacter.chest != null)
            {
                imgChest.Source = new BitmapImage(curCharacter.chest.icon);
                ToolTip tip = new ToolTip();
                tip.Background = new SolidColorBrush(Colors.Brown);
                tip.Content = curCharacter.chest.getDescriptionText();
                ToolTipService.SetToolTip(imgChest, tip);
            }
            if (curCharacter.legs != null)
            {
                imgLegs.Source = new BitmapImage(curCharacter.legs.icon);
                ToolTip tip = new ToolTip();
                tip.Background = new SolidColorBrush(Colors.Brown);
                tip.Content = curCharacter.legs.getDescriptionText();
                ToolTipService.SetToolTip(imgLegs, tip);
            }
            if (curCharacter.boots != null)
            {
                imgBoots.Source = new BitmapImage(curCharacter.boots.icon);
                ToolTip tip = new ToolTip();
                tip.Background = new SolidColorBrush(Colors.Brown);
                tip.Content = curCharacter.boots.getDescriptionText();
                ToolTipService.SetToolTip(imgBoots, tip);
            }
            if (curCharacter.gloves != null)
            {
                imgGloves.Source = new BitmapImage(curCharacter.gloves.icon);
                ToolTip tip = new ToolTip();
                tip.Background = new SolidColorBrush(Colors.Brown);
                tip.Content = curCharacter.gloves.getDescriptionText();
                ToolTipService.SetToolTip(imgGloves, tip);
            }
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
                    if (curCharacter.weapon.dequip(curCharacter, inBattle))
                        imgWeapon.Source = new BitmapImage(new Uri("Images/Items/InventoryBackWeapon.png", UriKind.Relative));
                    break;
                case "chest":
                    if (curCharacter.chest.dequip(curCharacter, inBattle))
                        imgChest.Source = new BitmapImage(new Uri("Images/Items/InventoryBackChest.png", UriKind.Relative));
                    break;
                case "legs":
                    if (curCharacter.legs.dequip(curCharacter, inBattle))
                        imgLegs.Source = new BitmapImage(new Uri("Images/Items/InventoryBackGreaves.png", UriKind.Relative));
                    break;
                case "boots":
                    if (curCharacter.boots.dequip(curCharacter, inBattle))
                        imgBoots.Source = new BitmapImage(new Uri("Images/Items/InventoryBackBoots.png", UriKind.Relative));
                    break;
                case "gloves":
                    if (curCharacter.gloves.dequip(curCharacter, inBattle))
                        imgGloves.Source = new BitmapImage(new Uri("Images/Items/InventoryBackGloves.png", UriKind.Relative));
                    break;
                case "helm":
                    if (curCharacter.helm.dequip(curCharacter, inBattle))
                        imgHelmet.Source = new BitmapImage(new Uri("Images/Items/InventoryBackHead.png", UriKind.Relative));
                    break;

            }
            updateInventory();
            updateStats();
            MainPage.currentSettingBar.alter_hp(curCharacter.maxHealth, curCharacter.currentHealth);
            MainPage.currentSettingBar.alter_mana(curCharacter.maxMana, curCharacter.currentMana);
        }

        private void inventory_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = (Image)sender;
            int pos = (int)img.GetValue(Grid.RowProperty) * 5 + (int)img.GetValue(Grid.ColumnProperty);

                curCharacter.inventory.ElementAt(pos).useItem(curCharacter, inBattle);
            
            updateInventory();
            updateEquipment();
            updateStats();
            MainPage.currentSettingBar.alter_hp(curCharacter.maxHealth, curCharacter.currentHealth);
            MainPage.currentSettingBar.alter_mana(curCharacter.maxMana, curCharacter.currentMana);
        }

        public void updateStats()
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
            classBox.Text = curCharacter.type.ToString();
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

