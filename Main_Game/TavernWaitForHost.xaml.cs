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
    public partial class TavernWaitForHost : ChildWindow
    {
        public int hostid;
        public int ellipses = 0;

        public TavernWaitForHost(int host)
        {
            this.hostid = host;
            InitializeComponent();
            Uri path = new Uri("tavern_party_poll.php", UriKind.Relative);
            try
            {
                HttpConnection.httpLongPoll(path, check_host_ready, 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void check_host_ready(object sender, DownloadStringCompletedEventArgs e)
        {
            string waiting = "Waiting for host";
            switch (ellipses)
            {
                case 0: lbl_waiting.Content = waiting + "."; ellipses++;
                    break;
                case 1: lbl_waiting.Content = waiting + ".."; ellipses++;
                    break;
                case 2: lbl_waiting.Content = waiting + "..."; ellipses++;
                    break;
                case 3: lbl_waiting.Content = waiting; ellipses = 0;
                    break;
            }
            if (e.Error == null)
            {
                try
                {
                    XDocument doc = XDocument.Parse(e.Result);
                    // Check if the host has disconnected
                    if (doc.Element("error") != null)
                    {
                        MessageBox.Show((string)doc.Element("error"));
                        HttpConnection.stopPolling();
                        CancelButton_Click(null,null);
                    }
                    else if (doc.Element("success") != null)
                    {
                        goto_dungeon();
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.ToString());
                }
            }
            else
            {
                MessageBox.Show(e.Error.ToString());
            }
        }

        private void goto_dungeon()
        {
            //MessageBox.Show("Mr LaForge - Engage!");
            HttpConnection.stopPolling();
            this.OKButton.IsEnabled = true;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenManager.SetScreen(new MapScreen(false,false,0,0,0,0,0,0));
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

