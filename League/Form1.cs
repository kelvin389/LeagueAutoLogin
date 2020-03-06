using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LCUSharp;
using Newtonsoft.Json.Linq;

namespace League
{
    public partial class Form1 : Form
    {
        // workaround to be able to call non static methods inside a static method
        public static Form form;

        private static ChampionForm ChampForm = new ChampionForm();
        private static int SelectingSlot = 0;
        private static int[] PreferredChamps = new int[5] { -1, -1, -1, -1, -1};

        private delegate void SafeCallDelegate(string label, string text);

        // event handlers
        private event EventHandler<LeagueEvent> ChampSelectSessionUpdated;
        private event EventHandler<LeagueEvent> GameFlowUpdated;
        
        private static List<int> UnavailableChampsID = new List<int>();

        private bool inChampSelect = false;

        private bool autoAcceptQueue = false;

        public Form1()
        {
            InitializeComponent();
            form = this;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Start();
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            LeagueEventHandler.UnsubscribeSocket();
            Console.ReadKey();
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
        }

        private void OnGameFlowUpdate(object sender, LeagueEvent e)
        {
            string phase = e.Data.ToString();

            if (phase == "ReadyCheck" && autoAcceptQueue)
            {
                API.client.MakeApiRequest(HttpMethod.Post, "/lol-matchmaking/v1/ready-check/decline");
            }
            // returned to lobby for whatever reason
            else if (inChampSelect && phase == "Lobby")
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
            if (!e.Data["actions"].HasValues) return;

            WriteSafe("formdebug", e.Data.ToString());
            JToken bans = e.Data["bans"];
            var ourBans = bans["myTeamBans"];
            var theirBans = bans["theirTeamBans"];
            int localCellId = Convert.ToInt32(e.Data["localPlayerCellId"]);

            // empty bans array is "[]". we only want to parse bans if there are any
            if (ourBans.ToString().Length > 2) ParseBans(ourBans);
            if (theirBans.ToString().Length > 2) ParseBans(theirBans);

            JToken actionstop = e.Data["actions"];

            var actionslist = actionstop.First().Children();
            var curchild = actionslist.First();

            // iterate through all actions
            for (int i = 0; i < actionslist.Count(); i++)
            {
                int curCellId = Convert.ToInt32(curchild["actorCellId"]);
                int actionId = Convert.ToInt32(curchild["id"]);
                bool myTurn = Convert.ToBoolean(curchild["isInProgress"]);

                if (curCellId == localCellId && myTurn)
                {
                    for (int j = 0; j < PreferredChamps.Length; j++)
                    {
                        if (PreferredChamps[j] != -1 && !UnavailableChampsID.Contains(PreferredChamps[j]))
                        {
                            // lock in based on order of prefernce
                            string str = "{\"actorCellId\": " + curCellId + ", \"championId\":" + PreferredChamps[j] + ", \"completed\": true, \"id\": " + actionId + ", \"type\": \"string\"}";
                            API.client.MakeApiRequest(HttpMethod.Patch, "/lol-champ-select/v1/session/actions/" + actionId, str);
                            break;
                        }
                    }
                }
            }
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

        public static void ChampionSelected()
        {
            ChampForm.Close();
            PreferredChamps[SelectingSlot] = ChampForm.selectedId;

            string label = "ChampPref" + SelectingSlot;
            Label lb = form.Controls.Find(label, false).FirstOrDefault() as Label;
            lb.Text = ChampForm.selectedName;
        }

        private void ChampPref0_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 0;
            ChampForm.Show();
        }

        private void ChampPref1_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 1;
            ChampForm.Show();
        }

        private void ChampPref2_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 2;
            ChampForm.Show();
        }

        private void ChampPref3_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 3;
            ChampForm.Show();
        }

        private void ChampPref4_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 4;
            ChampForm.Show();
        }

    }

    static class API
    {
        public static ILeagueClient client;
    }

}

