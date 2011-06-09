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
            ClassSet.createClass(ClassType.Warrior, new Class(new StatModifier(20, 15, 10), new StatModifier(3, 2, 1), ClassType.Warrior, "Im a Warrior"));
            ClassSet.createClass(ClassType.Mage, new Class(new StatModifier(10, 10, 25), new StatModifier(1, 1, 4), ClassType.Mage, "Im a Mage"));
            ClassSet.createClass(ClassType.Rogue, new Class(new StatModifier(15, 20, 5), new StatModifier(2, 3, 1), ClassType.Rogue, "Im a Rogue"));
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
                                         select new Character(
                                             (string)character.Element("name"),
                                             ClassSet.getClass((ClassType)((int)character.Element("classid"))),
                                             (int)character.Element("lvl"),
                                             (int)character.Element("exptonext"),
                                             (int)character.Element("maxhealth"),
                                             (int)character.Element("currenthealth"),
                                             (int)character.Element("maxmana"),
                                             (int)character.Element("currentmana"),
                                             (int)character.Element("strength"),
                                             (int)character.Element("agility"),
                                             (int)character.Element("intelligence")
                                             );
                        if (characters.LongCount() > 0)
                        {
                            Character c = characters.First();
                            string details = String.Format("Name:{0} \nClass:{1} \nLevel:{2}\nHealth:{3}\nMana:{4}\n" +
                                                            "Strength:{5} \nAgility:{6} \nIntelligence:{7}",
                                                            c.name, c.type.ToString(), c.level, c.maxHealth,
                                                            c.maxMana, c.strength, c.agility, c.intelligence);
                            CharacterBox.Text = details;
                            Character.currentCharacter = c;
                            createCharButton.IsEnabled = false;
                            playCharButton.IsEnabled = true;
                        }
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
            createForm.Closed += new EventHandler(updateCharPane);
            createForm.Show();
        }

        private void updateCharPane(Object sender, EventArgs e)
        {
            CharacterCreate createForm = (CharacterCreate)sender;
            Character c = createForm.createdChar;
            string details = String.Format("Name:{0} \nClass:{1} \nLevel:{2}\nHealth:{3}\nMana:{4}\n" +
                                                            "Strength:{5} \nAgility:{6} \nIntelligence:{7}",
                                                            c.name, c.type.ToString(), c.level, c.maxHealth,
                                                            c.maxMana, c.strength, c.agility, c.intelligence);
            CharacterBox.Text = details;
            Character.currentCharacter = c;
            createCharButton.IsEnabled = false;
            playCharButton.IsEnabled = true;
        }

        private void playCharButton_Click(object sender, RoutedEventArgs e)
        {
            HttpConnection.httpGet(new Uri("enterWorld.php", UriKind.Relative), characterLoaded);
        }

        private void characterLoaded(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XDocument doc = XDocument.Parse(e.Result);
                if (doc.Element("error") == null)
                {
                    Location l = new Location(LocationType.HomeHub);
                    Location.currentLocation = l;
                    MessageBox.Show("Entering location: " + l.place.ToString());
                }
                else
                {
                    MessageBox.Show((string)doc.Element("error"));
                }
            }
            else
            {
                MessageBox.Show("Error: " + e.Error.ToString());
            }
        }
    }
}
