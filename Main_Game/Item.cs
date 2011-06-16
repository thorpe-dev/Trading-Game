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
using System.Linq;

namespace Main_Game
{
    public static class ItemSet
    {
        public const double levelMultipler = 0.2;

        private static IDictionary<int, Item> allItems = new Dictionary<int, Item>();

        public static void addItem(Item i)
        {
            allItems.Add(i.id, i);
        }

        public static Item retrieveItem(int id)
        {
            Item i;
            allItems.TryGetValue(id, out i);
            return i;
        }

        public static void constructItemBase()
        {
            Consumable healthPot;
            Consumable manaPot;
            for (uint i = 1; i <= Consumable.consumablePrefixes.LongCount(); i++)
            {
                string prefix = Consumable.consumablePrefixes[i-1];
                healthPot = new Consumable((int)i, prefix + "healing potion", "Regenerates health", Consumable.healthPotionBaseValue * i,
                                ConsumableType.health, (int)(Consumable.healthPotionBaseRegen * i));
                ItemSet.addItem(healthPot);
                manaPot = new Consumable((int)(i + Consumable.consumablePrefixes.LongCount()), prefix + "mana potion", 
                                "Regenerates mana", Consumable.manaPotionBaseValue * i,
                                ConsumableType.mana, (int)(Consumable.manaPotionBaseRegen * i));
                ItemSet.addItem(manaPot);
            }
            Weapon.populateWeaponTypeEffects();
            Weapon w = new Weapon(101, "Magic stick", "A magical staff", 50, WeaponType.STAFF, 1);
            ItemSet.addItem(w);
            w = new Weapon(100, "Broadsword", "Large two handed sword", 50, WeaponType.TWOHANDEDSWORD, 1);
            ItemSet.addItem(w);
            w = new Weapon(102, "Shortsword", "A short blade", 50, WeaponType.ONEHANDEDSWORD, 1);
            ItemSet.addItem(w);
            Armour a = new Armour(200, "Long robe", "A cotton robe", 50, ArmourType.CHEST,
                                   new EquipmentEffect(0, 0, 10, 0,  0, 50), 1);
            ItemSet.addItem(a);
            a = new Armour(300, "Soft hood", "A nice hood", 30, ArmourType.HELM,
                                   new EquipmentEffect(-2, 0, 5, 0, 0, 20), 1);
            ItemSet.addItem(a);
            a = new Armour(400, "Woven gloves", "Handknitted gloves", 20, ArmourType.GLOVES,
                                  new EquipmentEffect(-1, 0, 4, 0, 0, 15), 1);
            ItemSet.addItem(a);
            a = new Armour(500, "Mystic treads", "Very reliable pair of shoes", 20, ArmourType.BOOTS,
                                  new EquipmentEffect(-2, 1, 3, 0, 0, 15), 1);
            ItemSet.addItem(a);

            a = new Armour(600, "Damp britches", "These have seen better days", 20, ArmourType.LEGS,
                                  new EquipmentEffect(-1, 0, 2, 0, 0, 10), 1);
            ItemSet.addItem(a);
        }
    }

    public class ItemStack
    {
        public const uint MAXSTACKSIZE = 5;

        public Item item { get; set; }
        public uint stackSize { get; set; }

        public ItemStack(Item _item)
        {
            item = _item;
            stackSize = 1;
        }

        public ItemStack(Item _item, uint numberOfItems)
        {
            item = _item;
            stackSize = numberOfItems;
        }

        public void dropStack(ICollection<ItemStack> inventory)
        {
            inventory.Remove(this);
        }

        public void dropItem(ICollection<ItemStack> inventory)
        {
            if (stackSize > 1)
            {
                stackSize--;
            }
            else
            {
                inventory.Remove(this);
            }
        }

