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
        private event EventHandler<LeagueEvent> GameFlowUpdated;
        private event EventHandler<LeagueEvent> ReadyCheckPopped;

        private static List<int> UnavailableChampsID = new List<int>();

        private bool inChampSelect = false;

        private bool autoAcceptQueue = false;

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

            // connect to client by path
            API.client = await LeagueClient.Connect();

            // hook onto game closing event
            API.client.LeagueClosed += GameClosed;

            status.Text = "Connecting to client...";

            // subscribe to all events via websocket
            LeagueEventHandler.Connect();

            status.Text = "Running";

            // check if we are already in a champ select
            var gameflow = await API.client.MakeApiRequest(HttpMethod.Get, "/lol-gameflow/v1/gameflow-phase");
            string phase = await gameflow.Content.ReadAsStringAsync();
            phase = phase.Replace("\"", ""); // remove quotes
            if (phase == "ChampSelect") inChampSelect = true;

            // subscribe to events
            ChampSelectSessionUpdated += OnChampSelectSessionUpdate;
            LeagueEventHandler.Subscribe("/lol-champ-select/v1/session", ChampSelectSessionUpdated);

            GameFlowUpdated += OnGameFlowUpdate;
            LeagueEventHandler.Subscribe("/lol-gameflow/v1/gameflow-phase", GameFlowUpdated);

            ReadyCheckPopped += OnReadyCheckPop;
            LeagueEventHandler.Subscribe("/lol-gameflow/v1/gameflow-phase", ReadyCheckPopped);
        }

        private void OnReadyCheckPop(object sender, LeagueEvent e)
        {
            if (autoAcceptQueue)
            {
                API.client.MakeApiRequest(HttpMethod.Post, "/lol-matchmaking/v1/ready-check/decline");
            }
        }

        private void OnGameFlowUpdate(object sender, LeagueEvent e)
        {
            string phase = e.Data.ToString();
            
            // returned to lobby for whatever reason
            if (inChampSelect && phase == "Lobby")
            {
                inChampSelect = false;
                UnavailableChampsID.Clear();
            }
            else if (!inChampSelect && phase == "ChampSelect") 
            {
                inChampSelect = true;
            }
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

        private void autoAccept_CheckedChanged(object sender, EventArgs e)
        {
            autoAcceptQueue = autoAccept.Checked;
        }
    }

    static class API
    {
        public static ILeagueClient client;
    }

}
