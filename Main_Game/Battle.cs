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
        public bool playersTurn { get; set; }

        protected Character char_1;
        protected Creep char_2;


        public Battle(Character p1, Dictionary<String, Creep> dictionary)
        {
            this.char_1 = p1;
            this.char_2 = generateCreep(dictionary);
            playersTurn = false;

        }

        public void beginTurn()
        {
            if (char_1.dice.roll() * char_1.speed * char_1.buffs.speed_mod <
                char_2.dice.roll() * char_2.speed * char_1.buffs.speed_mod)
            {
                this.char_2.Attack(char_2.getAbility(), char_1);
                if (char_1.currentHealth == 0)
                {
                    endBattle();
                    return;
                }
                beginTurn();
            }
            else
            {
                playersTurn = true;
            }

        }

        public void playerAttack(Ability attack)
        {
            char_1.Attack(attack, char_2);
            if (char_2.currentHealth == 0)
            {
                endBattle();
                return;
            }
            beginTurn();
        }

        public void endBattle()
        {
            if (char_1.currentHealth == 0)
            {
                //Player has lost
                return;
            }
            else
            {
                int progress = char_1.expToNext - char_2.expValue;
                if (progress <= 0)
                {
                    char_1.levelUp();
                    char_1.expToNext += progress;
                }
                else
                {
                    char_1.expToNext -= progress;
                }
                Random rnd = new Random();
                int lootTableSize = char_2.lootTable.Count;
                int lootRand = rnd.Next(1, ((int)Math.Sqrt(lootTableSize) + lootTableSize)/2);
                int n = lootTableSize;
                int i = 0;
                while (n < lootRand)
                {
                    n += (n - 1);
                    i++;
                }
               // Item 
                char_1.resetStats();
                char_1.money += char_2.expValue;
            }
            // Handle the end of the battle here
            // Update health and XP on server here
        }

        public Creep generateCreep(Dictionary<String, Creep> dict)
        {
            Random rnd = new Random();

            Creep[] array = new Creep[dict.Count];

            array = dict.Values.ToArray();


            return array[rnd.Next(0,array.Length)];
        }
    }
}
