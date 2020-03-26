using LCUSharp;
using LCUSharp.DataObjects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace League
{
    public partial class Form1 : Form
    {
        // workaround to be able to call non static methods inside a static method
        public static Form form;

        private static ChampionForm ChampForm = new ChampionForm();
        private static RunesForm RuneForm = new RunesForm();

        private static int SelectingSlot = 0;
        private static int CurrentRole = 0;

        private static int PrefPoolSize = 5;
        private static int[] PreferredTops = new int[PrefPoolSize];
        private static int[] PreferredJgs = new int[PrefPoolSize];
        private static int[] PreferredMids = new int[PrefPoolSize];
        private static int[] PreferredADCs = new int[PrefPoolSize];
        private static int[] PreferredSups = new int[PrefPoolSize];
        private static int[] PreferredGeneral = new int[PrefPoolSize];
        private static int[] PreferredBans = new int[3];

        private static string[] RunesTop = new string[PrefPoolSize];
        private static string[] RunesJg = new string[PrefPoolSize];
        private static string[] RunesMid = new string[PrefPoolSize];
        private static string[] RunesADC = new string[PrefPoolSize];
        private static string[] RunesSup = new string[PrefPoolSize];
        private static string[] RunesGeneral = new string[PrefPoolSize];

        private static bool selectingBan = false;

        private delegate void SafeCallDelegate(string label, string text);

        // event handlers
        private event EventHandler<LeagueEvent> ChampSelectSessionUpdated;
        private event EventHandler<LeagueEvent> GameFlowUpdated;

        private static List<int> UnavailableChampsID = new List<int>();
        private static List<int> HandledActionIDs = new List<int>();

        private bool autoAcceptQueue = false;
        private bool autoChampSelect = false;

        public Form1()
        {
            InitializeComponent();
            form = this;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            comboBox1.SelectedIndex = 0;
            Start();
            RuneForm.ParseRunesJson();
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            LeagueEventHandler.UnsubscribeSocket();
        }

        private async void Start()
        {
            LoadSettings();

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

            // subscribe to events
            ChampSelectSessionUpdated += OnChampSelectSessionUpdate;
            LeagueEventHandler.Subscribe("/lol-champ-select/v1/session", ChampSelectSessionUpdated);

            GameFlowUpdated += OnGameFlowUpdate;
            LeagueEventHandler.Subscribe("/lol-gameflow/v1/gameflow-phase", GameFlowUpdated);
        }

        private void LoadSettings()
        {
            // get saved settings
            PreferredGeneral[0] = Properties.Settings.Default.general0;
            PreferredGeneral[1] = Properties.Settings.Default.general1;
            PreferredGeneral[2] = Properties.Settings.Default.general2;
            PreferredGeneral[3] = Properties.Settings.Default.general3;
            PreferredGeneral[4] = Properties.Settings.Default.general4;

            PreferredTops[0] = Properties.Settings.Default.top0;
            PreferredTops[1] = Properties.Settings.Default.top1;
            PreferredTops[2] = Properties.Settings.Default.top2;
            PreferredTops[3] = Properties.Settings.Default.top3;
            PreferredTops[4] = Properties.Settings.Default.top4;

            PreferredJgs[0] = Properties.Settings.Default.jg0;
            PreferredJgs[1] = Properties.Settings.Default.jg1;
            PreferredJgs[2] = Properties.Settings.Default.jg2;
            PreferredJgs[3] = Properties.Settings.Default.jg3;
            PreferredJgs[4] = Properties.Settings.Default.jg4;

            PreferredMids[0] = Properties.Settings.Default.mid0;
            PreferredMids[1] = Properties.Settings.Default.mid1;
            PreferredMids[2] = Properties.Settings.Default.mid2;
            PreferredMids[3] = Properties.Settings.Default.mid3;
            PreferredMids[4] = Properties.Settings.Default.mid4;

            PreferredADCs[0] = Properties.Settings.Default.adc0;
            PreferredADCs[1] = Properties.Settings.Default.adc1;
            PreferredADCs[2] = Properties.Settings.Default.adc2;
            PreferredADCs[3] = Properties.Settings.Default.adc3;
            PreferredADCs[4] = Properties.Settings.Default.adc4;

            PreferredSups[0] = Properties.Settings.Default.sup0;
            PreferredSups[1] = Properties.Settings.Default.sup1;
            PreferredSups[2] = Properties.Settings.Default.sup2;
            PreferredSups[3] = Properties.Settings.Default.sup3;
            PreferredSups[4] = Properties.Settings.Default.sup4;

            PreferredBans[0] = Properties.Settings.Default.ban0;
            PreferredBans[1] = Properties.Settings.Default.ban1;
            PreferredBans[2] = Properties.Settings.Default.ban2;

            RunesTop[0] = Properties.Settings.Default.topRunes0;
            RunesTop[1] = Properties.Settings.Default.topRunes1;
            RunesTop[2] = Properties.Settings.Default.topRunes2;
            RunesTop[3] = Properties.Settings.Default.topRunes3;
            RunesTop[4] = Properties.Settings.Default.topRunes4;

            RunesJg[0] = Properties.Settings.Default.jgRunes0;
            RunesJg[1] = Properties.Settings.Default.jgRunes1;
            RunesJg[2] = Properties.Settings.Default.jgRunes2;
            RunesJg[3] = Properties.Settings.Default.jgRunes3;
            RunesJg[4] = Properties.Settings.Default.jgRunes4;

            RunesMid[0] = Properties.Settings.Default.midRunes0;
            RunesMid[1] = Properties.Settings.Default.midRunes1;
            RunesMid[2] = Properties.Settings.Default.midRunes2;
            RunesMid[3] = Properties.Settings.Default.midRunes3;
            RunesMid[4] = Properties.Settings.Default.midRunes4;

            RunesADC[0] = Properties.Settings.Default.adcRunes0;
            RunesADC[1] = Properties.Settings.Default.adcRunes1;
            RunesADC[2] = Properties.Settings.Default.adcRunes2;
            RunesADC[3] = Properties.Settings.Default.adcRunes3;
            RunesADC[4] = Properties.Settings.Default.adcRunes4;

            RunesSup[0] = Properties.Settings.Default.supRunes0;
            RunesSup[1] = Properties.Settings.Default.supRunes1;
            RunesSup[2] = Properties.Settings.Default.supRunes2;
            RunesSup[3] = Properties.Settings.Default.supRunes3;
            RunesSup[4] = Properties.Settings.Default.supRunes4;

            RunesGeneral[0] = Properties.Settings.Default.generalRunes0;
            RunesGeneral[1] = Properties.Settings.Default.generalRunes1;
            RunesGeneral[2] = Properties.Settings.Default.generalRunes2;
            RunesGeneral[3] = Properties.Settings.Default.generalRunes3;
            RunesGeneral[4] = Properties.Settings.Default.generalRunes4;

            CurrentRole = Properties.Settings.Default.curRole;

            UpdateChampionLabels();
            UpdateBanLabels();
        }

        private void UpdateChampionLabels()
        {
            int[] currentPrefs = GetChampionPrefsByRoleID(CurrentRole);
            // update champion pref labels to previous settings labels
            for (int i = 0; i < PrefPoolSize; i++)
            {
                string labelname = "ChampPref" + i;

                Label lb = Controls.Find(labelname, false).First() as Label;
                lb.Text = Champion.IDtoName(currentPrefs[i]);
            }
        }

        private void UpdateBanLabels()
        {
            // update ban pref labels to previous settings labels
            for (int i = 0; i < PreferredBans.Length; i++)
            {
                string labelname = "Ban" + i;

                Label lb = Controls.Find(labelname, false).First() as Label;
                lb.Text = Champion.IDtoName(PreferredBans[i]);
            }
        }

        private static int[] GetChampionPrefsByRoleID(int roleID)
        {
            switch (roleID)
            {
                case 0: return PreferredTops;
                case 1: return PreferredJgs;
                case 2: return PreferredMids;
                case 3: return PreferredADCs;
                case 4: return PreferredSups;
                case 5: return PreferredGeneral;
                default: return new int[5];
            }
        }

        private static string[] GetRunesByRoleID(int roleID)
        {
            switch (roleID)
            {
                case 0: return RunesTop;
                case 1: return RunesJg;
                case 2: return RunesMid;
                case 3: return RunesADC;
                case 4: return RunesSup;
                case 5: return RunesGeneral;
                default: return new string[5];
            }
        }

        private void OnGameFlowUpdate(object sender, LeagueEvent e)
        {
            string phase = e.Data.ToString();

            if (phase == "ReadyCheck" && autoAcceptQueue)
            {
                API.client.MakeApiRequest(HttpMethod.Post, "/lol-matchmaking/v1/ready-check/decline");
            }
            // returned to lobby for whatever reason
            else if (phase == "Lobby")
            {
                UnavailableChampsID.Clear();
                HandledActionIDs.Clear();
            }
        }

        private void OnChampSelectSessionUpdate(object sender, LeagueEvent e)
        {
            // exit if no actions (usually when leaving champ select)
            // or auto champ select not enabled
            if (!e.Data["actions"].HasValues || !autoChampSelect) return;

            WriteSafe("formdebug", e.Data.ToString());

            JToken bans = e.Data["bans"];
            var ourBans = bans["myTeamBans"];
            var theirBans = bans["theirTeamBans"];
            int localCellId = Convert.ToInt32(e.Data["localPlayerCellId"]);
            int roleID = 0;

            // empty bans array is "[]". we only want to parse bans if there are any
            if (ourBans.ToString().Length > 2) ParseBans(ourBans);
            if (theirBans.ToString().Length > 2) ParseBans(theirBans);

            JToken actionsTop = e.Data["actions"];
            JToken myTeamTop = e.Data["myTeam"];

            for (int i = 0; i < myTeamTop.Count(); i++)
            {
                var curPlayer = myTeamTop.ElementAt(i);
                int curCellId = Convert.ToInt32(curPlayer["cellId"]);

                if (localCellId == curCellId)
                {
                    string role = curPlayer["assignedPosition"].ToString().ToLower();
                    switch (role)
                    {
                        case "top": // top
                            roleID = 0;
                            break;
                        case "jungle": // jg
                            roleID = 1;
                            break;
                        case "middle": // mid
                            roleID = 2;
                            break;
                        case "bottom": // adc
                            roleID = 3;
                            break;
                        case "utility": // support
                            roleID = 4;
                            break;
                        default:
                            roleID = 5;
                            break;
                    }
                }
            }

            // iterate through each action (we have to do this becaused 1 action
            // is generated for each player at the same time meaning the last action
            // is only your action if you are last pick)
            for (int i = actionsTop.Count() - 1; i >= 0; i--)
            {
                var curAction = actionsTop.ElementAt(i).First;

                int curCellId = Convert.ToInt32(curAction["actorCellId"]);
                int actionId = Convert.ToInt32(curAction["id"]);
                bool myTurn = Convert.ToBoolean(curAction["isInProgress"]);
                string type = curAction["type"].ToString();

                // current cell belongs to us and hasn't 
                if (curCellId == localCellId && myTurn && !HandledActionIDs.Contains(curCellId))
                {
                    HandledActionIDs.Add(curCellId);

                    if (type == "pick")
                    {
                        int[] currentPrefs = GetChampionPrefsByRoleID(roleID);
                        string[] currentRunes = GetRunesByRoleID(roleID);

                        // automatically pick based on preferred list
                        for (int j = 0; j < PrefPoolSize; j++)
                        {
                            if (currentPrefs[j] != -1 && !UnavailableChampsID.Contains(currentPrefs[j]))
                            {
                                // lock in based on order of prefernce
                                string str = "{\"actorCellId\": " + curCellId + ", \"championId\":" + currentPrefs[j] + ", \"completed\": true, \"id\": " + actionId + ", \"type\": \"string\"}";
                                API.client.MakeApiRequest(HttpMethod.Patch, "/lol-champ-select/v1/session/actions/" + actionId, str);

                                // select runes
                                RuneManager runeManager = new RuneManager(API.client);
                                var allPages = runeManager.GetRunePages();
                                RunePage autoPage = null;

                                // find page named "auto"
                                foreach (RunePage p in allPages)
                                {
                                    if (p.Name == "auto")
                                    {
                                        autoPage = p;
                                        break;
                                    }
                                }
                                if (autoPage != null) // if page named "auto" is found
                                {
                                    // delete it
                                    API.client.MakeApiRequest(HttpMethod.Delete, "/lol-perks/v1/pages/" + autoPage.Id);
                                }

                                // convert runes string to int array
                                // right side is runes, left side is tree ids (-)
                                string primaryIdstr = currentRunes[j].Split('-')[0].Split(',')[0];
                                int primaryId = Convert.ToInt32(primaryIdstr);
                                string secondaryIdstr = currentRunes[j].Split('-')[0].Split(',')[1];
                                int secondaryId = Convert.ToInt32(secondaryIdstr);

                                string[] splitRunes = currentRunes[j].Split('-')[1].Split(',');
                                int[] selectedRunesArr = new int[9];
                                for (int k = 0; k < splitRunes.Length; k++)
                                {
                                    selectedRunesArr[k] = Convert.ToInt32(splitRunes[k]);
                                }

                                RunePage newPage = new RunePage()
                                {
                                    IsActive = true,
                                    IsCurrentPage = true,
                                    Name = "auto",
                                    PrimaryTreeId = primaryId,
                                    SelectedRunes = selectedRunesArr,
                                    SecondaryTreeId = secondaryId,
                                };

                                // create new rune page with runes chosen in form
                                API.client.MakeApiRequest(HttpMethod.Post, "/lol-perks/v1/pages/", newPage);

                                break;
                            }
                        }
                    }
                    else if (type == "ban")
                    {
                        // automatically ban based on preferred list
                        for (int j = 0; j < PreferredBans.Length; j++)
                        {
                            if (PreferredBans[j] != -1 && !UnavailableChampsID.Contains(PreferredBans[j]))
                            {
                                // lock in based on order of prefernce
                                string str = "{\"actorCellId\": " + curCellId + ", \"championId\":" + PreferredBans[j] + ", \"completed\": true, \"id\": " + actionId + ", \"type\": \"string\"}";
                                API.client.MakeApiRequest(HttpMethod.Patch, "/lol-champ-select/v1/session/actions/" + actionId, str);
                                break;
                            } //if
                        } //for
                    } // else if
                } // if
            } // for
        } //OnChampSelectSessionUpdate

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

        public static void ChampionSelected()
        {
            ChampForm.Close();

            if (selectingBan)
            {
                // update settings for this session
                PreferredBans[SelectingSlot] = ChampForm.selectedId;

                // update label
                string label = "Ban" + SelectingSlot;
                Label lb = form.Controls.Find(label, false).FirstOrDefault() as Label;
                lb.Text = Champion.IDtoName(ChampForm.selectedId);

                UpdateBanSettings();
            }
            else
            {
                // update settings for this session
                int[] currentPrefs = GetChampionPrefsByRoleID(CurrentRole);
                currentPrefs[SelectingSlot] = ChampForm.selectedId;

                // update label
                string label = "ChampPref" + SelectingSlot;
                Label lb = form.Controls.Find(label, false).FirstOrDefault() as Label;
                lb.Text = Champion.IDtoName(ChampForm.selectedId);

                UpdatePickSettings();
            }
            // save settings
            Properties.Settings.Default.Save();
        }

        public static void RunesSelected()
        {
            RuneForm.Close();

            string[] curRunes = GetRunesByRoleID(CurrentRole);
            curRunes[SelectingSlot] = RuneForm.selectedRunes;

            UpdateRuneSettings();
            Properties.Settings.Default.Save();
        }

        private static void UpdateBanSettings()
        {
            switch (SelectingSlot)
            {
                case 0:
                    Properties.Settings.Default.ban0 = ChampForm.selectedId;
                    break;
                case 1:
                    Properties.Settings.Default.ban1 = ChampForm.selectedId;
                    break;
                case 2:
                    Properties.Settings.Default.ban2 = ChampForm.selectedId;
                    break;
            }
        }

        private static void UpdatePickSettings()
        {
            if (CurrentRole == 0)
            {
                switch (SelectingSlot)
                {
                    case 0:
                        Properties.Settings.Default.top0 = ChampForm.selectedId;
                        break;
                    case 1:
                        Properties.Settings.Default.top1 = ChampForm.selectedId;
                        break;
                    case 2:
                        Properties.Settings.Default.top2 = ChampForm.selectedId;
                        break;
                    case 3:
                        Properties.Settings.Default.top3 = ChampForm.selectedId;
                        break;
                    case 4:
                        Properties.Settings.Default.top4 = ChampForm.selectedId;
                        break;
                }
            }
            else if (CurrentRole == 1)
            {

                switch (SelectingSlot)
                {
                    case 0:
                        Properties.Settings.Default.jg0 = ChampForm.selectedId;
                        break;
                    case 1:
                        Properties.Settings.Default.jg1 = ChampForm.selectedId;
                        break;
                    case 2:
                        Properties.Settings.Default.jg2 = ChampForm.selectedId;
                        break;
                    case 3:
                        Properties.Settings.Default.jg3 = ChampForm.selectedId;
                        break;
                    case 4:
                        Properties.Settings.Default.jg4 = ChampForm.selectedId;
                        break;
                }
            }
            else if (CurrentRole == 2)
            {

                switch (SelectingSlot)
                {
                    case 0:
                        Properties.Settings.Default.mid0 = ChampForm.selectedId;
                        break;
                    case 1:
                        Properties.Settings.Default.mid1 = ChampForm.selectedId;
                        break;
                    case 2:
                        Properties.Settings.Default.mid2 = ChampForm.selectedId;
                        break;
                    case 3:
                        Properties.Settings.Default.mid3 = ChampForm.selectedId;
                        break;
                    case 4:
                        Properties.Settings.Default.mid4 = ChampForm.selectedId;
                        break;
                }
            }
            else if (CurrentRole == 3)
            {

                switch (SelectingSlot)
                {
                    case 0:
                        Properties.Settings.Default.adc0 = ChampForm.selectedId;
                        break;
                    case 1:
                        Properties.Settings.Default.adc1 = ChampForm.selectedId;
                        break;
                    case 2:
                        Properties.Settings.Default.adc2 = ChampForm.selectedId;
                        break;
                    case 3:
                        Properties.Settings.Default.adc3 = ChampForm.selectedId;
                        break;
                    case 4:
                        Properties.Settings.Default.adc4 = ChampForm.selectedId;
                        break;
                }
            }
            else if (CurrentRole == 4)
            {

                switch (SelectingSlot)
                {
                    case 0:
                        Properties.Settings.Default.sup0 = ChampForm.selectedId;
                        break;
                    case 1:
                        Properties.Settings.Default.sup1 = ChampForm.selectedId;
                        break;
                    case 2:
                        Properties.Settings.Default.sup2 = ChampForm.selectedId;
                        break;
                    case 3:
                        Properties.Settings.Default.sup3 = ChampForm.selectedId;
                        break;
                    case 4:
                        Properties.Settings.Default.sup4 = ChampForm.selectedId;
                        break;
                }
            }
            else if (CurrentRole == 5)
            {

                switch (SelectingSlot)
                {
                    case 0:
                        Properties.Settings.Default.general0 = ChampForm.selectedId;
                        break;
                    case 1:
                        Properties.Settings.Default.general1 = ChampForm.selectedId;
                        break;
                    case 2:
                        Properties.Settings.Default.general2 = ChampForm.selectedId;
                        break;
                    case 3:
                        Properties.Settings.Default.general3 = ChampForm.selectedId;
                        break;
                    case 4:
                        Properties.Settings.Default.general4 = ChampForm.selectedId;
                        break;
                }
            }

        }

        private static void UpdateRuneSettings()
        {
            if (CurrentRole == 0)
            {
                switch (SelectingSlot)
                {
                    case 0:
                        Properties.Settings.Default.topRunes0 = RuneForm.selectedRunes;
                        break;
                    case 1:
                        Properties.Settings.Default.topRunes1 = RuneForm.selectedRunes;
                        break;
                    case 2:
                        Properties.Settings.Default.topRunes2 = RuneForm.selectedRunes;
                        break;
                    case 3:
                        Properties.Settings.Default.topRunes3 = RuneForm.selectedRunes;
                        break;
                    case 4:
                        Properties.Settings.Default.topRunes4 = RuneForm.selectedRunes;
                        break;
                }
            }
            else if (CurrentRole == 1)
            {

                switch (SelectingSlot)
                {
                    case 0:
                        Properties.Settings.Default.jgRunes0 = RuneForm.selectedRunes;
                        break;
                    case 1:
                        Properties.Settings.Default.jgRunes1 = RuneForm.selectedRunes;
                        break;
                    case 2:
                        Properties.Settings.Default.jgRunes2 = RuneForm.selectedRunes;
                        break;
                    case 3:
                        Properties.Settings.Default.jgRunes3 = RuneForm.selectedRunes;
                        break;
                    case 4:
                        Properties.Settings.Default.jgRunes4 = RuneForm.selectedRunes;
                        break;
                }
            }
            else if (CurrentRole == 2)
            {

                switch (SelectingSlot)
                {
                    case 0:
                        Properties.Settings.Default.midRunes0 = RuneForm.selectedRunes;
                        break;
                    case 1:
                        Properties.Settings.Default.midRunes1 = RuneForm.selectedRunes;
                        break;
                    case 2:
                        Properties.Settings.Default.midRunes2 = RuneForm.selectedRunes;
                        break;
                    case 3:
                        Properties.Settings.Default.midRunes3 = RuneForm.selectedRunes;
                        break;
                    case 4:
                        Properties.Settings.Default.midRunes4 = RuneForm.selectedRunes;
                        break;
                }
            }
            else if (CurrentRole == 3)
            {

                switch (SelectingSlot)
                {
                    case 0:
                        Properties.Settings.Default.adcRunes0 = RuneForm.selectedRunes;
                        break;
                    case 1:
                        Properties.Settings.Default.adcRunes1 = RuneForm.selectedRunes;
                        break;
                    case 2:
                        Properties.Settings.Default.adcRunes2 = RuneForm.selectedRunes;
                        break;
                    case 3:
                        Properties.Settings.Default.adcRunes3 = RuneForm.selectedRunes;
                        break;
                    case 4:
                        Properties.Settings.Default.adcRunes4 = RuneForm.selectedRunes;
                        break;
                }
            }
            else if (CurrentRole == 4)
            {

                switch (SelectingSlot)
                {
                    case 0:
                        Properties.Settings.Default.supRunes0 = RuneForm.selectedRunes;
                        break;
                    case 1:
                        Properties.Settings.Default.supRunes1 = RuneForm.selectedRunes;
                        break;
                    case 2:
                        Properties.Settings.Default.supRunes2 = RuneForm.selectedRunes;
                        break;
                    case 3:
                        Properties.Settings.Default.supRunes3 = RuneForm.selectedRunes;
                        break;
                    case 4:
                        Properties.Settings.Default.supRunes4 = RuneForm.selectedRunes;
                        break;
                }
            }
            else if (CurrentRole == 5)
            {

                switch (SelectingSlot)
                {
                    case 0:
                        Properties.Settings.Default.generalRunes0 = RuneForm.selectedRunes;
                        break;
                    case 1:
                        Properties.Settings.Default.generalRunes1 = RuneForm.selectedRunes;
                        break;
                    case 2:
                        Properties.Settings.Default.generalRunes2 = RuneForm.selectedRunes;
                        break;
                    case 3:
                        Properties.Settings.Default.generalRunes3 = RuneForm.selectedRunes;
                        break;
                    case 4:
                        Properties.Settings.Default.generalRunes4 = RuneForm.selectedRunes;
                        break;
                }
            }
        }

        private void autoAccept_CheckedChanged(object sender, EventArgs e)
        {
            autoAcceptQueue = autoAccept.Checked;
        }

        private void autoChampSel_CheckedChanged(object sender, EventArgs e)
        {
            autoChampSelect = autoChampSel.Checked;
        }

        private void ChampPref0_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 0;
            selectingBan = false;
            ChampForm.Show();
        }

        private void ChampPref1_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 1;
            selectingBan = false;
            ChampForm.Show();
        }

        private void ChampPref2_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 2;
            selectingBan = false;
            ChampForm.Show();
        }

        private void ChampPref3_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 3;
            selectingBan = false;
            ChampForm.Show();
        }

        private void ChampPref4_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 4;
            selectingBan = false;
            ChampForm.Show();
        }

        private void Ban0_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 0;
            selectingBan = true;
            ChampForm.Show();
        }

        private void Ban1_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 1;
            selectingBan = true;
            ChampForm.Show();
        }
        private void Ban2_DoubleClick(object sender, EventArgs e)
        {
            SelectingSlot = 2;
            selectingBan = true;
            ChampForm.Show();
        }

        private void Runes0_Click(object sender, EventArgs e)
        {
            SelectingSlot = 0;
            SetRuneFormRunes();
            RuneForm.Show();
        }

        private void Runes1_Click(object sender, EventArgs e)
        {
            SelectingSlot = 1;
            SetRuneFormRunes();
            RuneForm.Show();
        }

        private void Runes2_Click(object sender, EventArgs e)
        {
            SelectingSlot = 2;
            SetRuneFormRunes();
            RuneForm.Show();
        }

        private void Runes3_Click(object sender, EventArgs e)
        {
            SelectingSlot = 3;
            SetRuneFormRunes();
            RuneForm.Show();
        }

        private void Runes4_Click(object sender, EventArgs e)
        {
            SelectingSlot = 4;
            SetRuneFormRunes();
            RuneForm.Show();
        }

        private void SetRuneFormRunes()
        {
            switch (CurrentRole)
            {
                case 0:
                    RuneForm.selectedRunes = RunesTop[SelectingSlot];
                    break;
                case 1:
                    RuneForm.selectedRunes = RunesJg[SelectingSlot];
                    break;
                case 2:
                    RuneForm.selectedRunes = RunesMid[SelectingSlot];
                    break;
                case 3:
                    RuneForm.selectedRunes = RunesADC[SelectingSlot];
                    break;
                case 4:
                    RuneForm.selectedRunes = RunesSup[SelectingSlot];
                    break;
                case 5:
                    RuneForm.selectedRunes = RunesGeneral[SelectingSlot];
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentRole = comboBox1.SelectedIndex;
            UpdateChampionLabels();
        }
    }

    static class API
    {
        public static ILeagueClient client;
    }

}

