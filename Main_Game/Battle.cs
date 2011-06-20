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
    public class Battle
    {
        public bool isPlayersTurn { get; set; }
        public string battleText { get; set; }
        private BattleScreen observer;

        protected Character char_1;
        protected Creep char_2;
        private int playerSpeedDecay;
        private int enemySpeedDecay;

        public Battle(Character p1, Creep p2, BattleScreen obs)
        {
            char_1 = p1;
            char_2 = p2;
            isPlayersTurn = false;
            playerSpeedDecay = 1;
            enemySpeedDecay = 1;
            battleText = "";
            observer = obs;
            observer.addBattleText("Encountered a " + char_2.name);
        }

        public void beginTurn()
        {
            if (((char_1.speed * char_1.buffs.speed_mod) / playerSpeedDecay) <
                ((char_2.speed * char_2.buffs.speed_mod) / enemySpeedDecay))
            {
                observer.addBattleText(char_2.name + " is readying an attack");
                Ability attack = char_2.getAbility();
                int damage = char_2.Attack(attack, char_1);
                if (damage == 0)
                {
                    if (attack is SelfAbility)
                    {
                        observer.addBattleText(char_2.name + " used " + attack.name + " and powered up");
                    }
                    else
                    {
                        observer.addBattleText(char_2.name + " used " + attack.name + " and missed");
                    }
                }
                else
                {
                    observer.addBattleText(char_2.name + " used " + attack.name + " for " + damage + " damage");
                }
                observer.updateStats(char_1, char_2);
                if (char_1.currentHealth == 0)
                {
                    observer.addBattleText("You have died");
                    endBattle();
                    return;
                }
                enemySpeedDecay *= 2;
                playerSpeedDecay = 1;
                beginTurn();
            }
            else
            {
                observer.addBattleText("You are able to attack");
                isPlayersTurn = true;
            }

        }


        public void playerAttack(Ability attack)
        {
            int damage = char_1.Attack(attack, char_2);
            if (damage == 0)
            {
                if (attack is SelfAbility)
                {
                    observer.addBattleText("You used " + attack.name + " and powered up");
                }
                else
                {
                    observer.addBattleText("You used " + attack.name + " and missed");
                }
            }
            else
            {
                observer.addBattleText("You used " + attack.name + " for " + damage + " damage");
            }
            observer.updateStats(char_1, char_2);
            if (char_2.currentHealth == 0)
            {
                observer.addBattleText(char_2.name + " is defeated");
                observer.addBattleText("You gained " + char_2.expValue + " experience");
                endBattle();
                return;
            }
            observer.updateAbilities(char_1);
            playerSpeedDecay *= 2;
            enemySpeedDecay = 1;
            beginTurn();
        }

        private void endBattle()
        {
            if (char_1.currentHealth == 0)
            {

                char_1.resetStats();
                char_2.refreshCreep();
                DeathWindow death = new DeathWindow();
                death.Show();
                return;
            }
            else
            {
                int expValue = char_2.expValue;
                int expToNext = char_1.expToNext;
                while (expToNext <= expValue)
                {
                    char_1.levelUp();
                    observer.addBattleText("Level up!");
                    observer.addBattleText("You are now level " + char_1.level);
                    expValue -= expToNext;
                    expToNext = char_1.expToNext;
                }
                char_1.expToNext -= expValue;
                MainPage.currentSideBar.updateStats();
                Random rnd = new Random();
                int lootTableSize = char_2.lootTable.Count;
                int lootRand = rnd.Next(1, (((int)Math.Pow(lootTableSize, 2) + lootTableSize) / 2) + 1);
                int n = lootTableSize;
                int i = 0;
                while (n < lootRand)
                {
                    n += (n - 1);
                    i++;
                }
                Item loot = char_2.lootTable.ToArray()[i];
                if (loot is Armour)
                {
                    lootRand = rnd.Next(1, (((int)Math.Pow(Equipment.NUMBEROFLEVELS, 2) + Equipment.NUMBEROFLEVELS) / 2) + 1);
                    int j = Equipment.NUMBEROFLEVELS;
                    int k = 1;
                    while (j < lootRand)
                    {
                        j += (j - 1);
                        k++;
                    }
                    loot = new Armour(loot.id, loot as Armour, k);
                }
                else if (loot is Weapon)
                {
                    lootRand = rnd.Next(1, (((int)Math.Pow(Equipment.NUMBEROFLEVELS, 2) + Equipment.NUMBEROFLEVELS) / 2) + 1);
                    int j = Equipment.NUMBEROFLEVELS;
                    int k = 1;
                    while (j < lootRand)
                    {
                        j += (j - 1);
                        k++;
                    }
                    loot = new Weapon(loot.id, loot as Weapon, k);
                }
                //Setup loot screen              
                char_1.resetStats();
                char_2.refreshCreep();
                char_1.money += char_2.moneyDrop;
                observer.displayLoot(loot);
            }
        }

        public bool playerCanCast(Ability a)
        {
            return (a.manaCost <= char_1.currentMana);
        }

        public static Creep generateCreep(IDictionary<string, Creep> dict)
        {
            Random rnd = new Random();

            Creep[] array = new Creep[dict.Count];

            array = dict.Values.ToArray();


            return array[rnd.Next(0, array.Length)];
        }
    }
}
