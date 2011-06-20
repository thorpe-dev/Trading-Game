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
            IDictionary<string, Ability> warriorAbilities = new Dictionary<string, Ability>();
            warriorAbilities.Add("Maim", Ability.fetchAbility("Maim"));
            ClassSet.createClass(ClassType.Warrior, new Class(new StatModifier(20, 15, 10, 15), new StatModifier(3, 2, 1, 1), ClassType.Warrior,
                                                                "Im a Warrior", new Uri("Images/robot.png", UriKind.Relative),
                                                                warriorAbilities, null,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                null));


            IDictionary<string, Ability> mageAbilities = new Dictionary<string, Ability>();
            mageAbilities.Add("Fireball", Ability.fetchAbility("Fireball"));
            mageAbilities.Add("Energy arrow", Ability.fetchAbility("Energy arrow"));
            ClassSet.createClass(ClassType.Mage, new Class(new StatModifier(10, 100, 25, 5), new StatModifier(1, 1, 4, 1), ClassType.Mage,
                                                                "Im a Mage", new Uri("Images/failsprite.png", UriKind.Relative),
                                                                mageAbilities, null,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                null));


            IDictionary<string, Ability> rogueAbilities = new Dictionary<string, Ability>();
            rogueAbilities.Add("Attack", Ability.fetchAbility("Attack"));
            ClassSet.createClass(ClassType.Rogue, new Class(new StatModifier(15, 20, 5, 20), new StatModifier(2, 3, 1, 3), ClassType.Rogue,
                                                                "Im a Rogue", new Uri("Images/clam.png", UriKind.Relative),
                                                                 rogueAbilities, null,
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
