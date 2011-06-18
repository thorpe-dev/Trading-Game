using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Windows.Threading;

namespace Main_Game
{
    public class HttpConnection
    {
        private static DispatcherTimer timer;
        private static PollClient pollClient;

        public static void httpGet(Uri resource, DownloadStringCompletedEventHandler e)
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += e;
            client.DownloadStringAsync(resource);
        }

        public static void httpPost(Uri resource, string postString, UploadStringCompletedEventHandler e)
        {
            WebClient client = new WebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            client.UploadStringCompleted += e;
            client.UploadStringAsync(resource, "POST", postString);
        }

        public static void httpLongPoll(Uri resource, DownloadStringCompletedEventHandler e, int pollInterval)
        {
            try
            {
                if (timer != null)
                {
                    timer.Stop();
                }
                WebClient client = new WebClient();
                client.DownloadStringCompleted += e;
                pollClient = new PollClient()
                {
                    client = client,
                    resource = resource
                };
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, pollInterval, 0);
                timer.Tick += new EventHandler(tickRequest);
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private static void tickRequest(Object state, EventArgs sender)
        {
            try
            {
                if (!pollClient.client.IsBusy)
                {
                    pollClient.client.DownloadStringAsync(pollClient.resource);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private class PollClient
        {
            public WebClient client;
            public Uri resource;
        }

        public static void stopPolling()
        {
            timer.Stop();
        }
    }
}
