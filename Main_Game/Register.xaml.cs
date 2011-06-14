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

namespace Main_Game
{
    public partial class Register : ChildWindow
    {
        public Register()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (txt_password.Password != txt_confirm.Password)
            {
                lbl_confirm_fail.Content = "Password values do not match";
                lbl_confirm_fail.Visibility = Visibility.Visible;
            }
            else if (txt_username.Text == "")
            {
                lbl_confirm_fail.Content = "You must have a username";
                lbl_confirm_fail.Visibility = Visibility.Visible;
            }
            else if (txt_username.Text.Contains(";")
                   || txt_username.Text.Contains("'")
                   || txt_username.Text.Contains(" ")
                   || txt_username.Text.Contains(">")
                   || txt_username.Text.Contains("\t"))
            {
                lbl_confirm_fail.Content = "Username contains invalid characters";
                lbl_confirm_fail.Visibility = Visibility.Visible;
            }
            else if (txt_password.Password == "") 
            {
                lbl_confirm_fail.Content = "You must have a password";
                lbl_confirm_fail.Visibility = Visibility.Visible;
            }
            else
            {
                lbl_confirm_fail.Visibility = Visibility.Collapsed;
                string hash = LoginScreen.hashString(txt_password.Password);
                string parameters = String.Format("name={0}&password={1}", txt_username.Text, hash);
                HttpConnection.httpPost(new Uri("register.php", UriKind.Relative), parameters, dataCompleted);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void dataCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    XDocument doc = XDocument.Parse(e.Result);
                    if (doc.Element("validity") == null && doc.Element("validity") == null)
                    {
                        this.DialogResult = true;
                    }
                    else if (doc.Element("validity") != null)
                    {
                        lbl_confirm_fail.Content = (string)doc.Element("validity");
                        lbl_confirm_fail.Visibility = Visibility.Visible;
                        txt_username.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Error: " + (string)doc.Element("error"));
                        txt_username.Focus();
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
}

