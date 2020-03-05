using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using LCUSharp;
using Newtonsoft.Json.Linq;

namespace League
{
    public partial class Form1 : Form
    {
        private delegate void SafeCallDelegate(string label, string text);

        // event handlers
        public event EventHandler<LeagueEvent> ChampSelectSessionUpdated;

        enum ChampionID
        {
            
        };
        public Form1()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(Form1_Closing);
            Start();
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            // clean up before closing
            LeagueEventHandler.UnsubscribeSocket();
        }

        private async void Start()
        {
            status.Text = "Waiting for League...";
            Console.WriteLine("Waiting for League...");

            // connect to client by path
            API.client = await LeagueClient.Connect();

            // hook onto game closing event
            API.client.LeagueClosed += GameClosed;

            status.Text = "Connecting to client...";

            // subscribe to events via websocket
            LeagueEventHandler.Connect();

            status.Text = "Running";

            ChampSelectSessionUpdated += OnChampSelectSessionUpdate;
            LeagueEventHandler.Subscribe("/lol-champ-select/v1/session", ChampSelectSessionUpdated);
        }

        private void OnChampSelectSessionUpdate(object sender, LeagueEvent e)
        {
            //Console.WriteLine(e.Data.ToString());
            
            WriteSafe("status", e.Data.ToString());
        }

        private void WriteSafe(string label, string text)
        {
            Label lb = Controls.Find(label, false).FirstOrDefault() as Label;

            if (lb.InvokeRequired)
            {
                var d = new SafeCallDelegate(WriteSafe);
                lb.Invoke(d, new object[] { label, text });
            }
            else
            {
                lb.Text = text;
            }
        }


        private void GameClosed()
        {
            // this takes like 5 seconds after the game closes to trigger
            // something to do with it being on a different thread?
            Application.Exit();
        }

    }

    static class API
    {
        public static ILeagueClient client;
    }

}
