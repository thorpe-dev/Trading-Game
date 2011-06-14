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
        private static IDictionary<string, Item> allItems = new Dictionary<string, Item>();

        public static void addItem(Item i)
        {
            allItems.Add(i.name, i);
        }

        public static Item retrieveItem(string name)
        {
            Item i;
            allItems.TryGetValue(name, out i);
            return i;
        }

        public static void constructItemBase()
        {
            Consumable healthPot;
            Consumable manaPot;
            for (uint i = 0; i < Consumable.consumablePrefixes.Length; i++)
            {
                string prefix = Consumable.consumablePrefixes[i];
                healthPot = new Consumable(prefix + "healing potion", "Regenerates health", Consumable.healthPotionBaseValue * i,
                                ConsumableType.health, (int)(Consumable.healthPotionBaseRegen * i));
                ItemSet.addItem(healthPot);
                manaPot = new Consumable(prefix + "mana potion", "Regenerates mana", Consumable.manaPotionBaseValue * i,
                                ConsumableType.mana, (int)(Consumable.manaPotionBaseRegen * i));
                ItemSet.addItem(manaPot);
            }
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

        public bool useItem(Character c)
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
            if (item is Weapon)
            {
                Weapon weapon = item as Weapon;
                if (c.weapon == null)
                {
                    
                    weapon.equip(c);
                    c.inventory.Remove(this);
                    return true;
                }
                else
                {
                    ICollection<ItemStack> inventory = c.inventory;
                    if (inventory.LongCount() < Character.INVENTORYSIZE)
                    {
                        inventory.Remove(this);
                        ItemStack oldWeapon = new ItemStack(c.weapon);
                        inventory.Add(oldWeapon);
                        c.weapon = weapon;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public abstract class Item
    {
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

        public Consumable(string _name, string _description, uint _value, ConsumableType _type, int _amountRegenerated)
        {
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
        public static IDictionary<WeaponType, BaseWeaponEffect> weaponTypeEffects = new Dictionary<WeaponType, BaseWeaponEffect>();

        public WeaponType type { get; set; }
        
        public Weapon(string _name, string _description, uint _value, WeaponType _type)
        {
            name = _name;
            description = _description;
            value = _value;
            type = _type;
            stackable = false;
        }

        public static void populateWeaponTypeEffects()
        {
            BaseWeaponEffect staffEffect = new BaseWeaponEffect(10, 0, 0, 10, 0, 50);
            weaponTypeEffects.Add(WeaponType.STAFF, staffEffect);

            BaseWeaponEffect shieldEffect = new BaseWeaponEffect(0, 10, -5, 0, 50, 0);
            weaponTypeEffects.Add(WeaponType.SHIELD, shieldEffect);

            BaseWeaponEffect oneHandedSword = new BaseWeaponEffect(20, 5, 5, 0, 0, 0);
            weaponTypeEffects.Add(WeaponType.ONEHANDEDSWORD, oneHandedSword);

            BaseWeaponEffect twoHandedSword = new BaseWeaponEffect(35, 10, -5, 0, 0, 0);
            weaponTypeEffects.Add(WeaponType.TWOHANDEDSWORD, twoHandedSword);
        }

        public void equip(Character c)
        {
            if (c.weapon != null)
            {
                c.weapon = this;
            }
            else
            {

            }
        }
    }

    public class BaseWeaponEffect
    {
        public uint baseDamage { get; set; }
        public int strengthMod { get; set; }
        public int agilityMod { get; set; }
        public int intelligenceMod { get; set; }
        public int healthMod { get; set; }
        public int manaMod { get; set; }

        public BaseWeaponEffect(uint _baseDamage, int _strengthMod, int _agilityMod, int _intelligenceMod, int _healthMod, int _manaMod)
        {
            baseDamage = _baseDamage;
            strengthMod = _strengthMod;
            agilityMod = _agilityMod;
            intelligenceMod = _intelligenceMod;
            healthMod = _healthMod;
            manaMod = _manaMod;
        }
    }

    public enum WeaponType
    {
        STAFF,
        ONEHANDEDSWORD,
        TWOHANDEDSWORD,
        SHIELD
    }

}
