using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Main_Game
{
    public abstract class Entity
    {
        public string name { get; set; }
        public int strength { get; set; }
        public int agility { get; set; }
        public int intelligence { get; set; }
        public int speed { get; set; }
        public int maxHealth { get; set; }
        public int currentHealth { get; set; }
        public int maxMana { get; set; }
        public int currentMana { get; set; }
        public Effect buffs { get; set; }
        public IDictionary<string, Ability> abilities { get; set; }
        public D20 dice;

        public void applyEffect(Effect effect)
        {
            buffs.strength_mod *= effect.strength_mod;
            buffs.agility_mod *= effect.agility_mod;
            buffs.intelligence_mod *= effect.intelligence_mod;
            buffs.speed_mod *= effect.speed_mod;

            if (currentHealth + effect.health_restore >= maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth += (int)effect.health_restore;
            }

        }

        public void Attack(Ability move, Entity enemy)
        {
            uint damage = move.attack(this, enemy);
            if ((int)(this.currentHealth - damage) <= 0)
                enemy.currentHealth = 0;
            this.currentHealth -= (int)damage;
        }

        public abstract Ability getAbility();
    }

    public class Creep : Entity
    {

        public int expValue { get; set; }
        public ICollection<Item> lootTable;
        public int moneyDrop;

        public Creep(string _name, int _strength, int _agility, int _intelligence, int _speed, int _maxHealth, int _currentHealth,
                        int _maxMana, int _currentMana, IDictionary<string, Ability> _abilities, ICollection<Item> _lootTable,
                        int _moneyDrop)
        {
            name = _name;
            strength = _strength;
            agility = _agility;
            intelligence = _intelligence;
            speed = _speed;
            maxHealth = _maxHealth;
            currentHealth = _currentHealth;
            maxMana = _maxMana;
            currentMana = _currentMana;
            buffs = new Effect();
            abilities = new Dictionary<string, Ability>(_abilities);
            lootTable = new List<Item>(_lootTable);
            moneyDrop = _moneyDrop;
            dice = new D20();
        }

        public override Ability getAbility()
        {
            Random rnd = new Random();
            Ability[] abilityArray = abilities.Values.ToArray();
            return abilityArray[rnd.Next(0, abilityArray.Count())];
        }


    }

    public class Character : Entity
    {
        public const int BASEHEALTH = 0;
        public const int BASEMANA = 0;
        public const int INVENTORYSIZE = 20;
        public static Character currentCharacter { get; set; }

        public Class charClass { get; set; }
        public int level { get; set; }
        public int expToNext { get; set; }
        public int money { get; set; }

        public ClassType type { get; set; }
        public ICollection<ItemStack> inventory { get; set; }
        public Weapon weapon { get; set; }
        public Armour chest { get; set; }
        public Armour helm { get; set; }
        public Armour gloves { get; set; }
        public Armour boots { get; set; }
        public Armour legs { get; set; }

        public Character(string _name, Class _class, int _level, int _expToNext, int _maxHealth, int _currentHealth, 
                            int _maxMana, int _currentMana, int _money, int _strength, int _agility, int _intelligence,
                                int _speed, IDictionary<string, Ability> _abilities, ICollection<ItemStack> _inventory, Weapon _weapon,
                                Armour _chest, Armour _helm, Armour _gloves, Armour _boots, Armour _legs)
        {
            name = _name;
            charClass = _class;
            level = _level;
            expToNext = _expToNext;
            maxHealth = _maxHealth;
            currentHealth = _currentHealth;
            maxMana = _maxMana;
            currentMana = _currentMana;
            money = _money;
            strength = _strength;
            agility = _agility;
            intelligence = _intelligence;
            speed = _speed;
            type = _class.type;
            abilities = new Dictionary<string, Ability>(_abilities);
            inventory = new List<ItemStack>(_inventory);
            weapon = _weapon;
            chest = _chest;
            helm = _helm;
            gloves = _gloves;
            boots = _boots;
            legs = _legs;
            dice = new D20();
            buffs = new Effect();
        }

        public static Character createNewCharacter(string _name, Class _class, IDictionary<string, Ability> _abilities)
        {
            StatModifier mod = _class.initialMod;
            int _maxHealth = calculateMaxHealth(mod.strength);
            int _maxMana = calculateMaxMana(mod.intelligence);
            return new Character(_name, _class, 1, calculateExpToNextLevel(1), _maxHealth, _maxHealth, _maxMana, 
                                    _maxMana, 100, mod.strength, mod.agility, mod.intelligence, mod.speed, _abilities, initialItems(), 
                                    _class.startingWeapon,
                                    _class.startingChest,
                                    _class.startingHelm,
                                    _class.startingGloves,
                                    _class.startingBoots,
                                    _class.startingLegs);
        }

        private static ICollection<ItemStack> initialItems()
        {
            ICollection<ItemStack> startingInventory = new List<ItemStack>((int)ItemStack.MAXSTACKSIZE);
            Consumable minorHealthPot = (Consumable) ItemSet.retrieveItem(1);
            startingInventory.Add(new ItemStack(minorHealthPot, 3));
            return startingInventory;
        }

        public void submitCharacter(UploadStringCompletedEventHandler characterUploaded)
        {
            string charFormatString = String.Format("name={0}&classid={1}&lvl=1&exptonext={2}&maxhealth={3}&currenthealth={3}&" +
                                                    "maxmana={4}&currentmana={4}&money={5}&strength={6}&agility={7}&intelligence={8}&" +
                                                    "speed={9}",
                                                    name, (int)type, expToNext, maxHealth,
                                                    maxMana, money, strength, agility, intelligence, speed);
            charFormatString += formatAbilities(abilities) + formatItems(inventory) + formatWeapon() + formatChest() + formatHelm()
                                + formatGloves() + formatBoots() + formatLegs();
            HttpConnection.httpPost(new Uri("characterCreate.php", UriKind.Relative), charFormatString, characterUploaded);
        }

        private string formatAbilities(IDictionary<string, Ability> abilityList)
        {
            Ability[] abilityArray = abilityList.Values.ToArray();
            string formatString = "";
            for (int i = 0; i < abilityArray.LongCount(); i++)
            {
                formatString += "&ability" + i + "=" + abilityArray[i].name;
            }
            return formatString;
        }

        private string formatItems(ICollection<ItemStack> inventory)
        {
            ItemStack[] itemArray = inventory.ToArray();
            string formatString = "";
            for (int i = 0; i < itemArray.LongCount(); i++)
            {
                formatString += "&itemid" + i + "=" + itemArray[i].item.id + "&itemcount" + i + "=" + itemArray[i].stackSize;
            }
            return formatString;
        }

        private string formatWeapon()
        {
            return "&weaponid=" + weapon.id + "&weaponlevel=" + weapon.level;
        }

        private string formatChest()
        {
            return "&chestid=" + chest.id + "&chestlevel=" + chest.level; 
        }

        private string formatHelm()
        {
            return "&helmid=" + helm.id + "&helmlevel=" + helm.level;
        }

        private string formatGloves()
        {
            return "&glovesid=" + gloves.id + "&gloveslevel=" + gloves.level;
        }

        private string formatBoots()
        {
            return "&bootsid=" + boots.id + "&bootslevel=" + boots.level;
        }

        private string formatLegs()
        {
            return "&legsid=" + legs.id + "&legslevel=" + legs.level;
        }

        public void calculateStats()
        {
            int baseStrength = charClass.initialMod.strength + level * charClass.levelMod.strength;
            int baseInt = charClass.initialMod.intelligence + level * charClass.levelMod.intelligence;
            int baseAgi = charClass.initialMod.agility + level * charClass.levelMod.agility;
            int baseSpeed = charClass.initialMod.speed + level * charClass.levelMod.speed;

            int itemStrength = weapon.effect.strengthMod + chest.stats.strengthMod + helm.stats.strengthMod
                                + gloves.stats.strengthMod + boots.stats.strengthMod + legs.stats.strengthMod;
            int itemAgility = weapon.effect.agilityMod + chest.stats.agilityMod + helm.stats.agilityMod
                                + gloves.stats.agilityMod + boots.stats.agilityMod + legs.stats.agilityMod;
            int itemIntelligence = weapon.effect.intelligenceMod + chest.stats.intelligenceMod + helm.stats.intelligenceMod
                                + gloves.stats.intelligenceMod + boots.stats.intelligenceMod + legs.stats.intelligenceMod;
            int itemSpeed = weapon.effect.speedMod + chest.stats.speedMod + helm.stats.speedMod
                                + gloves.stats.speedMod + boots.stats.speedMod + legs.stats.speedMod;

            strength = baseStrength + itemStrength;
            agility = baseAgi + itemAgility;
            intelligence = baseInt + itemIntelligence;
            speed = baseSpeed + itemSpeed;

            int itemHealth = weapon.effect.healthMod + chest.stats.healthMod + helm.stats.healthMod
                                + gloves.stats.healthMod + boots.stats.healthMod + legs.stats.healthMod;
            int itemMana = weapon.effect.manaMod + chest.stats.manaMod + helm.stats.manaMod
                                + gloves.stats.manaMod + boots.stats.manaMod + legs.stats.manaMod;

            maxHealth = calculateMaxHealth(strength) + itemHealth;
            maxMana = calculateMaxMana(intelligence) + itemMana;
        }

        public void levelUp()
        {
            level++;
            calculateStats();
            currentHealth = maxHealth;
            currentMana = maxMana;
            expToNext = calculateExpToNextLevel(level);
        }

        public void resetStats()
        {
            buffs =  new Effect();
        }

        public override Ability getAbility()
        {
            throw new NotImplementedException();
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
