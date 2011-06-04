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

namespace SilverlightApplication1
{
    public partial class CharacterCreate : ChildWindow
    {
        public CharacterCreate()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void classSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Class c = ClassSet.getClass((ClassType)Enum.Parse(typeof(ClassType), (string)classSelect.SelectedItem, false));
            StatModifier stats = c.initialMod;
            descBlock.Text = c.description;
            string statString = String.Format("Strength: {0}\nAgility: {1}\nIntelligence: {2}", stats.strength, stats.agility,
                                               stats.intelligence);
            statText.Text = statString;
        }
    }
}

