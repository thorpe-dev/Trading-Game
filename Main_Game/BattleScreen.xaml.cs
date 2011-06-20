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
        Storyboard hp_bar_reduce;
        DoubleAnimation damage;
        Storyboard mana_bar_reduce;
        DoubleAnimation mana_damage;
        Rectangle hp_bar;
        Rectangle mana_bar;
        double bar_width = 135;


        public BattleScreen(Character c, Creep creep, MapScreen _dungeon)
        {
            InitializeComponent();
            battle = new Battle(c, creep, this);
            populateEnemy(creep);
            populateAbilityGrid(c);
            dungeon = _dungeon;

            lbl_creepname.Content = creep.name;

            hp_bar_reduce = new Storyboard();
            damage = new DoubleAnimation();
            damage.Duration = new Duration(TimeSpan.FromSeconds(1));
            hp_bar = new Rectangle();
            hp_bar.Width = bar_width;
            hp_bar.Height = 9;
            Color hp_color = Color.FromArgb(255, 0, 200, 0);
            SolidColorBrush hp_brush = new SolidColorBrush();
            hp_brush.Color = hp_color;
            hp_bar.Fill = hp_brush;
            hp_bar.HorizontalAlignment = rectangle1.HorizontalAlignment;
            hp_bar.VerticalAlignment = rectangle1.VerticalAlignment;
            hp_bar.Margin = new Thickness(255, 86, 0, 0);
            Grid.SetColumn(hp_bar, 1);
            LayoutRoot.Children.Add(hp_bar);

            mana_bar_reduce = new Storyboard();
            mana_damage = new DoubleAnimation();
            mana_damage.Duration = new Duration(TimeSpan.FromSeconds(1));
            mana_bar = new Rectangle();
            mana_bar.Width = bar_width;
            mana_bar.Height = 9;
            hp_color = Color.FromArgb(255, 0, 0, 200);
            hp_brush = new SolidColorBrush();
            hp_brush.Color = hp_color;
            mana_bar.Fill = hp_brush;
            mana_bar.HorizontalAlignment = HorizontalAlignment.Left;
            mana_bar.VerticalAlignment = VerticalAlignment.Top;
            mana_bar.Margin = new Thickness(255,114,0,0);
            Grid.SetColumn(mana_bar, 1);
            LayoutRoot.Children.Add(mana_bar);

            Storyboard.SetTarget(damage, hp_bar);
            Storyboard.SetTargetProperty(damage, new PropertyPath("(Width)"));
            hp_bar_reduce.Children.Add(damage);
            damage.From = bar_width;

            Storyboard.SetTarget(mana_damage, mana_bar);
            Storyboard.SetTargetProperty(mana_damage, new PropertyPath("(Width)"));
            hp_bar_reduce.Children.Add(mana_damage);
            mana_damage.From = bar_width;

            battle.start();
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
            c.R = 0;
            c.G = 0;
            c.B = 0;
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
            alter_hp((double)cr.maxHealth, (double)cr.currentHealth);
            alter_mana((double)cr.maxMana, (double)cr.currentMana);
            settingBar settings = MainPage.currentSettingBar;
            settings.alter_hp(ch.maxHealth, ch.currentHealth);
            settings.alter_mana(ch.maxMana, ch.currentMana);
        }

        public void displayLoot(Item loot)
        {
            LootBox lootBox = new LootBox(loot);
            lootBox.addReturnScreen(dungeon);
            lootBox.Show();
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


        public void alter_hp(double hpmax, double newhp)
        {
            damage.To = (newhp) / hpmax * bar_width;
            hp_bar_reduce.Begin();
            damage.From = hp_bar.Width;
        }

        public void alter_mana(double manamax, double newmana)
        {
            mana_damage.To = (newmana) / manamax * bar_width;
            mana_bar_reduce.Begin();
            mana_damage.From = mana_bar.Width;
        }
    }
}
