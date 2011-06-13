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

namespace Login
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
                WebClient client = new WebClient();
                client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                client.UploadStringCompleted += new UploadStringCompletedEventHandler(dataCompleted);
                string parameters = String.Format("name={0}&password={1}", txt_username.Text, txt_password.Password);
                client.UploadStringAsync(
                     new Uri("http://www.doc.ic.ac.uk/project/2010/271/g1027131/Trading-Game/Login/register.php",
                              UriKind.Absolute), "POST", parameters);
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
                    if (e.Result.Length == 1)
                    {
                        lbl_confirm_fail.Content = "Username is already in use - choose another";
                        lbl_confirm_fail.Visibility = Visibility.Visible;
                        txt_username.Focus();
                    }
                    else
                    {
                        MessageBox.Show(e.Result);
                        this.DialogResult = true;
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

