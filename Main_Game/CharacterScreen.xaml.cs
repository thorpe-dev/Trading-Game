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
using System.Windows.Browser;
using System.Threading;
using System.IO;
using System.Windows.Media.Imaging;

namespace Main_Game
{
    public partial class CharacterScreen : UserControl, IScreen
    {
        public CharacterScreen()
        {
            InitializeComponent();
            Ability.populateAllAbility();
            ItemSet.constructItemBase();
            Creep.populateCreeps();
            initialiseClasses();
            HttpConnection.httpGet(new Uri("character.php", UriKind.Relative), new DownloadStringCompletedEventHandler(transferComplete));
        }

        public UIElement Element { get { return this; } }

        private void initialiseClasses()
        {
            IDictionary<string, Ability> warriorAbilities = new Dictionary<string, Ability>();
            warriorAbilities.Add("Maim", Ability.fetchAbility("Maim"));
            ClassSet.createClass(ClassType.Warrior, new Class(new StatModifier(20, 15, 10, 15), new StatModifier(3, 2, 1, 1), ClassType.Warrior,
                                                                "Im a Warrior", new Uri("Images/robot.png", UriKind.Relative),
                                                                warriorAbilities, ItemSet.retrieveItem(100) as Weapon,
                                                                ItemSet.retrieveItem(300) as Armour,
                                                                ItemSet.retrieveItem(200) as Armour,
                                                                ItemSet.retrieveItem(400) as Armour,
                                                                ItemSet.retrieveItem(500) as Armour,
                                                                ItemSet.retrieveItem(600) as Armour));


            IDictionary<string, Ability> mageAbilities = new Dictionary<string, Ability>();
            mageAbilities.Add("Fireball", Ability.fetchAbility("Fireball"));
            mageAbilities.Add("Energy arrow", Ability.fetchAbility("Energy arrow"));
            ClassSet.createClass(ClassType.Mage, new Class(new StatModifier(10, 10, 25, 5), new StatModifier(1, 1, 4, 1), ClassType.Mage,
                                                                "Im a Mage", new Uri("Images/failsprite.png", UriKind.Relative),
                                                                mageAbilities, ItemSet.retrieveItem(101) as Weapon,
                                                                ItemSet.retrieveItem(300) as Armour,
                                                                ItemSet.retrieveItem(200) as Armour,
                                                                ItemSet.retrieveItem(400) as Armour,
                                                                ItemSet.retrieveItem(500) as Armour,
                                                                ItemSet.retrieveItem(600) as Armour));


            IDictionary<string, Ability> rogueAbilities = new Dictionary<string, Ability>();
            rogueAbilities.Add("Attack", Ability.fetchAbility("Attack"));
            ClassSet.createClass(ClassType.Rogue, new Class(new StatModifier(15, 20, 5, 20), new StatModifier(2, 3, 1, 3), ClassType.Rogue,
                                                                "Im a Rogue", new Uri("Images/clam.png", UriKind.Relative),
                                                                 rogueAbilities, ItemSet.retrieveItem(102) as Weapon,
                                                                 ItemSet.retrieveItem(300) as Armour,
                                                                 ItemSet.retrieveItem(200) as Armour,
                                                                 ItemSet.retrieveItem(400) as Armour,
                                                                 ItemSet.retrieveItem(500) as Armour,
                                                                 ItemSet.retrieveItem(600) as Armour));
        }

