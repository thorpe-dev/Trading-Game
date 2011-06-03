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
using System.Windows.Navigation;
using System.Xml.Linq;

namespace SilverlightApplication1
{
    public partial class LoginPage : Page
    {

        public LoginPage()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string uploadString = String.Format("username={0}&password={1}", userInput.Text, passwordBox.Password);
            HttpConnection.httpPost(new Uri("login.php", UriKind.Relative), uploadString, new UploadStringCompletedEventHandler(dataComplete));
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
                    MainPage.frame.Navigate(new Uri("/CharacterPage.xaml", UriKind.Relative));
                }
            }
            else
            {
                MessageBox.Show("ERROR: " + e.Error.ToString());
            }
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpConnection.httpLongPoll(new Uri("reverseAjax.php", UriKind.Relative), transferDone, 2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void transferDone(Object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    registerButton.Content = e.Result;
                }
                else
                {
                    MessageBox.Show("ERROR: " + e.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
