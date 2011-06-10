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
using System.Collections.Generic;

namespace SilverlightApplication1
{

    public class Ability
    {
        public const int initialAbilityLimit = 3;

        public static IDictionary<string, Ability> allAbilities = new Dictionary<string, Ability>();
        public string name { get; set; }
        public string description { get; set; }
        public int manaCost { get; set; }
        public Uri icon { get; set; }
        public AbilityType type { get; set; }

        public Ability(string _name, string _description, int _manacost, Uri _icon, AbilityType _type)
        {
            name = _name;
            description = _description;
            manaCost = _manacost;
            icon = _icon;
            type = _type;
        }

        public static void populateAllAbility()
        {
            allAbilities.Add("Fireball", new Ability("Fireball", "Burns the enemy for massive damage", 50,
                                        new Uri("Images/fireball.png", UriKind.Relative), AbilityType.directdamage));
            allAbilities.Add("Energy arrow", new Ability("Energy arrow", "Ouch", 30,
                                        new Uri("Images/energyarrow.png", UriKind.Relative), AbilityType.directdamage));
            allAbilities.Add("Attack", new Ability("Attack", "Attacks with equipped weapon", 0,
                                        new Uri("Images/attack.png", UriKind.Relative), AbilityType.directdamage));
            allAbilities.Add("Maim", new Ability("Maim", "Injures the enemy with all your might", 10,
                                        new Uri("Images/maim.png", UriKind.Relative), AbilityType.directdamage));             
        }

        public static Ability fetchAbility(string abilityname)
        {
            Ability a;
            allAbilities.TryGetValue(abilityname, out a);
            return a;
        }
    }

    public enum AbilityType
    {
        directdamage, healing, dot
    }
}
