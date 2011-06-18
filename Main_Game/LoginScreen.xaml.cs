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
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Main_Game
{
    public partial class LoginScreen : UserControl, IScreen
    {

        public LoginScreen()
        {
            InitializeComponent();
        }

        public UIElement Element { get { return this; } }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            String hash = hashString(txt_password.Password);
            string uploadString = String.Format("username={0}&password={1}", txt_username.Text, hash);
            HttpConnection.httpPost(new Uri("login.php", UriKind.Relative), uploadString, new UploadStringCompletedEventHandler(dataComplete));
        }

        public static String hashString(String text)
        {
            SHA256 hasher = new SHA256Managed();
            hasher.Initialize();
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            byte[] hash = hasher.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }

        private void dataComplete(Object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XDocument doc = XDocument.Parse(e.Result);
                if (doc.Element("error") != null)
                {
                    MessageBox.Show("ERROR: " + (string)doc.Element("error"));
                }
                else
                {
                    ScreenManager.SetScreen(new CharacterScreen());
                }
            }
            else
            {
                MessageBox.Show("ERROR: " + e.Error.ToString());
            }
        }

        private void btn_register_Click(object sender, RoutedEventArgs e)
        {
            Register r = new Register();
            r.Show();
        }

        private void btn_skip_Click(object sender, RoutedEventArgs e)
        {
            BattleScreen tBattleScreen = new BattleScreen();
            CharacterScreen tCharScreen = new CharacterScreen();
            ScreenManager.SetScreen(tCharScreen);
            ScreenManager.SetSettingBar(new settingBar());
            ScreenManager.SetSideBar(new sideBar());
            tBattleScreen.Focus();
        }

    }
}
