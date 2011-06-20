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

namespace Main_Game
{

    public abstract class Ability
    {
        public const int initialAbilityLimit = 1;
        public const int levelAbilityLimit = 1;
        public const int iconSize = 40;

        public static IDictionary<string, Ability> allAbilities = new Dictionary<string, Ability>();
        public string name { get; set; }
        public string description { get; set; }
        public int manaCost { get; set; }
        public Uri icon { get; set; }
        public Effect abilityEffect { get; set; }

        public Ability(string _name, string _description, int _manacost, Uri _icon, Effect _abilityEffect)
        {
            name = _name;
            description = _description;
            manaCost = _manacost;
            icon = _icon;
            abilityEffect = _abilityEffect;
        }

        public static void populateAllAbility()
        {
            allAbilities.Add("Grow", new SelfAbility("Grow", "Grows in size and gains strength", 20, 
                                        new Uri("Images/grow.png", UriKind.Relative),
                                        new Effect(0, 1.3, 1, 1, 1)));

            allAbilities.Add("War Cry", new SelfAbility("War Cry", "A morale-boosting shout that raises attack", 10,
                                        new Uri("Images/grow.png", UriKind.Relative),
                                        new Effect(0, 1.3, 1, 1, 1)));
            allAbilities.Add("Wild Charge", new AttackAbility("Wild Charge", "A reckless attack that causes more damage", 40,
                                        new Uri("Images/grow.png", UriKind.Relative),
                                        new Effect(0, 1, 1, 1, 1), 1.3));
            allAbilities.Add("Regenerate", new SelfAbility("Regenerate", "Restores the user", 50,
                                        new Uri("Images/grow.png", UriKind.Relative),
                                        new Effect(50, 1, 1, 1, 1)));
            allAbilities.Add("Grave Chill", new MagicAbility("Grave Chill", "Freezes the enemy with unholy power", 25,
                                        new Uri("Images/fireball.png", UriKind.Relative),
                                        new Effect(), 1.2));
            allAbilities.Add("Bull Rush", new AttackAbility("Bull Rush", "A reckless attack that causes more damage and boosts speed", 40,
                                        new Uri("Images/grow.png", UriKind.Relative),
                                        new Effect(0, 1, 1, 1, 1.2), 1.4));
            allAbilities.Add("Battle Roar", new SelfAbility("Battle Roar", "A morale-boosting shout that raises attack", 10,
                                        new Uri("Images/grow.png", UriKind.Relative),
                                        new Effect(0, 1.3, 1, 1, 1)));

            allAbilities.Add("Attack", new AttackAbility("Attack", "Strikes the enemy with the currently held weapon", 0,
                                        new Uri("Images/Abilities/Attack.png", UriKind.Relative),
                                        new Effect(), 1.5));

            allAbilities.Add("Double Strike", new AttackAbility("Double Strike", "Strikes the enemy with two hits in quick succession", 10,
                                        new Uri("Images/Abilities/DoubleStrike.png", UriKind.Relative),
                                        new Effect(), 2));
            allAbilities.Add("Battle Cry", new SelfAbility("Battle Cry", "Lets out a terrifying roar, raising the user’s strength by a lot", 30,
                                        new Uri("Images/Abilities/BattleCry.png", UriKind.Relative),
                                        new Effect(0,1.5,1,1,1)));
            allAbilities.Add("Butcher", new AttackAbility("Butcher", "Hacks at the enemy, doing lots of damage", 80,
                                        new Uri("Images/Abilities/Butcher.png", UriKind.Relative),
                                        new Effect(), 3));
            allAbilities.Add("Dash", new SelfAbility("Dash", "Increases the user's speed by 50%", 50,
                                        new Uri("Images/Abilities/Dash.png", UriKind.Relative),
                                        new Effect(0,1,1,1,1.5)));
            allAbilities.Add("Insane Rage", new AttackAbility("Insane Rage", "Goes berserk, swinging blindly at the opponent but angers your enemy resulting in a increase in their strength",
                                        10, new Uri("Images/Abilities/InsaneRage.png", UriKind.Relative),
                                        new Effect(0,1.4,1,1,1), 3));

            allAbilities.Add("Fireball", new MagicAbility("Fireball", "Hurls a ball of fire at the target", 50,
                            new Uri("Images/Abilities/Fireball.png", UriKind.Relative),
                            new Effect(), 2));
            allAbilities.Add("Concentration", new SelfAbility("Concentration", "Meditates to raise the effectiveness of magic attacks", 30,
                                        new Uri("Images/Abilities/Concentration.png", UriKind.Relative),
                                        new Effect(0, 1, 1, 1.5, 1)));
            allAbilities.Add("Earth Shatter", new MagicAbility("Earth Shatter", "Shatters the ground under the target, reducing their speed and damaging them", 80,
                                        new Uri("Images/Abilities/EarthShatter.png", UriKind.Relative),
                                        new Effect(0, 1, 1, 1, 0.65), 3));
            allAbilities.Add("Restore Health", new SelfAbility("Restore Health", "Sacrifices magic power to restore health", 100,
                                        new Uri("Images/Abilities/HealthRestore.png", UriKind.Relative),
                                        new Effect(200, 1, 1, 1, 1)));
            allAbilities.Add("Meteor Storm", new MagicAbility("Meteor Storm", "Summons a hail of meteors to obliterate the target", 200,
                                        new Uri("Images/Abilities/MeteorStorm.png", UriKind.Relative),
                                        new Effect(), 5));

            allAbilities.Add("Mystic Slash", new MagicAbility("Mystic Slash", "Swipes at the energy with a magically imbued blade. Also fucked", 200,
                                        new Uri("Images/Abilities/Mystic Slash.png", UriKind.Relative),
                                        new Effect(0,0.8,1,0.8,1), 4));
        }