        public bool useItem(Character c, bool inBattle)
        {
            if (item is Consumable)
            {
                Consumable potion = item as Consumable;
                potion.consume(c);
                if (stackSize > 1)
                {
                    stackSize--;
                }
                else
                {
                    c.inventory.Remove(this);
                }
                return true;
            }
            if (!inBattle && item is Weapon)
            {
                Weapon weapon = item as Weapon;
                if (c.weapon == null)
                {

                    c.weapon = weapon;
                    c.inventory.Remove(this);
                    return true;
                }
                else
                {
                    ICollection<ItemStack> inventory = c.inventory;
                    inventory.Remove(this);
                    ItemStack oldWeapon = new ItemStack(c.weapon);
                    inventory.Add(oldWeapon);
                    c.weapon = weapon;
                    return true;
                }
            }
            if (!inBattle && item is Armour)
            {
                Armour armour = item as Armour;
                switch (armour.type)
                {
                    case (ArmourType.CHEST):
                        {
                            if (c.chest == null)
                            {

                                c.chest = armour;
                                c.inventory.Remove(this);
                                return true;
                            }
                            else
                            {
                                ICollection<ItemStack> inventory = c.inventory;
                                inventory.Remove(this);
                                ItemStack oldChest = new ItemStack(c.chest);
                                inventory.Add(oldChest);
                                c.chest = armour;
                                return true;
                            }
                        }
                    case (ArmourType.HELM):
                        {
                            if (c.helm == null)
                            {

                                c.helm = armour;
                                c.inventory.Remove(this);
                                return true;
                            }
                            else
                            {
                                ICollection<ItemStack> inventory = c.inventory;
                                inventory.Remove(this);
                                ItemStack oldHelm = new ItemStack(c.helm);
                                inventory.Add(oldHelm);
                                c.helm = armour;
                                return true;
                            }
                        }
                    case (ArmourType.GLOVES):
                        {
                            if (c.gloves == null)
                            {

                                c.gloves = armour;
                                c.inventory.Remove(this);
                                return true;
                            }
                            else
                            {
                                ICollection<ItemStack> inventory = c.inventory;
                                inventory.Remove(this);
                                ItemStack oldgloves = new ItemStack(c.gloves);
                                inventory.Add(oldgloves);
                                c.gloves = armour;
                                return true;
                            }
                        }
                    case (ArmourType.BOOTS):
                        {
                            if (c.boots == null)
                            {

                                c.boots = armour;
                                c.inventory.Remove(this);
                                return true;
                            }
                            else
                            {
                                ICollection<ItemStack> inventory = c.inventory;
                                inventory.Remove(this);
                                ItemStack oldboots = new ItemStack(c.boots);
                                inventory.Add(oldboots);
                                c.boots = armour;
                                return true;
                            }
                        }
                    case (ArmourType.LEGS):
                        {
                            if (c.legs == null)
                            {

                                c.legs = armour;
                                c.inventory.Remove(this);
                                return true;
                            }
                            else
                            {
                                ICollection<ItemStack> inventory = c.inventory;
                                inventory.Remove(this);
                                ItemStack oldlegs = new ItemStack(c.legs);
                                inventory.Add(oldlegs);
                                c.legs = armour;
                                return true;
                            }
                        }
                }
            }
            return false;
        }
    }

    public abstract class Item
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public uint value { get; set; }
        public bool stackable { get; set; }

        public bool loot(ICollection<ItemStack> inventory)
        {
            if (stackable)
            {
                IEnumerable<ItemStack> stacks = inventory.Where(retrieveNonFullItemStack);
                if (stacks.LongCount() > 0)
                {
                    ItemStack stack = stacks.First();
                    stack.stackSize++;
                    return true;
                }
            }
            if (inventory.Count < Character.INVENTORYSIZE)
            {
                inventory.Add(new ItemStack(this));
                return true;
            }
            return false;
        }

