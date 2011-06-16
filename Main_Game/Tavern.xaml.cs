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
using System.Windows.Browser;

namespace Main_Game
{
    public partial class Tavern : UserControl, IScreen
    {
        Player[] players;
        Grid grd_players;
        List<int> invites = new List<int>();
        bool first = true;
        bool only = true;

        public Tavern()
        {
            string cookie = HtmlPage.Document.Cookies;
            MessageBox.Show(cookie);
            char[] cookiearray = cookie.ToCharArray();
            for (int i = 0; i < cookiearray.Length; i++)
            {

            }

            /*
            // Update the player's location
            Uri path = new Uri("enter_tavern.php", UriKind.Relative);
            string parameters = String.Format("charnum={0}", );
            HttpConnection.httpPost(path, parameters, start_poll_tavern);
            */
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
            grd_players.Width = 200;
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

            
      
        private void btn_mission_Click(object sender, RoutedEventArgs e)
        {
            MapScreen tDungeon = new MapScreen(Int32.Parse(txt_id.Text),
                                                  Int32.Parse(txt_dungeon.Text),
                                                  (first),
                                                  (only));
            ScreenManager.SetScreen(tDungeon);
            tDungeon.Focus();
        }

        public void lobby(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                txt_test.Text = e.Result.ToString();
                try
                {
                    XDocument doc = XDocument.Parse(e.Result);
                    var lobby_players = from player in doc.Descendants("player")
                                        select new Player()
                                        {
                                            pid = (int)player.Element("pid"),
                                            username = (string)player.Element("username")
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
                    txt_test.Text = "Results:\n";
                    for (int i = 0; i < players.Length; i++)
                    {
                        Label new_lbl = new Label();
                        new_lbl.Width = 125;
                        new_lbl.Height = 25;
                        new_lbl.Content = players[i].username;
                        Grid.SetRow(new_lbl, i);
                        Grid.SetColumn(new_lbl, 0);
                        grd_players.Children.Add(new_lbl);

                        Button new_btn = new Button();
                        new_btn.Tag = i;
                        if (!invites.Contains(players[i].pid))
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

                        players[i].show_name = new_lbl;
                        players[i].invite = new_btn;
                        txt_test.Text = txt_test.Text + players[i].username + "\n";
                    }
                }
                catch (Exception e1)
                {
                    txt_test.Text = "Error in parsing: " + e1.ToString();
                }
            }
            else
            {
                txt_test.Text = "ERROR in results" + e.Error.ToString();
            }
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
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

        private void btn_invite_handler(object sender, RoutedEventArgs e)
        {
            int index = Int32.Parse((sender as Button).Tag.ToString());
            string name = players[index].username;
            // Invite them...
            MessageBox.Show("Sent invite to " + name + " (player number " + players[index].pid + ")");
            players[index].invite.Content = "Remove";
            players[index].invite.Click -= btn_invite_handler;
            players[index].invite.Click += new RoutedEventHandler(btn_remove_handler);
            invites.Add(players[index].pid);
        }

        private void btn_remove_handler(object sender, RoutedEventArgs e)
        {
            int index = Int32.Parse((sender as Button).Tag.ToString());
            string name = players[index].username;
            // Remove an invite
            MessageBox.Show("Removed invite to " + name);
            players[index].invite.Content = "Invite";
            players[index].invite.Click -= btn_remove_handler;
            players[index].invite.Click += new RoutedEventHandler(btn_invite_handler);
            invites.Remove(players[index].pid);
        }

        private void btn_clear_list_Click(object sender, RoutedEventArgs e)
        {
            txt_test.Text = "";
        }

        private void chk_fp_Checked(object sender, RoutedEventArgs e)
        {
            first = true;
        }

        private void chk_op_Checked(object sender, RoutedEventArgs e)
        {
            only = true;
        }

        private void chk_op_Unchecked(object sender, RoutedEventArgs e)
        {
            only = false;
        }

        private void chk_fp_Unchecked(object sender, RoutedEventArgs e)
        {
            first = false;
        }

    }
    /*
    public class PlayerInvite
    {
        string player_name;
        bool invited;

        public PlayerInvite(string name)
        {
            this.player_name = name;
            this.invited = false;
        }
    }
    */
    public class Player
    {
        public int pid;
        public string username;
        public Label show_name;
        public Button invite;
        //public bool invited;
    }
}