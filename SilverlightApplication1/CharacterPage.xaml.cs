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
using System.Windows.Navigation;
using System.Xml.Linq;
using System.Windows.Browser;
using System.Threading;
using System.IO;

namespace SilverlightApplication1
{
    public partial class CharacterPage : Page
    {
        public CharacterPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            initialiseClasses();
            HttpConnection.httpGet(new Uri("character.php", UriKind.Relative), new DownloadStringCompletedEventHandler(transferComplete));
        }

        private void initialiseClasses()
        {
            ClassSet.createClass(ClassType.Warrior, new Class(new StatModifier(20, 15, 10), new StatModifier(3, 2, 1), "Im a Warrior"));
            ClassSet.createClass(ClassType.Mage, new Class(new StatModifier(10, 10, 25), new StatModifier(1, 1, 4), "Im a Mage"));
            ClassSet.createClass(ClassType.Rogue, new Class(new StatModifier(15, 20, 5), new StatModifier(2, 3, 1), "Im a Rogue"));
        }

        private void transferComplete(Object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XDocument doc = XDocument.Parse(e.Result);
                if (doc.Element("error") == null)
                {
                    try
                    {
                        var characters = from character in doc.Descendants("character")
                                         select new Character()
                                         {
                                             name = (string)character.Element("name"),
                                             classid = (ClassType)((int)character.Element("classid")),
                                             lvl = (int)character.Element("lvl"),
                                             exptonext = (int)character.Element("exptonext"),
                                             strength = (int)character.Element("strength"),
                                             agility = (int)character.Element("agility"),
                                             intelligence = (int)character.Element("intelligence")
                                         };
                        Character c = characters.First();
                        string details = String.Format("Name:{0} \nClass:{1} \nLevel:{2}\nStrength:{3} \nAgility:{4} \nIntelligence:{5}",
                          c.name, c.classid.ToString(), c.lvl, c.strength, c.agility, c.intelligence);
                        CharacterBox.Text = details;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("ERROR: " + doc.Element("error"));
                }
            }
            else
            {
                MessageBox.Show("ERROR: " + e.Error.ToString());
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            CharacterCreate createForm = new CharacterCreate();
            createForm.Show();
        }
    }

    public class Character
    {
        public string name;
        public ClassType classid;
        public int lvl;
        public int exptonext;
        public int strength;
        public int agility;
        public int intelligence;
    }
}
