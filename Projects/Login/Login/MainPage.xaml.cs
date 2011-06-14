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

namespace Login
{
    public partial class MainPage : UserControl
    {

        public MainPage()
        {
            InitializeComponent();

        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            WebClient client = new WebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(dataCompleted);
            string parameters = String.Format("name={0}&password={1}", txt_username.Text, txt_password.Password);
            client.UploadStringAsync(
                 new Uri("http://www.doc.ic.ac.uk/project/2010/271/g1027131/Trading-Game/Login/login.php",
                          UriKind.Absolute), "POST", parameters);
        }

        private void dataCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    XDocument doc = XDocument.Parse(e.Result);
                    var people = from person in doc.Descendants("person")
                                 select new Person()
                                 {
                                     playernumber = (int)person.Element("playernumber"),
                                     username = (string)person.Element("username"),
                                     password = (string)person.Element("password")
                                 };
                    Person p = people.First();
                    if (p.playernumber == 0)
                    {
                        MessageBox.Show("Login or Password incorrect");
                    }
                    else
                    {
                        MessageBox.Show("Player " + p.playernumber);
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show("Error: " + e1.ToString());
                }
            }

            else
            {
                MessageBox.Show("ERROR " + e.Error.ToString());
            }
        }

        private void btn_register_Click(object sender, RoutedEventArgs e)
        {
            Register r = new Register();
            r.Show();
        }

    }

    public class Person
    {
        public int playernumber;
        public string username;
        public string password;
    }
}
