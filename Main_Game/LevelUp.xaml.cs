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
    public partial class LevelWindow : ChildWindow
    {
        private ICollection<Ability> selectedAbilities;
        private IDictionary<string, Ability> availableAbilities;
        private Character c;

        public LevelWindow(Character _c)
        {
            InitializeComponent();
            c = _c;
            levelLabel.Content = "Congratulations you are now level " + c.level + "\n";
            levelLabel.FontSize = 14;
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(Ability.iconSize);
            abilityGrid.RowDefinitions.Add(row);
            availableAbilities = new Dictionary<string, Ability>(c.charClass.abilities);
            foreach (Ability a in c.abilities.Values)
            {
                availableAbilities.Remove(a.name);
            }
            if (availableAbilities.Count != 0)
            {
                levelLabel.Content += "Please select an ability";
            }
            Ability[] abilities = availableAbilities.Values.ToArray();
            for (int n = 0; n < abilities.Length; n++)
            {
                Ability a = abilities[n];
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(Ability.iconSize);
                abilityGrid.ColumnDefinitions.Add(col);
                Image icon = new Image();
                icon.Source = new BitmapImage(a.icon);
                icon.Opacity = 0.6;
                icon.Tag = a;
                icon.MouseLeftButtonDown += new MouseButtonEventHandler(select_Ability);
                ToolTip t = new ToolTip();
                t.Background = new SolidColorBrush(Colors.Brown);
                t.Content = a.name + "  Mana cost:" + a.manaCost + "\n" + a.description;
                ToolTipService.SetToolTip(icon, t);
                Grid.SetRow(icon, 0);
                Grid.SetColumn(icon, n);
                abilityGrid.Children.Add(icon);
            }
            selectedAbilities = new List<Ability>();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (availableAbilities.Count == 0 || selectedAbilities.Count == Ability.levelAbilityLimit)
            {
                foreach (Ability a in selectedAbilities)
                {
                    c.abilities.Add(a.name, a);
                }

                this.DialogResult = true;
            }
            else
            {
                errorLabel.Content = "Please select a ability";
            }
        }

        private void select_Ability(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            Ability a = img.Tag as Ability;
            if (!selectedAbilities.Contains(a))
            {
                if (selectedAbilities.Count < Ability.levelAbilityLimit)
                {
                    selectedAbilities.Add(a);
                    img.Opacity = 1;
                }
            }
            else
            {
                selectedAbilities.Remove(a);
                img.Opacity = 0.6;
            }
        }
    }
}

