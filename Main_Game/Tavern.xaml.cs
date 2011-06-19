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
        Grid grd_quests;
        List<int> invites = new List<int>();
        bool only = true;
        int host;
        Quest[] quests;
        int pollcount = 0;

        public Tavern(bool success, int reward)
        {
            if (success)
            {
                // The player has just returned from a mission, so reward them
                MessageBox.Show("You have completed the quest and been awarded " + reward
                    + " experience points");
            }
            Character.currentCharacter.restoreCharacter();
            // Update the player's location
            Uri path = new Uri("dungeon_host_exit.php", UriKind.Relative);
            HttpConnection.httpGet(path, start_poll_tavern);
            // Make sure they appear in the tavern 
            path = new Uri("enterWorld.php", UriKind.Relative);
            HttpConnection.httpGet(path, check_success);
            InitializeComponent();
            // Grid to store the players
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
            
            // Grid to store the quests
            grd_quests = new Grid();
            new_def = new ColumnDefinition();
            new_def.Width = new GridLength(50);
            grd_quests.ColumnDefinitions.Add(new_def);
            new_def = new ColumnDefinition();
            new_def.Width = new GridLength(200);
            grd_quests.ColumnDefinitions.Add(new_def);
            grd_quests.HorizontalAlignment = HorizontalAlignment.Left;
            grd_quests.VerticalAlignment = VerticalAlignment.Top;
            grd_quests.Margin = new Thickness(1, 1, 0, 0);
            grd_quests.Height = 100;
            grd_quests.Width = 250;
            scv_quests.Content = grd_quests;

            txt_title.Visibility = Visibility.Collapsed;
            txt_description.Visibility = Visibility.Collapsed;
            rectangle1.Visibility = Visibility.Collapsed;
            rectangle2.Visibility = Visibility.Collapsed;

            // Get a selection of missions
            path = new Uri("tavern_get_missions.php", UriKind.Relative);
            HttpConnection.httpGet(path, quests_recieved);
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
            Uri path = new Uri("tavern_all_invites.php", UriKind.Relative);
            HttpConnection.httpGet(path, check_success);
            for (int i = 0; i < quests.Length; i++)
            {
                if (quests[i].chosen)
                {
                    Quest q = quests[i];
                    ScreenManager.SetScreen(new MapScreen(true, only, q.theme,q.size,q.monsters,
                        q.items,q.light_level,q.reward));
                }
            }
        }

        public void check_success(object sender, DownloadStringCompletedEventArgs e)
        {
        }

        public void lobby(object sender, DownloadStringCompletedEventArgs e)
        {
            pollcount++;
            lbl_poll_check.Content = pollcount;
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

        private void quests_recieved(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    XDocument doc = XDocument.Parse(e.Result);
                    // Check if any missions were found
                    if (doc.Element("notfound") == null)
                    {
                        var missions = from mission in doc.Descendants("mission")
                                             select new Quest()
                                             {
                                                 title = (string)mission.Element("title"),
                                                 description = (string)mission.Element("description"),
                                                 size = (int)mission.Element("size"),
                                                 theme = (int)mission.Element("theme"),
                                                 light_level = (int)mission.Element("light"),
                                                 monsters  = (int)mission.Element("monster"),
                                                 items  = (int)mission.Element("item"),
                                                 reward = (int)mission.Element("reward")
                                             };
                        this.quests = missions.ToArray();
                        fill_quests();
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

        private void fill_quests()
        {
            grd_quests.Children.Clear();
            grd_quests.Height = quests.Length * 25;
            while (grd_quests.RowDefinitions.Count < quests.Length)
            {
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(25);
                grd_quests.RowDefinitions.Add(newrow);
            }
            for (int i = 0; i < quests.Length; i++)
            {
                quests[i].selected = new RadioButton();
                quests[i].selected.Content = "";
                Grid.SetColumn(quests[i].selected, 0);
                Grid.SetRow(quests[i].selected, i);
                grd_quests.Children.Add(quests[i].selected);
                quests[i].selected.Tag = i;
                quests[i].selected.Checked += select_quest;
                quests[i].display_title = new Label();
                quests[i].display_title.Content = quests[i].title;
                Grid.SetColumn(quests[i].display_title, 1);
                Grid.SetRow(quests[i].display_title, i);
                grd_quests.Children.Add(quests[i].display_title);
                quests[i].display_title.Tag = i;
                quests[i].display_title.MouseEnter += display_quest;
            }
        }

        public void select_quest(object sender, RoutedEventArgs e)
        {
            int index = Int32.Parse((sender as RadioButton).Tag.ToString());
            view_quest(index);
            btn_mission.IsEnabled = true;
            for (int i = 0; i < quests.Length; i++)
            {
                if (i == index)
                    quests[i].chosen = true;
                else
                    quests[i].chosen = false;
            }
        }

        public void display_quest(object sender, RoutedEventArgs e)
        {
            int index = Int32.Parse((sender as Label).Tag.ToString());
            view_quest(index);
        }

        public void view_quest(int index)
        {
            txt_title.Text = quests[index].title;
            txt_description.Text = quests[index].description;
            txt_title.Visibility = Visibility.Visible;
            txt_description.Visibility = Visibility.Visible;
            rectangle1.Visibility = Visibility.Visible;
            rectangle2.Visibility = Visibility.Visible;
        }

        private void btn_add_quest_Click(object sender, RoutedEventArgs e)
        {
            Uri path = new Uri("add_quests.php", UriKind.Relative);
            HttpConnection.httpGet(path, quests_added);
        }

        private void quests_added(object sender, DownloadStringCompletedEventArgs e)
        {
            MessageBox.Show(e.Result);
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
        public int light_level;
        public int monsters;
        public int items;
        public int reward;
        public bool chosen;
        public RadioButton selected;
        public Label display_title;
    }
}