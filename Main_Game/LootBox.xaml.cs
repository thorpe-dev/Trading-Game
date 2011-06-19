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
            string descString = _loot.name + "\n" +
                                _loot.description + "\n";
            if (_loot is Consumable)
            {
                Consumable c = _loot as Consumable;
                descString += "Regenerates " + c.amountRegenerated + " " + Enum.GetName(typeof(ConsumableType), c.type);
            }
            else if (_loot is Weapon)
            {
                Weapon w = _loot as Weapon;
                descString += "Stat bonuses:\n" +
                              "\tHealth: " + w.effect.healthMod + "\n" +
                              "\tMana: " + w.effect.manaMod + "\n" +
                              "\tStrength: " + w.effect.strengthMod + "\n" +
                              "\tAgility: " + w.effect.agilityMod + "\n" +
                              "\tIntelligence: " + w.effect.intelligenceMod + "\n" +
                              "\tSpeed: " + w.effect.speedMod;

            }
            else
            {
                Armour a = _loot as Armour;
                descString += "Stat bonuses:\n" +
                              "\tHealth: " + a.stats.healthMod + "\n" +
                              "\tMana: " + a.stats.manaMod + "\n" +
                              "\tStrength: " + a.stats.strengthMod + "\n" +
                              "\tAgility: " + a.stats.agilityMod + "\n" +
                              "\tIntelligence: " + a.stats.intelligenceMod + "\n" +
                              "\tSpeed: " + a.stats.speedMod;
            }
            descLabel.Text = descString;
        }

        private void lootBtn_Click(object sender, RoutedEventArgs e)
        {
            bool looted = loot.loot(Character.currentCharacter.inventory);
            if (looted)
            {
                fullLabel.Content = "";
                lootBtn.IsEnabled = false;
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