        private void transferComplete(Object sender, DownloadStringCompletedEventArgs e)
        {
            MessageBox.Show(e.Result);
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
                                             (int)character.Element("money"),
                                             (int)character.Element("strength"),
                                             (int)character.Element("agility"),
                                             (int)character.Element("intelligence"),
                                             (int)character.Element("speed"),
                                             parseAbilities(character.Element("abilities")),
                                             parseInventory(character.Element("inventory")),
                                             parseWeapon(character.Element("weapon")),
                                             parseArmour(character.Element("chest")),
                                             parseArmour(character.Element("helm")),
                                             parseArmour(character.Element("gloves")),
                                             parseArmour(character.Element("boots")),
                                             parseArmour(character.Element("legs"))
                                             );
                        if (characters.LongCount() > 0)
                        {
                            Character c = characters.First();
                            updateCharPane(c);
                        }
                        else
                        {
                            createCharButton.IsEnabled = true;
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

        private IDictionary<string, Ability> parseAbilities(XElement abilities)
        {
            IEnumerable<XElement> abilitySet = abilities.Elements("ability");
            IDictionary<string, Ability> abilityDictionary = new Dictionary<string, Ability>();
            foreach (XElement elem in abilitySet)
            {
                abilityDictionary.Add((string)elem, Ability.fetchAbility((string)elem));
            }
            return abilityDictionary;
        }

        private ICollection<ItemStack> parseInventory(XElement inventoryElements)
        {
            if (inventoryElements.Element("item") != null)
            {
                IEnumerable<XElement> itemSet = inventoryElements.Elements("item");
                ICollection<ItemStack> inventory = new List<ItemStack>();
                foreach (XElement elem in itemSet)
                {
                    Item item = ItemSet.retrieveItem((int)elem.Element("id"));
                    uint amount = (uint)elem.Element("amount");
                    if (elem.Element("level") != null)
                    {
                        if (item is Weapon)
                        {
                            Weapon w = item as Weapon;
                            inventory.Add(new ItemStack(new Weapon(w.id, w, (int)elem.Element("level"))));
                        }
                        else
                        {
                            Armour a = item as Armour;
                            inventory.Add(new ItemStack(new Armour(a.id, a, (int)elem.Element("level"))));
                        }
                    }
                    else
                    {
                        inventory.Add(new ItemStack(item, amount));
                    }
                }
                return inventory;
            }
            return new List<ItemStack>();
        }

        private Weapon parseWeapon(XElement weaponElements)
        {
            if (weaponElements != null)
            {
                int weaponId = (int)weaponElements.Element("id");
                int weaponLevel = (int)weaponElements.Element("level");
                Weapon weapon = ItemSet.retrieveItem(weaponId) as Weapon;
                return new Weapon(weaponId, weapon, weaponLevel);
            }
            return null;
        }

        private Armour parseArmour(XElement armourElements)
        {
            if (armourElements != null)
            {
                int armourId = (int)armourElements.Element("id");
                int armourLevel = (int)armourElements.Element("level");
                Armour armour = ItemSet.retrieveItem(armourId) as Armour;
                return new Armour(armourId, armour, armourLevel);
            }
            return null;
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
            if ((bool)createForm.DialogResult)
            {
                Character c = createForm.createdChar;
                updateCharPane(c);
            }
        }

        private void updateCharPane(Character c)
        {
            string details = String.Format("Name:{0} \nClass:{1} \nLevel:{2}\nHealth:{3}\nMana:{4}\nMoney:{5}\n" +
                                            "Strength:{6} \nAgility:{7} \nIntelligence:{8}\nSpeed:{9}",
                                            c.name, c.type.ToString(), c.level, c.maxHealth,
                                            c.maxMana, c.money, c.strength, c.agility, c.intelligence, c.speed);
            CharacterBox.Text = details;
            Ability[] abilities = c.abilities.Values.ToArray();
            for (int i = 0; i < 5; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(Ability.iconSize);
                abilityGrid.ColumnDefinitions.Add(col);
            }
            for (int i = 0; i <= (abilities.LongCount() - 1) / 5; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(Ability.iconSize);
                abilityGrid.RowDefinitions.Add(row);
            }
            for (int i = 0; i < abilities.LongCount(); i++)
            {
                Ability a = abilities[i];
                Image icon = new Image();
                icon.Source = new BitmapImage(a.icon);
                ToolTip t = new ToolTip();
                t.Background = new SolidColorBrush(Colors.Brown);
                t.Content = a.name + ":\n" + a.description;
                ToolTipService.SetToolTip(icon, t);
                Grid.SetRow(icon, i / 5);
                Grid.SetColumn(icon, i % 5);
                abilityGrid.Children.Add(icon);
            }
            ItemStack[] inventory = c.inventory.ToArray();
            string inventoryString = "";
            for (int i = 0; i < inventory.LongCount(); i++)
            {
                inventoryString += inventory[i].item.name + ": " + inventory[i].stackSize + "\n";
            }
            if (c.weapon != null)
            {
                inventoryString += c.weapon.name + "\n";
            }
            if (c.chest != null)
            {
                inventoryString += c.chest.name + "\n";
            }
            if (c.helm != null)
            {
                inventoryString += c.helm.name + "\n";
            }
            if (c.gloves != null)
            {
                inventoryString += c.gloves.name + "\n";
            }
            if (c.boots != null)
            {
                inventoryString += c.boots.name + "\n";
            }
            if (c.legs != null)
            {
                inventoryString += c.legs.name + "\n";
            }
            inventoryLabel.Content = inventoryString;

            Class _class = c.charClass;
            charImage.Source = new BitmapImage(_class.imageSrc);
            Character.currentCharacter = c;
            createCharButton.IsEnabled = false;
            playCharButton.IsEnabled = true;
            deleteCharButton.IsEnabled = true;
        }

        private void playCharButton_Click(object sender, RoutedEventArgs e)
        {
            HttpConnection.httpGet(new Uri("enterWorld.php", UriKind.Relative), characterLoaded);
        }

        private void characterLoaded(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {        
                
                    ScreenManager.SetScreen(new Tavern(false,0));
            }
            else
            {
                MessageBox.Show("Error: " + e.Error.ToString());
            }
        }

        private void deleteCharButton_Click(object sender, RoutedEventArgs e)
        {
            HttpConnection.httpGet(new Uri("deleteCharacter.php", UriKind.Relative), deleteCharComplete);
        }

        private void deleteCharComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XDocument doc = XDocument.Parse(e.Result);
                if (doc.Element("error") == null)
                {
                    Character.currentCharacter = null;
                    charImage.Source = null;
                    CharacterBox.Text = "";
                    abilityGrid.Children.Clear();
                    inventoryLabel.Content = "";
                    deleteCharButton.IsEnabled = false;
                    playCharButton.IsEnabled = false;
                    createCharButton.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Error: " + (string)doc.Element("error"));
                }
            }
            else
            {
                MessageBox.Show("Error: " + e.Error.ToString());
            }
        }
    }
}