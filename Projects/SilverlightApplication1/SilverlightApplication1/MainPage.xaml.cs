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

namespace SilverlightApplication1
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            
        }

        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            /*
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(dataCompleted);
            
            wc.DownloadStringAsync(
                new Uri("http://www.doc.ic.ac.uk/project/2010/271/g1027131/Trading-Game/name.php?name=" 
                         + txt_name.Text + "&password=" + txt_password.Password, UriKind.Absolute));
            
            */
           
            WebClient client = new WebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            client.UploadStringCompleted += new UploadStringCompletedEventHandler(dataCompleted);
            string parameters = String.Format("name={0}&password={1}", txt_name.Text,txt_password.Password);
            client.UploadStringAsync(
                 new Uri("http://www.doc.ic.ac.uk/project/2010/271/g1027131/Trading-Game/name.php",
                          UriKind.Absolute), "POST", parameters);
        }


        private void dataCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                MessageBox.Show("You logged in with username " + e.Result);
            }

            else
            {
                MessageBox.Show("ERROR " + e.Error.ToString());
            }
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
    }

    public class Person
    {
        public int playernumber;
        public string username;
        public string password;
    }
}
