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
    public partial class LootBox : ChildWindow
    {
        private Item loot;
        private IScreen returnScreen;

        public LootBox(Item _loot)
        {
            InitializeComponent();
            update(_loot);
        }

        public void addReturnScreen(IScreen _returnScreen)
        {
            returnScreen = _returnScreen;
        }

        public void update(Item _loot)
        {
            loot = _loot;
            lootIcon.Source = new BitmapImage(loot.icon);
            descLabel.Text = loot.getDescriptionText();
        }

        private void lootBtn_Click(object sender, RoutedEventArgs e)
        {
            bool looted = loot.loot(Character.currentCharacter.inventory);
            if (looted)
            {
                fullLabel.Content = "";
                DialogResult = true;
                if (returnScreen != null)
                    ScreenManager.SetScreen(returnScreen);
            }
            else
                fullLabel.Content = "Your inventory is full";
        }

        private void dropBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            if (returnScreen != null)
                ScreenManager.SetScreen(returnScreen);
        }

    }
}
