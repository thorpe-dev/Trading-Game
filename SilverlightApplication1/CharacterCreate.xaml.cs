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

namespace SilverlightApplication1
{
    public partial class CharacterCreate : ChildWindow
    {
        private Character _createdChar;

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
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Class selectedClass = ClassSet.getClass((ClassType)Enum.Parse(typeof(ClassType), (string)classSelect.SelectedItem, false));
            string name = nameBox.Text;
            Character newChar = Character.createNewCharacter(name, selectedClass);
            newChar.submitCharacter(characterTransferComplete);
            _createdChar = newChar;
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
            classImage.Source = new BitmapImage(c.imageSrc);
            statText.Text = statString;
        }

        private void characterTransferComplete(Object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XDocument doc = XDocument.Parse(e.Result);
                if (doc.Element("error") == null)
                {
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show((string) doc.Element("error"));
                }
            }
            else
            {
                MessageBox.Show("ERROR: " + e.Error.ToString());
            }
        }
    }
}

