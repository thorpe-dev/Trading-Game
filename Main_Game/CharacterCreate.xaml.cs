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
using System.Xml.Linq;
using System.Windows.Media.Imaging;

namespace Main_Game
{
    public partial class CharacterCreate : ChildWindow
    {
        private Character _createdChar;
        private IDictionary<string, Ability> selectedAbilities;
        private int abilitiesToSelect;

        public Character createdChar
        {
            get
            {
                return _createdChar;
            }
        }

        public CharacterCreate()
        {
            InitializeComponent();
            selectedAbilities = new Dictionary<string, Ability>();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            b.IsEnabled = false;
            string name = nameBox.Text;
            if (name.Equals(""))
            {
                MessageBox.Show("Please enter a character name");
                b.IsEnabled = true;
                return;
            }
            if (name.Contains(';') || name.Contains(' ') || name.Contains('\\') || name.Contains('\n') || name.Contains('\t'))
            {
                MessageBox.Show("Character name contains invalid characters");
                b.IsEnabled = true;
                return;
            }
            if (classSelect.SelectedItem.Equals(null))
            {
                MessageBox.Show("Please select a class");
                b.IsEnabled = true;
                return;
            }
            if (abilitiesToSelect != 0)
            {
                MessageBox.Show("Please select " + Ability.initialAbilityLimit + " abilities");
                b.IsEnabled = true;
                return;
            }
            Class selectedClass = ClassSet.getClass((ClassType)Enum.Parse(typeof(ClassType), (string)classSelect.SelectedItem, false));
            Character newChar = Character.createNewCharacter(name, selectedClass, selectedAbilities);
            newChar.calculateStats();
            newChar.submitCharacter(characterTransferComplete);
            _createdChar = newChar;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void classSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            abilityGrid.Children.Clear();
            Class c = ClassSet.getClass((ClassType)Enum.Parse(typeof(ClassType), (string)classSelect.SelectedItem, false));
            StatModifier stats = c.initialMod;
            descBlock.Text = c.description;
            string statString = String.Format("Strength: {0}\nAgility: {1}\nIntelligence: {2}", stats.strength, stats.agility,
                                               stats.intelligence);
            classImage.Source = new BitmapImage(c.imageSrc);
            statText.Text = statString;
            int n = 0;
            foreach (Ability a in c.abilities.Values)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(Ability.iconSize);
                abilityGrid.ColumnDefinitions.Add(column);
                Image icon = new Image();
                icon.Tag = a;
                icon.Source = new BitmapImage(a.icon);
                icon.Opacity = 0.6;
                icon.MouseLeftButtonDown += new MouseButtonEventHandler(selectAbility);
                ToolTip t = new ToolTip();
                t.Background = new SolidColorBrush(Colors.Brown);
                t.Content = a.name + "  Mana cost:" + a.manaCost + "\n" + a.description;
                ToolTipService.SetToolTip(icon, t);
                Grid.SetRow(icon, 0);
                Grid.SetColumn(icon, n);
                abilityGrid.Children.Add(icon);
                n++;
            }
            abilitiesToSelect = Ability.initialAbilityLimit;
            choicesLabel.Content = abilitiesToSelect.ToString();
        }

        private void selectAbility(Object sender, MouseEventArgs e)
        {
            Image icon = sender as Image;

            if (icon.Opacity != 1)
            {
                if (abilitiesToSelect <= Ability.initialAbilityLimit)
                {
                    abilitiesToSelect--;
                    icon.Opacity = 1;
                    Ability a = icon.Tag as Ability;
                    selectedAbilities.Add(a.name, a);
                }
            }
            else
            {
                abilitiesToSelect++;
                icon.Opacity = 0.6;
                selectedAbilities.Remove(((Ability)icon.Tag).name);
            }
            choicesLabel.Content = abilitiesToSelect.ToString();
        }

        private void characterTransferComplete(Object sender, UploadStringCompletedEventArgs e)
        {
            MessageBox.Show(e.Result);
            if (e.Error == null)
            {
                XDocument doc = XDocument.Parse(e.Result);
                if (doc.Element("error") == null)
                {
                    selectedAbilities.Clear();
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show((string)doc.Element("error"));
                }
            }
            else
            {
                MessageBox.Show("ERROR: " + e.Error.ToString());
            }
        }
    }
}

