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
using System.Collections.ObjectModel;
using System.Linq;

namespace Main_Game
{
    public static class ClassSet
    {
        private static IDictionary<ClassType, Class> _classes = new Dictionary<ClassType, Class>();

        public static void createClass(ClassType type, Class _class)
        {
            _classes.Add(type, _class);
        }

        public static Class getClass(ClassType type)
        {
            Class c;
            bool success = _classes.TryGetValue(type, out c);
            if (!success)
                throw new NoSuchClassException("No such class has been found");
            else
                return c;
        }

        public static IDictionary<ClassType, Class> classes
        {
            get
            {
                return _classes;
            }
        }

    }

    public class NoSuchClassException : Exception
    {
        public NoSuchClassException() { }
        public NoSuchClassException(string msg) { }
    }

    public class Class
    {
        private StatModifier initialModifier;
        private StatModifier levelModifier;
        private ClassType _type;
        private Uri imageSource;
        private string _description;
        public Weapon startingWeapon { get; set; }
        public Armour startingHelm { get; set; }
        public Armour startingChest { get; set; }
        public Armour startingGloves { get; set; }
        public Armour startingBoots { get; set; }
        public Armour startingLegs { get; set; }
        public IDictionary<string, Ability> abilities;

        public Class(StatModifier initialModifier, StatModifier levelModifier, ClassType ctype, string description, Uri imageSource,
                        IDictionary<string, Ability> _abilities, Weapon _startingWeapon, Armour _startingHelm, Armour _startingChest,
                        Armour _startingGloves, Armour _startingBoots, Armour _startingLegs)
        {
            this.initialModifier = initialModifier;
            this.levelModifier = levelModifier;
            _type = ctype;
            _description = description;
            this.imageSource = imageSource;
            abilities = _abilities;
            startingWeapon = _startingWeapon;
            startingHelm = _startingHelm;
            startingChest = _startingChest;
            startingGloves = _startingGloves;
            startingBoots = _startingBoots;
            startingLegs = _startingLegs;
        }

        public StatModifier levelMod
        {
            get
            {
                return levelModifier;
            }
        }

        public StatModifier initialMod
        {
            get
            {
                return initialModifier;
            }
        }

        public ClassType type
        {
            get
            {
                return _type;
            }
        }

        public string description
        {
            get
            {
                return _description;
            }
        }

        public Uri imageSrc
        {
            get
            {
                return imageSource;
            }
        }
    }

    public enum ClassType
    {
        Brute, Bard, Sorcerer
    }

    public class StringClasses
    {
        public string[] classes { get; set; }

        public StringClasses()
        {
            var type = typeof(ClassType);
            classes = (from item in type.GetFields()
                       where item.IsLiteral
                       select item.Name).ToArray();
        }
    }

    public class StatModifier
    {
        private int _strength;
        private int _agility;
        private int _intelligence;
        private int _speed;

        public StatModifier(int strength, int agility, int intelligence, int speed)
        {
            this._strength = strength;
            this._agility = agility;
            this._intelligence = intelligence;
            this._speed = speed;
        }

        public int strength
        {
            get
            {
                return _strength;
            }
        }

        public int agility
        {
            get
            {
                return _agility;
            }
        }

        public int intelligence
        {
            get
            {
                return _intelligence;
            }
        }

        public int speed
        {
            get
            {
                return _speed;
            }
        }
    }
}