        public static Ability fetchAbility(string abilityname)
        {
            Ability a;
            allAbilities.TryGetValue(abilityname, out a);
            return a;
        }

        public static IDictionary<string, Ability> constructAbilitySet(params string[] abilityNames)
        {
            IDictionary<string, Ability> abilitySet = new Dictionary<string, Ability>();
            foreach (string name in abilityNames)
            {
                abilitySet.Add(name, fetchAbility(name));
            }
            return abilitySet;
        }

        public abstract uint attack(Entity attacker, Entity defender);
    }

    public class AttackAbility : Ability
    {
        protected double p_attackbonus;

        public double attackbonus { get { return p_attackbonus; } }

        public AttackAbility(string _name, string _description, int _manacost, Uri _icon, Effect _abilityEffect, double _attackBonus)
            : base(_name, _description, _manacost, _icon, _abilityEffect)
        {
            this.p_attackbonus = _attackBonus;
        }

        public override uint attack(Entity attacker, Entity defender)
        {

            int hit = attacker.dice.roll();
            uint damage = 0;
            double agiDiff = defender.agility * defender.buffs.agility_mod - attacker.agility * attacker.buffs.agility_mod;
            int hitBonus = (int)Math.Floor(agiDiff / 15);
            if (hit <= 3 + hitBonus || (this.manaCost > attacker.currentMana))
                return damage;
            else
            {
                attacker.currentMana -= manaCost;
                defender.applyEffect(abilityEffect);
                damage = (uint)Math.Floor(p_attackbonus * (double)attacker.strength * attacker.buffs.strength_mod);
                if (hit == 20)
                    damage *= 2;

                return damage;
            }

        }
    }

    public class SelfAbility : Ability
    {
        public SelfAbility(string _name, string _description, int _manacost, Uri _icon, Effect _abilityEffect)
            : base(_name, _description, _manacost, _icon, _abilityEffect)
        {
        }

        public override uint attack(Entity attacker, Entity defender)
        {
            if (manaCost <= attacker.currentMana)
            {
                attacker.applyEffect(abilityEffect);
                attacker.currentMana -= manaCost;
            }
            return 0;
        }
    }

    public class MagicAbility : AttackAbility
    {

        public MagicAbility(string _name, string _description, int _manacost, Uri _icon, Effect _abilityEffect, double _attackBonus)
            : base(_name, _description, _manacost, _icon, _abilityEffect, _attackBonus) { }

        public override uint attack(Entity attacker, Entity defender)
        {
            int hit = attacker.dice.roll();
            uint damage = 0;
            double agiDiff = defender.agility * defender.buffs.agility_mod - attacker.agility * attacker.buffs.agility_mod;
            int hitBonus = (int)Math.Floor(agiDiff / 15);
            if (hit <= 3 + hitBonus || (manaCost > attacker.currentMana))
                return damage;
            else
            {
                defender.applyEffect(abilityEffect);
                attacker.currentMana -= manaCost;
                damage = (uint)Math.Floor(p_attackbonus * (double)attacker.intelligence * attacker.buffs.intelligence_mod);
                if (hit == 20)
                    damage *= 2;

                return damage;
            }
        }
    }
}
