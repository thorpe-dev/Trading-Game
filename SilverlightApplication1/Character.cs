using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightApplication1
{
    public class Character
    {
        public const int BASEHEALTH = 0;
        public const int BASEMANA = 0;
        public static Character currentCharacter { get; set; }


        public string name { get; set; }
        public Class charClass { get; set; }
        public int level { get; set; }
        public int expToNext { get; set; }
        public int maxHealth { get; set; }
        public int currentHealth { get; set; }
        public int maxMana { get; set; }
        public int currentMana { get; set; }
        public int strength { get; set; }
        public int agility { get; set; }
        public int intelligence { get; set; }
        public ClassType type { get; set; }

        public Character(string _name, Class _class, int _level, int _expToNext, int _maxHealth, int _currentHealth, 
                            int _maxMana, int _currentMana, int _strength, int _agility, int _intelligence)
        {
            name = _name;
            charClass = _class;
            level = _level;
            expToNext = _expToNext;
            maxHealth = _maxHealth;
            currentHealth = _currentHealth;
            maxMana = _maxMana;
            currentMana = _currentMana;
            strength = _strength;
            agility = _agility;
            intelligence = _intelligence;
            type = _class.type;
        }

        public static Character createNewCharacter(string _name, Class _class)
        {
            StatModifier mod = _class.initialMod;
            int _maxHealth = calculateMaxHealth(mod.strength);
            int _maxMana = calculateMaxMana(mod.intelligence);
            return new Character(_name, _class, 1, calculateExpToNextLevel(1), _maxHealth, _maxHealth, _maxMana, 
                                    _maxMana, mod.strength, mod.agility, mod.intelligence);
        }

        public void submitCharacter(UploadStringCompletedEventHandler characterUploaded)
        {
            string charFormatString = String.Format("name={0}&classid={1}&lvl=1&exptonext={2}&maxhealth={3}&currenthealth={3}&" +
                                                    "maxmana={4}&currentmana={4}&strength={5}&agility={6}&intelligence={7}",
                                                    name, (int)type, expToNext, maxHealth,
                                                    maxMana, strength, agility, intelligence);
            HttpConnection.httpPost(new Uri("characterCreate.php", UriKind.Relative), charFormatString, characterUploaded);
        }

        public static int calculateMaxHealth(int _strength)
        {
            return BASEHEALTH + 20 * _strength;        
        }

        public static int calculateMaxMana(int _intelligence)
        {
            return BASEMANA + 30 * _intelligence;
        }

        public static int calculateExpToNextLevel(int level)
        {
            return level * 100;
        }


    }
}
