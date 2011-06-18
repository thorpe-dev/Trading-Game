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
    public partial class Tavern : UserControl, IScreen
    {
        Player[] players;
        Grid grd_players;
        List<int> invites = new List<int>();
        bool only = true;
        int host;

        public Tavern()
        {
            
            // Update the player's location
            Uri path = new Uri("dungeon_host_exit.php", UriKind.Relative);
            HttpConnection.httpGet(path, start_poll_tavern);
            
            InitializeComponent();
            grd_players = new Grid();
            ColumnDefinition new_def = new ColumnDefinition();
            new_def.Width = new GridLength(125);
            grd_players.ColumnDefinitions.Add(new_def);
            new_def = new ColumnDefinition();
            new_def.Width = new GridLength(50);
            grd_players.ColumnDefinitions.Add(new_def);
            new_def = new ColumnDefinition();
            new_def.Width = new GridLength(50);
            grd_players.ColumnDefinitions.Add(new_def);
            grd_players.HorizontalAlignment = HorizontalAlignment.Left;
            grd_players.VerticalAlignment = VerticalAlignment.Top;
            grd_players.Margin = new Thickness(1, 1, 0, 0);
            grd_players.Height = 100;
            grd_players.Width = 250;
            scv_players.Content = grd_players;
            /*
            // Test shit...
            players = new Player[1];
            players[0] = new Player();
            players[0].username = "AAAAAAAAAAAAAAAA";
            players[0].pid = 1;
            Label player1 = new Label();
            player1.Width = 150;
            player1.Height = 25;
            player1.Content = "AAAAAAAAAAAAAAAA";
            Grid.SetRow(player1, 0);
            Grid.SetColumn(player1, 0);
            grd_players.Children.Add(player1);
            Button player1btn = new Button();
            player1btn.Click += new RoutedEventHandler(btn_invite_handler);
            player1btn.Tag = 0;
            player1btn.Content = "Invite";
            player1btn.Width = 50;
            player1btn.Height = 25;
            Grid.SetRow(player1btn, 0);
            Grid.SetColumn(player1btn, 1);
            grd_players.Children.Add(player1btn);
            players[0].invite = player1btn;
            //grd_players.Children.Clear();*/
        }

        public UIElement Element { get { return this; } }

        private void start_poll_tavern(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Uri path = new Uri("lobby.php", UriKind.Relative);
                try
                {
                    HttpConnection.httpLongPoll(path, lobby, 2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show(e.Error.ToString());
            }
        }
      
        private void btn_mission_Click(object sender, RoutedEventArgs e)
        {
            Uri path = new Uri("tavern_remove_all.php", UriKind.Relative);
            HttpConnection.httpGet(path, check_success);
            ScreenManager.SetScreen(new MapScreen(0,
                                                  0,
                                                  (true),
                                                  (only)));
        }

        public void check_success(object sender, DownloadStringCompletedEventArgs e)
        {
        }

        public void lobby(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    XDocument doc = XDocument.Parse(e.Result);
                    // Check if any other players were found
                    if (doc.Element("notfound") == null)
                    {
                        var lobby_players = from player in doc.Descendants("player")
                                            select new Player()
                                            {
                                                cid = (int)player.Element("cid"),
                                                username = (string)player.Element("name"),
                                                invited_by = (int)player.Element("invited")
                                            };
                        grd_players.Children.Clear();
                        players = lobby_players.ToArray();
                        if (grd_players.RowDefinitions.Count < players.Length)
                        {
                            while (grd_players.RowDefinitions.Count < players.Length)
                            {
                                RowDefinition new_def = new RowDefinition();
                                new_def.Height = new GridLength(25);
                                grd_players.RowDefinitions.Add(new_def);
                            }
                        }
                        grd_players.Height = 25 * players.Length;
                        for (int i = 0; i < players.Length; i++)
                        {
                            // The name in the grid
                            Label new_lbl = new Label();
                            new_lbl.Width = 125;
                            new_lbl.Height = 25;
                            new_lbl.Content = players[i].username;
                            Grid.SetRow(new_lbl, i);
                            Grid.SetColumn(new_lbl, 0);
                            grd_players.Children.Add(new_lbl);

                            if (players[i].invited_by == 1)
                            {
                                // The 'join' button
                                Button new_btn = new Button();
                                new_btn.Tag = i;
                                new_btn.Click += new RoutedEventHandler(btn_join_handler);
                                new_btn.Content = "Join";
                                new_btn.Width = 50;
                                new_btn.Height = 25;
                                Grid.SetRow(new_btn, i);
                                Grid.SetColumn(new_btn, 2);
                                grd_players.Children.Add(new_btn);
                                players[i].join = new_btn;
                            }
                            else
                            {
                                // The 'invite' button
                                Button new_btn = new Button();
                                new_btn.Tag = i;
                                if (!invites.Contains(players[i].cid))
                                {
                                    new_btn.Click += new RoutedEventHandler(btn_invite_handler);
                                    new_btn.Content = "Invite";
                                }
                                else
                                {
                                    new_btn.Click += new RoutedEventHandler(btn_remove_handler);
                                    new_btn.Content = "Remove";
                                }
                                new_btn.Width = 50;
                                new_btn.Height = 25;
                                Grid.SetRow(new_btn, i);
                                Grid.SetColumn(new_btn, 1);
                                grd_players.Children.Add(new_btn);
                                players[i].invite = new_btn;
                            }


                            players[i].show_name = new_lbl;
                        }
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show("Error: " + e.Result + e1.ToString());
                }
            }
            else
            {
                //txt_test.Text = "ERROR in results" + e.Error.ToString();
            }
        }

        private void btn_invite_handler(object sender, RoutedEventArgs e)
        {
            int index = Int32.Parse((sender as Button).Tag.ToString());
            string name = players[index].username;
            // Invite them...
            Uri path = new Uri("tavern_invite.php", UriKind.Relative);
            string parameters = String.Format("target={0}", players[index].cid);
            HttpConnection.httpPost(path, parameters, invite_update);
        }

        private void invite_update(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    XDocument doc = XDocument.Parse(e.Result);
                    int cid = (int)doc.Element("cid");
                    // Update the UI
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i].cid == cid)
                        {
                            players[i].invite.Content = "Remove";
                            players[i].invite.Click -= btn_invite_handler;
                            players[i].invite.Click += new RoutedEventHandler(btn_remove_handler);
                            if (invites.Count == 0)
                            {
                                // Add the player to the party table
                                only = false;
                                Uri path = new Uri("tavern_start_party.php", UriKind.Relative);
                                HttpConnection.httpGet(path, party_started);
                            }
                            invites.Add(cid);
                        }
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show("Parse error: " + e1.ToString());
                }
            }
            else
            {
                MessageBox.Show("SendInvite Fail: " + e.Error.ToString());
            }
        }

        private void party_started(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //MessageBox.Show("Result of party: " + e.Result);
            }
            else
            {
                MessageBox.Show("Could not START PARTY: " + e.Error.ToString());
            }
        }

        private void btn_remove_handler(object sender, RoutedEventArgs e)
        {
            int index = Int32.Parse((sender as Button).Tag.ToString());
            string name = players[index].username;
            // Remove an invite
            Uri path = new Uri("tavern_remove_invite.php", UriKind.Relative);
            string parameters = String.Format("target={0}", players[index].cid);
            HttpConnection.httpPost(path, parameters, remove_invite_update);
        }

        private void remove_invite_update(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    XDocument doc = XDocument.Parse(e.Result);
                    int cid = (int)doc.Element("cid");
                    // Update the UI
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i].cid == cid)
                        {
                            players[i].invite.Content = "Invite";
                            players[i].invite.Click -= btn_remove_handler;
                            players[i].invite.Click += new RoutedEventHandler(btn_invite_handler);
                            invites.Remove(cid);
                            if (invites.Count == 0)
                            {
                                // Remove the player from the party table
                                only = true;
                                Uri path = new Uri("tavern_remove_all.php", UriKind.Relative);
                                HttpConnection.httpGet(path, party_started);
                            }
                        }
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show("Parse error: " + e1.ToString());
                }
            }
            else
            {
                MessageBox.Show("Invite remove error: " + e.Error.ToString());
            }
        }

        private void btn_join_handler(object sender, RoutedEventArgs e)
        {
            int index = Int32.Parse((sender as Button).Tag.ToString());
            this.host = players[index].cid;
            //MessageBox.Show("Trying to join " + host);
            // Remove all their invites and party up....
            invites.Clear();
            Uri path = new Uri("tavern_remove_all.php", UriKind.Relative);
            HttpConnection.httpGet(path, confirm_invite);
        }

        private void confirm_invite(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Uri path = new Uri("tavern_accept_invite.php", UriKind.Relative);
                string parameters = String.Format("hostid={0}", host);
                HttpConnection.httpPost(path, parameters, start_poll_party);
            }
            else
            {
                MessageBox.Show(e.Error.ToString());
            }
        }

        private void start_poll_party(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XDocument doc = XDocument.Parse(e.Result);
                // Check if the invite still stands
                if (doc.Element("error") == null)
                {
                    TavernWaitForHost wait = new TavernWaitForHost(host);
                    wait.Show();
                }
            }
            else
            {
                MessageBox.Show(e.Error.ToString());
            }
        }

    }

    public class Player
    {
        public int cid;
        public string username;
        public Label show_name;
        public Button invite;
        public Button join;
        public int invited_by;
    }

    public class Quest
    {
        public string title;
        public string description;
        public int size;
        public int theme;
        public int ligth_level;
        public int monsters;
        public int items;
        public int reward;
    }
}