        private bool retrieveNonFullItemStack(ItemStack stack)
        {
            return (stack.item.name.Equals(name) && stack.stackSize < ItemStack.MAXSTACKSIZE);
        }
    }

    public class Consumable : Item
    {
        public const uint healthPotionBaseValue = 50;
        public const int healthPotionBaseRegen = 200;
        public const uint manaPotionBaseValue = 40;
        public const int manaPotionBaseRegen = 300;

        public static string[] consumablePrefixes = { "minor ", "", "greater " };

        public ConsumableType type { get; set; }
        public int amountRegenerated;

        public Consumable(int _id, string _name, string _description, uint _value, ConsumableType _type, int _amountRegenerated)
        {
            id = _id;
            name = _name;
            description = _description;
            value = _value;
            type = _type;
            amountRegenerated = _amountRegenerated;
            stackable = true;
        }

        public void consume(Character c)
        {
            if (type == ConsumableType.health)
            {
                if (c.currentHealth + amountRegenerated >= c.maxHealth)
                {
                    c.currentHealth = c.maxHealth;
                }
                else
                {
                    c.currentHealth += amountRegenerated;
                }
            }
            else if (type == ConsumableType.mana)
            {
                if (c.currentMana + amountRegenerated >= c.maxMana)
                {
                    c.currentMana = c.maxMana;
                }
                else
                {
                    c.currentMana += amountRegenerated;
                }
            }
        }
    }

    public enum ConsumableType { health, mana }

    public class Weapon : Item
    {
        public static IDictionary<WeaponType, EquipmentEffect> weaponTypeEffects = new Dictionary<WeaponType, EquipmentEffect>();

        public WeaponType type { get; set; }
        public int level { get; set; }
        public EquipmentEffect effect { get; set; }
        
        public Weapon(int _id, string _name, string _description, uint _value, WeaponType _type, int _level)
        {
            id = _id;
            name = _name;
            description = _description;
            value = _value;
            type = _type;
            level = _level;
            stackable = false;
            EquipmentEffect e;
            weaponTypeEffects.TryGetValue(_type, out e);
            effect = e;
            effect.modifyEffect(1 + ItemSet.levelMultipler * (level-1));
        }

        public Weapon(int _id, Weapon weapon, int _level)
        {
            id = _id;
            name = Equipment.equipmentPrefixes[_level-1] + weapon.name;
            description = weapon.description;
            value = weapon.value;
            type = weapon.type;
            level = _level;
            stackable = false;
            EquipmentEffect e;
            weaponTypeEffects.TryGetValue(type, out e);
            effect = new EquipmentEffect(e);
            effect.modifyEffect(1 + ItemSet.levelMultipler * (level-1));
        }

        public static void populateWeaponTypeEffects()
        {
            EquipmentEffect staffEffect = new EquipmentEffect(0, 0, 10, 0, 0, 50);
            weaponTypeEffects.Add(WeaponType.STAFF, staffEffect);

            EquipmentEffect oneHandedSword = new EquipmentEffect(5, 5, 0, 10, 0, 0);
            weaponTypeEffects.Add(WeaponType.ONEHANDEDSWORD, oneHandedSword);

            EquipmentEffect twoHandedSword = new EquipmentEffect(10, -5, 0, -5, 0, 0);
            weaponTypeEffects.Add(WeaponType.TWOHANDEDSWORD, twoHandedSword);
        }

        public bool dequip(Character c)
        {
            if (c.inventory.Count == Character.INVENTORYSIZE)
            {
                return false;
            }
            else
            {
                Weapon oldWeapon = c.weapon;
                c.inventory.Add(new ItemStack(oldWeapon));
                c.weapon = null;
                return true;
            }
        }

    }

    public class EquipmentEffect
    {
        public int strengthMod { get; set; }
        public int agilityMod { get; set; }
        public int intelligenceMod { get; set; }
        public int speedMod { get; set; }
        public int healthMod { get; set; }
        public int manaMod { get; set; }

        public EquipmentEffect(int _strengthMod, int _agilityMod, int _intelligenceMod, int _speedMod, int _healthMod, int _manaMod)
        {
            strengthMod = _strengthMod;
            agilityMod = _agilityMod;
            intelligenceMod = _intelligenceMod;
            speedMod = _speedMod;
            healthMod = _healthMod;
            manaMod = _manaMod;
        }

        public EquipmentEffect(EquipmentEffect effect)
        {
            strengthMod = effect.strengthMod;
            agilityMod = effect.agilityMod;
            intelligenceMod = effect.intelligenceMod;
            speedMod = effect.speedMod;
            healthMod = effect.healthMod;
            manaMod = effect.manaMod;
        }

        public void modifyEffect(double modifier)
        {
            healthMod = (int)Math.Floor(healthMod*modifier);
            manaMod = (int)Math.Floor(manaMod*modifier);
            strengthMod = (int)Math.Floor(strengthMod * modifier);
            agilityMod = (int)Math.Floor(agilityMod * modifier);
            intelligenceMod = (int)Math.Floor(intelligenceMod * modifier);
            speedMod = (int)Math.Floor(speedMod * modifier);
        }
    }

    public enum WeaponType
    {
        STAFF,
        ONEHANDEDSWORD,
        TWOHANDEDSWORD,
    }

    public class Armour : Item
    {
        public ArmourType type { get; set; }
        public EquipmentEffect stats { get; set; }
        public int level { get; set; }

        public Armour(int _id, string _name, string _description, uint _value, ArmourType _type, EquipmentEffect _stats, int _level)
        {
            id = _id;
            name = _name;
            description = _description;
            value = _value;
            type = _type;
            stackable = false;
            _stats.modifyEffect(1 + (_level-1) * ItemSet.levelMultipler);
            stats = _stats;
            level = _level;
        }

        public Armour(int _id, Armour armour, int _level)
        {
            id = _id;
            name = Equipment.equipmentPrefixes[_level-1] + armour.name;
            description = armour.description;
            value = armour.value;
            type = armour.type;
            stackable = false;
            stats = new EquipmentEffect(armour.stats);
            stats.modifyEffect(1 + (_level-1) * ItemSet.levelMultipler);
            level = _level;
        }

        public bool dequip(Character c)
        {
            if (c.inventory.Count == Character.INVENTORYSIZE)
            {
                return false;
            }
            else
            {
                switch (type)
                {
                    case (ArmourType.CHEST):
                        {
                            Armour oldchest = c.chest;
                            c.inventory.Add(new ItemStack(oldchest));
                            c.chest = null;
                            return true;
                        }
                    case (ArmourType.HELM):
                        {
                            Armour oldhelm = c.helm;
                            c.inventory.Add(new ItemStack(oldhelm));
                            c.helm = null;
                            return true;
                        }
                    case (ArmourType.GLOVES):
                        {
                            Armour oldgloves = c.gloves;
                            c.inventory.Add(new ItemStack(oldgloves));
                            c.gloves = null;
                            return true;
                        }
                    case (ArmourType.BOOTS):
                        {
                            Armour oldboots = c.boots;
                            c.inventory.Add(new ItemStack(oldboots));
                            c.boots = null;
                            return true;
                        }
                    case (ArmourType.LEGS):
                        {
                            Armour oldlegs = c.legs;
                            c.inventory.Add(new ItemStack(oldlegs));
                            c.legs = null;
                            return true;
                        }
                }
                return true;
            }
        }

    }

    public enum ArmourType
    {
        HELM,
        CHEST,
        GLOVES,
        LEGS,
        BOOTS
    }

    public class Equipment
    {
        public static string[] equipmentPrefixes = { "Damaged ", "", "Superior " };
    }

}
