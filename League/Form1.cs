using System;
using System.Collections.Generic;
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
        private event EventHandler<LeagueEvent> ChampSelectSessionUpdated;

        private static List<int> UnavailableChampsID = new List<int>();

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
            WriteSafe("formdebug", e.Data.ToString());
            JToken bans = e.Data["bans"];
            var ourBans = bans["myTeamBans"];
            var theirBans = bans["theirTeamBans"];

            // empty bans array is "[]". we only want to parse bans if there are any
            if (ourBans.ToString().Length > 2) ParseBans(ourBans);
            if (theirBans.ToString().Length > 2) ParseBans(theirBans);
        }

        private static void ParseBans(JToken data)
        {
            string[] bansStrArr = null;

            // clean up data
            data = data.ToString().Replace("[", "");
            data = data.ToString().Replace("]", "");
            data.ToString().Trim();

            // break up into string array
            bansStrArr = data.ToString().Split(',');

            // add missing bans to unavailable champs
            for (int i = 0; i < bansStrArr.Length; i++)
            {
                int id = Convert.ToInt32(bansStrArr[i]);

                if (!UnavailableChampsID.Contains(id))
                {
                    UnavailableChampsID.Add(id);
                }
            }
        }

        private void WriteSafe(string label, string text)
        {
            RichTextBox lb = Controls.Find(label, false).FirstOrDefault() as RichTextBox;

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
