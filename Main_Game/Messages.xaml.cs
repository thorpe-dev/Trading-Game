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
    public partial class Messages : ChildWindow
    {

        Grid grd_messages;
        List<Message> messages;
        bool writing;
        Label grd_sender;
        Label grd_subject;
        TextBlock grd_message_text;
        TextBox txt_subject;
        TextBox txt_message;
        ComboBox cmb_players;

        Contact[] players;

        public Messages()
        {
            InitializeComponent();
            grd_messages = new Grid();
            ColumnDefinition new_def = new ColumnDefinition();
            new_def.Width = new GridLength(50);
            grd_messages.ColumnDefinitions.Add(new_def);
            new_def = new ColumnDefinition();
            new_def.Width = new GridLength(140);
            grd_messages.ColumnDefinitions.Add(new_def);
            new_def = new ColumnDefinition();
            new_def.Width = new GridLength(232);
            grd_messages.ColumnDefinitions.Add(new_def);
            grd_messages.HorizontalAlignment = HorizontalAlignment.Left;
            grd_messages.VerticalAlignment = VerticalAlignment.Top;
            grd_messages.Margin = new Thickness(1, 1, 0, 0);
            grd_messages.Height = 80;
            grd_messages.Width = 422;
            scv_message_holder.Content = grd_messages;

            // Sender/To label
            grd_sender = new Label();
            Grid.SetRow(grd_sender, 0);
            grd_message_view.Children.Add(grd_sender);
            grd_sender.Visibility = Visibility.Collapsed;

            // Subject label
            grd_subject = new Label();
            Grid.SetRow(grd_subject, 1);
            grd_message_view.Children.Add(grd_subject);
            grd_subject.Visibility = Visibility.Collapsed;
            grd_subject.Content = "Subject:";

            // Message label
            grd_message_text = new TextBlock();
            Grid.SetRow(grd_message_text, 2);
            grd_message_text.HorizontalAlignment = HorizontalAlignment.Left;
            grd_message_text.Margin = new Thickness(0, 0, 0, 0);
            grd_message_view.Children.Add(grd_message_text);
            grd_message_text.Visibility = Visibility.Collapsed;
            grd_message_text.Height = 100;
            grd_message_text.Width = 375;
            grd_message_text.TextWrapping = TextWrapping.Wrap;

            // Players combobox
            cmb_players = new ComboBox();
            Grid.SetRow(cmb_players, 0);
            cmb_players.HorizontalAlignment = HorizontalAlignment.Left;
            cmb_players.Margin = new Thickness(60, 0, 0, 0);
            cmb_players.Width = 315;
            grd_message_view.Children.Add(cmb_players);
            cmb_players.Visibility = Visibility.Collapsed;

            // Subject textbox
            txt_subject = new TextBox();
            Grid.SetRow(txt_subject, 1);
            txt_subject.HorizontalAlignment = HorizontalAlignment.Left;
            txt_subject.Margin = new Thickness(60, 0, 0, 0);
            txt_subject.Width = 315;
            grd_message_view.Children.Add(txt_subject);
            txt_subject.Visibility = Visibility.Collapsed;

            // Message textbox
            txt_message = new TextBox();
            Grid.SetRow(txt_message, 2);
            txt_message.HorizontalAlignment = HorizontalAlignment.Left;
            txt_message.Margin = new Thickness(0, 0, 0, 0);
            txt_message.Width = 375;
            txt_message.Height = 100;
            grd_message_view.Children.Add(txt_message);
            txt_message.Visibility = Visibility.Collapsed;
            txt_message.AcceptsReturn = true;
            txt_message.TextWrapping = TextWrapping.Wrap;

            // Get a list of all characters on the server
            Uri path = new Uri("messages_get_contacts.php", UriKind.Relative);
            HttpConnection.httpGet(path, fill_contacts);
        }

        private void fill_contacts(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    XDocument doc = XDocument.Parse(e.Result);
                    // Check if any other players were found
                    if (doc.Element("notfound") == null)
                    {
                        var contacts = from player in doc.Descendants("player")
                                       select new Contact()
                                       {
                                           cid = (int)player.Element("cid"),
                                           name = (string)player.Element("name")
                                       };
                        this.players = contacts.ToArray();
                        for (int i = 0; i < players.Length; i++)
                        {
                            //MessageBox.Show(players[i].name);
                            cmb_players.Items.Add(players[i].name);
                        }
                        if (players.Length > 0)
                            cmb_players.SelectedIndex = 0;
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show("XML error: " + e1.ToString());
                    MessageBox.Show("Result: " + e.Result);
                }
            }
            else
            {
                MessageBox.Show("PHP error: " + e.Error.ToString());
            }
            // Get the player's messages
            Uri path = new Uri("messages_get_message.php", UriKind.Relative);
            HttpConnection.httpGet(path, fill_messages);
        }

        private void fill_messages(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    XDocument doc = XDocument.Parse(e.Result);
                    // Check if any messages were found
                    if (doc.Element("notfound") == null)
                    {
                        var found_messages = from message in doc.Descendants("message")
                                       select new Message()
                                       {
                                           id = (int)message.Element("id"),
                                           sender = (string)message.Element("sender"),
                                           subject = (string)message.Element("subject"),
                                           message  = (string)message.Element("text"),
                                           read = (int)message.Element("read")
                                       };
                        this.messages = found_messages.ToList();
                        for (int i = 0; i < messages.Count; i++)
                        {
                            CheckBox chk = new CheckBox();
                            chk.Tag = i;
                            chk.Checked += select;
                            chk.Unchecked += deselect;
                            Grid.SetColumn(chk, 0);
                            messages.ElementAt(i).selected = chk;
                            Label snd = new Label();
                            snd.Content = messages.ElementAt(i).sender;
                            snd.Tag = i;
                            snd.MouseLeftButtonDown += display_message;
                            Grid.SetColumn(snd, 1);
                            messages.ElementAt(i).sender_label = snd;
                            snd = new Label();
                            snd.Content = messages.ElementAt(i).subject;
                            snd.Tag = i;
                            snd.MouseLeftButtonDown += display_message;
                            Grid.SetColumn(snd, 2);
                            messages.ElementAt(i).subject_label = snd;
                        }
                        fill();
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show("XML error: " + e1.ToString());
                    MessageBox.Show("Result: " + e.Result);
                }
            }
            else
            {
                MessageBox.Show("PHP error: " + e.Error.ToString());
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btn_gen_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            messages = new List<Message>();
            Message new_msg = new Message();
            new_msg.sender = "Pandora";
            new_msg.subject = "Test";
            new_msg.message = "This is a test message. It has worked if you are reading this";
            CheckBox chk = new CheckBox();
            chk.Tag = i;
            chk.Checked += select;
            chk.Unchecked += deselect;
            Grid.SetColumn(chk, 0);
            new_msg.selected = chk;
            Label snd = new Label();
            snd.Content = new_msg.sender;
            snd.Tag = i;
            snd.MouseLeftButtonDown += display_message;
            Grid.SetColumn(snd, 1);
            new_msg.sender_label = snd;
            snd = new Label();
            snd.Content = new_msg.subject;
            snd.Tag = i;
            snd.MouseLeftButtonDown += display_message;
            Grid.SetColumn(snd, 2);
            new_msg.subject_label = snd;
            messages.Add(new_msg);

            i++;
            new_msg = new Message();
            new_msg.sender = "Pandora";
            new_msg.subject = "Test Again";
            new_msg.message = "This is another test message. It has worked if you are reading this";
            chk = new CheckBox();
            chk.Checked += select;
            chk.Unchecked += deselect;
            chk.Tag = i;
            Grid.SetColumn(chk, 0);
            new_msg.selected = chk;
            snd = new Label();
            snd.Content = new_msg.sender;
            snd.Tag = i;
            snd.MouseLeftButtonDown += display_message;
            Grid.SetColumn(snd, 1);
            new_msg.sender_label = snd;
            snd = new Label();
            snd.Content = new_msg.subject;
            snd.Tag = i;
            snd.MouseLeftButtonDown += display_message;
            Grid.SetColumn(snd, 2);
            new_msg.subject_label = snd;
            messages.Add(new_msg);
        }

        private void select(object sender, RoutedEventArgs e)
        {
            int index = Int32.Parse((sender as CheckBox).Tag.ToString());
            messages.ElementAt(index).is_selected = true;
        }

        private void deselect(object sender, RoutedEventArgs e)
        {
            int index = Int32.Parse((sender as CheckBox).Tag.ToString());
            messages.ElementAt(index).is_selected = false;
        }

        private void display_message(object sender, RoutedEventArgs e)
        {
            int index = Int32.Parse((sender as Label).Tag.ToString());
            grd_sender.Content = "Sender: " + messages.ElementAt(index).sender;
            grd_sender.Visibility = Visibility.Visible;
            grd_subject.Content = "Subject: " + messages.ElementAt(index).subject;
            grd_subject.Visibility = Visibility.Visible;
            grd_message_text.Text = messages.ElementAt(index).message;
            grd_message_text.Visibility = Visibility.Visible;
            if (writing)
            {
                writing = false;
                btn_new_msg.Content = "New";
                cmb_players.Visibility = Visibility.Collapsed;
                txt_subject.Visibility = Visibility.Collapsed;
                txt_message.Visibility = Visibility.Collapsed;
            }
        }

        private void btn_fill_Click(object sender, RoutedEventArgs e)
        {
            fill();
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            if (!writing)
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    if (messages.ElementAt(i).is_selected)
                    {
                        // Remove the message
                        Uri path = new Uri("messages_delete_message.php", UriKind.Relative);
                        string parameters = String.Format("id={0}", messages.ElementAt(i).id);
                        HttpConnection.httpPost(path, parameters, message_sent);
                        messages.RemoveAt(i);
                        i--;
                    }
                }
                grd_sender.Visibility = Visibility.Collapsed;
                grd_subject.Visibility = Visibility.Collapsed;
                grd_message_text.Visibility = Visibility.Collapsed;
                fill();
            }
            else
            {
                writing = false;
                btn_new_msg.Content = "New";
                btn_delete.Content = "Delete";
                grd_sender.Visibility = Visibility.Collapsed;
                grd_subject.Visibility = Visibility.Collapsed;
                cmb_players.Visibility = Visibility.Collapsed;
                txt_subject.Visibility = Visibility.Collapsed;
                txt_message.Visibility = Visibility.Collapsed;
            }
        }

        private void fill()
        {
            grd_messages.Children.Clear();
            while (grd_messages.RowDefinitions.Count < messages.Count)
            {
                RowDefinition new_def = new RowDefinition();
                new_def.Height = new GridLength(25);
                grd_messages.RowDefinitions.Add(new_def);
            }
            for (int i = 0; i < messages.Count; i++)
            {
                Grid.SetRow(messages.ElementAt(i).selected, i);
                grd_messages.Children.Add(messages[i].selected);
                Grid.SetRow(messages.ElementAt(i).sender_label, i);
                grd_messages.Children.Add(messages[i].sender_label);
                Grid.SetRow(messages.ElementAt(i).subject_label, i);
                grd_messages.Children.Add(messages[i].subject_label);
            }
            grd_messages.Height = 25 * messages.Count;
        }

        private void btn_new_msg_Click(object sender, RoutedEventArgs e)
        {
            if (!writing)
            {
                writing = true;
                btn_new_msg.Content = "Send";
                btn_delete.Content = "Cancel";
                grd_sender.Content = "To:";
                grd_sender.Visibility = Visibility.Visible;
                grd_subject.Content = "Subject: ";
                grd_subject.Visibility = Visibility.Visible;
                txt_subject.Text = "";
                txt_subject.Visibility = Visibility.Visible;
                grd_message_text.Visibility = Visibility.Collapsed;
                txt_message.Visibility = Visibility.Visible;
                cmb_players.Visibility = Visibility.Visible;
            }
            else
            {
                // Find the target
                int recipient = this.players.ElementAt(cmb_players.SelectedIndex).cid;
                // Send the message
                Uri path = new Uri("messages_post_message.php", UriKind.Relative);
                string parameters = String.Format("subject={0}&body={1}&recipient={2}", 
                    txt_subject.Text,txt_message.Text,recipient);
                HttpConnection.httpPost(path, parameters, message_sent);
                MessageBox.Show("Message sent");
            }
        }

        public void message_sent(object sender, UploadStringCompletedEventArgs e)
        {/*
            if (e.Error == null)
            MessageBox.Show(e.Result);
            else
                MessageBox.Show(e.Error.ToString());*/
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            // Get the player's messages
            Uri path = new Uri("messages_get_message.php", UriKind.Relative);
            HttpConnection.httpGet(path, fill_messages);
        }
    }

    public class Message
    {
        public int id;
        public string sender;
        public string subject;
        public string message;
        public int read;
        public CheckBox selected;
        public Label sender_label;
        public Label subject_label;
        public bool is_selected;
    }

    public class Contact
    {
        public string name;
        public int cid;
    }
}


