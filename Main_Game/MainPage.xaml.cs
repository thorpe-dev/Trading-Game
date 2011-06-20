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

namespace Main_Game
{
    public partial class MainPage : UserControl, IScreenHost
    {

        public static settingBar currentSettingBar;
        public static sideBar currentSideBar;

        public MainPage()
        {
            InitializeComponent();
            Ability.populateAllAbility();
            ItemSet.constructItemBase();
            Creep.populateCreeps();
            initialiseClasses();
        }

        private void initialiseClasses()
        {
            IDictionary<string, Ability> bruteAbilities = Ability.constructAbilitySet("Attack", "Double Strike", "Battle Cry", "Butcher",
                                                                                      "Dash", "Insane Rage");
            ClassSet.createClass(ClassType.Brute, new Class(new StatModifier(21, 10, 4, 8), new StatModifier(3, 2, 1, 1), ClassType.Brute,
                                                                "Brutes embody primal rage. When angered, they will rarely calm down before all their enemies are reduced to red goo. Muscle-bound and stubborn as "
                                                                + "mules, if a brute does not approve of something he will not express the fact diplomatically - encounters with brutes will often end quickly an violently.",
                                                                new Uri("Images/WarriorIcon.png", UriKind.Relative),
                                                                bruteAbilities, null,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                null));


            IDictionary<string, Ability> sorcererAbilities = Ability.constructAbilitySet("Attack", "Fireball", "Concentration", "Earth Shatter",
                                                                                     "Restore Health", "Meteor Storm");
            ClassSet.createClass(ClassType.Sorcerer, new Class(new StatModifier(8, 10, 20, 6), new StatModifier(1, 1, 4, 1), ClassType.Sorcerer,
                                                                "Sorcerers are scholars who have dedicated their live to the study of magic and the arcane arts. Unlike brutes, "
                                                                + "mages are not content to simply destroy their foes - they wish to study, to learn and to improve. Enemies that die quick deaths "
                                                                + "can be thankful that they will not become the subjects of a sorcerer's experiments.",
                                                                new Uri("Images/MageIcon.png", UriKind.Relative),
                                                                sorcererAbilities, null,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                null));


            IDictionary<string, Ability> bardAbilities = Ability.constructAbilitySet("Attack", "Double Strike", "Restore Health", "Earth Shatter",
                                                                                    "Dash", "Mystic Slash");
            ClassSet.createClass(ClassType.Bard, new Class(new StatModifier(10, 14, 5, 15), new StatModifier(1, 3, 1, 2), ClassType.Bard,
                                                                "Bards mix the sorcerer's knowledge of the arcane with the brute's martial prowess. Able to employ both strength and subtlty when necessary, "
                                                                + "bards are flexible and adaptably. Bards are rarely content to remain in one place for long, and many are subject to an intense wanderlust "
                                                                + "which leads them into many adventures and, more often than not, back out of them again.", 
                                                                 new Uri("Images/RogueIcon.png", UriKind.Relative),
                                                                 bardAbilities, null,
                                                                 null,
                                                                 null,
                                                                 null,
                                                                 null,
                                                                 null));
        }

        public void SetScreen(IScreen screen)
        {
            mainContent.Children.Clear();
            mainContent.Children.Add(screen.Element);
        }

        public void SetSettingBar(IScreen screen)
        {
            settingBar.Children.Clear();
            if (screen != null)
            {
                settingBar.Children.Add(screen.Element);
                currentSettingBar = screen as settingBar;
            }
        }

        public void SetSideBar(IScreen screen)
        {
            sideBar.Children.Clear();
            if (screen != null)
            {
                sideBar.Children.Add(screen.Element);
                currentSideBar = screen as sideBar;
            }
        }

        private void main_Loaded(object sender, RoutedEventArgs e)
        {
            ScreenManager.SetHost(this);
            ScreenManager.SetScreen(new LoginScreen());
        }

    }
    public interface IScreen
    {
        UIElement Element { get; }
    }

    public interface IScreenHost
    {
        void SetScreen(IScreen screen);
        void SetSettingBar(IScreen screen);
        void SetSideBar(IScreen screen);
    }

    public static class ScreenManager
    {
        private static IScreenHost _host;

        public static void SetHost(IScreenHost host)
        {
            _host = host;
        }

        public static void SetScreen(IScreen screen)
        {
            _host.SetScreen(screen);
        }

        public static void SetSettingBar(IScreen screen)
        {
            _host.SetSettingBar(screen);
        }

        public static void SetSideBar(IScreen screen)
        {
            _host.SetSideBar(screen);
        }
    }


}
