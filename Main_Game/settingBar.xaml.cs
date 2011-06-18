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
    public partial class settingBar : UserControl, IScreen
    {
        Storyboard hp_bar_reduce;
        DoubleAnimation damage;
        Storyboard mana_bar_reduce;
        DoubleAnimation mana_damage;
        Rectangle hp_bar;
        Rectangle mana_bar;
        double bar_width = 135;
        double hp_max = 200;
        double hp_current = 200;
        double mana_max = 200;
        double mana_current = 200;

        public settingBar()
        {
            InitializeComponent();
            hp_bar_reduce = new Storyboard();
            damage = new DoubleAnimation();
            damage.Duration = new Duration(TimeSpan.FromSeconds(1));
            hp_bar = new Rectangle();
            hp_bar.Width = bar_width;
            hp_bar.Height = 9;
            Color hp_color = Color.FromArgb(255,0,200,0);
            SolidColorBrush hp_brush = new SolidColorBrush();
            hp_brush.Color = hp_color;
            hp_bar.Fill = hp_brush;
            hp_bar.HorizontalAlignment = HorizontalAlignment.Left;
            hp_bar.VerticalAlignment = VerticalAlignment.Top;
            hp_bar.Margin = new Thickness(436, 13, 0, 0);
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
            mana_bar.Margin = new Thickness(436, 47, 0, 0);
            LayoutRoot.Children.Add(mana_bar);

            Storyboard.SetTarget(damage,hp_bar);
            Storyboard.SetTargetProperty(damage, new PropertyPath("(Width)"));
            hp_bar_reduce.Children.Add(damage);
            damage.From = bar_width;

            Storyboard.SetTarget(mana_damage, mana_bar);
            Storyboard.SetTargetProperty(mana_damage, new PropertyPath("(Width)"));
            hp_bar_reduce.Children.Add(mana_damage);
            mana_damage.From = bar_width;
        }

        public UIElement Element { get { return this; } }

        private void img_messages_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Messages m = new Messages();
            m.Show();
            img_messages.Source = new BitmapImage(new Uri("Images/mail.png", UriKind.Relative));
        }

        private void alter_hp(double hpmax, double newhp)
        {
            damage.To = (newhp) / hpmax * bar_width;
            hp_bar_reduce.Begin();
            damage.From = hp_bar.Width;
        }

        private void alter_mana(double manamax, double newmana)
        {
            mana_damage.To = (newmana) / manamax * bar_width;
            mana_bar_reduce.Begin();
            mana_damage.From = mana_bar.Width;
        }

        private void btn_hit_Click(object sender, RoutedEventArgs e)
        {/*
            try
            {
                hp_current = (hp_current - Int32.Parse(txt_damage.Text));
                if (hp_current < 0)
                {
                    hp_current = 0;
                }
                damage.To = (hp_current) / hp_max * bar_width;
                hp_bar_reduce.Begin();
                damage.From = hp_bar.Width;
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }*/
            alter_hp(200, 140);
            alter_mana(700, 369);
        }

        private void btn_restore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                hp_current = (hp_current + Int32.Parse(txt_damage.Text));
                if (hp_current > hp_max)
                {
                    hp_current = hp_max;
                }
                if ((hp_current / hp_max *bar_width) > bar_width)
                {
                    damage.To = bar_width;
                }
                else
                {
                    damage.To = (hp_current) / hp_max * bar_width;
                }
                hp_bar_reduce.Begin();
                damage.From = hp_bar.Width;
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
        }
    }
}
