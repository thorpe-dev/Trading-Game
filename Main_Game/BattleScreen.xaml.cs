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
    public partial class BattleScreen : UserControl, IScreen
    {
        public Battle battle { get; set; }
        public MapScreen dungeon { get; set; }

        public BattleScreen(Character c, Creep creep, MapScreen _dungeon)
        {
            InitializeComponent();
            battle = new Battle(c, creep, this);
            populateEnemy(creep);
            populateAbilityGrid(c);
            dungeon = _dungeon;
            battle.beginTurn();
        }

        private void populateEnemy(Creep creep)
        {
            imgEnemyIcon.Source = new BitmapImage(creep.icon);
        }

        private void populateAbilityGrid(Character c)
        {
            Ability[] abilities = c.abilities.Values.ToArray();
            for (int n = 0; n < abilities.Length; n++)
            {
                Ability a = abilities[n];
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(Ability.iconSize);
                abilityGrid.ColumnDefinitions.Add(col);
                Image img = new Image();
                img.Source = new BitmapImage(a.icon);
                img.Tag = a;
                img.MouseLeftButtonDown += new MouseButtonEventHandler(useAbility);
                ToolTip t = new ToolTip();
                t.Content = a.name + "  Mana cost:" + a.manaCost + "\n" + a.description;
                t.Background = new SolidColorBrush(Colors.Brown);
                ToolTipService.SetToolTip(img, t);
                Grid.SetRow(img, 0);
                Grid.SetColumn(img, n);
                abilityGrid.Children.Add(img);
            }

        }

        private void useAbility(object sender, MouseButtonEventArgs e)
        {
            if (battle.isPlayersTurn)
            {
                battle.isPlayersTurn = false;
                Ability a = (sender as Image).Tag as Ability;
                if (battle.playerCanCast(a))
                    battle.playerAttack(a);
                    
            }
        }

        public void addBattleText(string text)
        {
            TextBlock block = new TextBlock();
            block.Margin = new Thickness(5, 0, 0, 0);
            Color c = new Color();
            c.A = 255;
            c.R = 144;
            c.G = 238;
            c.B = 144;
            block.Foreground = new SolidColorBrush(c);
            block.Text = text;
            battleTextBox.Children.Add(block);
            battleTextBox.UpdateLayout();
            textScroller.ScrollToVerticalOffset(textScroller.ScrollableHeight);
            textScroller.UpdateLayout();
            this.UpdateLayout();
        }


        public void updateStats(Character ch, Creep cr)
        {
            prgCharHealth.Value = ((double)cr.currentHealth / cr.maxHealth) * 100;
            prgCharMagic.Value = ((double)cr.currentMana / cr.maxMana) * 100;
            settingBar settings = MainPage.currentSettingBar;
            settings.alter_hp(ch.maxHealth, ch.currentHealth);
            settings.alter_mana(ch.maxMana, ch.currentMana);
        }

        public void displayLoot(Item loot)
        {
            lootScreen.addReturnScreen(dungeon);
            lootScreen.update(loot);
            lootScreen.Visibility = Visibility.Visible;
        }

        public void updateAbilities(Character c)
        {
            foreach (Image i in abilityGrid.Children)
            {
                Ability a = i.Tag as Ability;
                if (a.manaCost > c.currentMana)
                    i.Opacity = 0.4;
                else
                    i.Opacity = 1;
            }
        }

        public UIElement Element { get { return this; } }

        private void leaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Character.currentCharacter.restoreCharacter();
            ScreenManager.SetScreen(new Tavern(false, 0));
        }
    }
}
