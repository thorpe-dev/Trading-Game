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
            HttpConnection.httpGet(new Uri("character.php", UriKind.Relative), new DownloadStringCompletedEventHandler(transferComplete));
        }

        private void transferComplete(Object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XDocument doc = XDocument.Parse(e.Result);
                if (doc.Element("error") == null)
                {
                    var characters = from character in doc.Descendants("character")
                                     select new Character()
                                     {
                                         name = (string)character.Element("name"),
                                         classid = (int)character.Element("classid"),
                                         lvl = (int)character.Element("lvl"),
                                         exptonext = (int)character.Element("exptonext"),
                                         strength = (int)character.Element("strength"),
                                         agility = (int)character.Element("agility"),
                                         intelligence = (int)character.Element("intelligence")
                                     };
                    Character c = characters.First();
                    string details = String.Format("Name:{0} \nClass:{1} \nLevel:{2}\nExperience to next level:{3}\nStrength:{4} \nAgility:{5} \nIntelligence:{6}",
                      c.name, c.classid, c.lvl, c.exptonext, c.strength, c.agility, c.intelligence);
                    CharacterBox.Text = details;
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
    }

    public class Character
    {
        public string name;
        public int classid;
        public int lvl;
        public int exptonext;
        public int strength;
        public int agility;
        public int intelligence;
    }
}
