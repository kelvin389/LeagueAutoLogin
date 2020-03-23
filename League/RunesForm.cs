using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace League
{
    public partial class RunesForm : Form
    {
        /*
         * 0 : domination
         * 1 : inspiration
         * 2 : precision
         * 3 : resolve
         * 4 : sorcery
         */
        private List<RuneTree> runeTrees = new List<RuneTree>();
        private List<GroupBox> primaryGroupBoxes = new List<GroupBox>();
        private List<GroupBox> secondaryGroupBoxes = new List<GroupBox>();
        private RuneTree primaryTree;
        private RuneTree secondaryTree;
        private const int GLOBAL_START_X = 10;
        private const int GLOBAL_START_Y = 25;
        private const int GLOBAL_X_OFFSET = 130;
        private const int PRIMARY_GROUPBOX_START_Y = 100;
        private const int PRIMARY_GROUPBOX_START_X = 50;
        private const int PRIMARY_GROUPBOX_OFFSET = 50;
        private const int SECONDARY_GROUPBOX_START_Y = 50;
        private const int SECONDARY_GROUPBOX_START_X = 600;
        private const int SECONDARY_GROUPBOX_OFFSET = 50;

        public string selectedRunes = "";

        public RunesForm()
        {
            InitializeComponent();
        }

        public void ParseRunesJson()
        {
            //string jsondir = Directory.GetCurrentDirectory() + "/data/champion.json";
            string jsondir = "C:/Users/kelvi/Documents/Visual Studio 2019/Projects/League/League/resources/runesReforged.json";
            string json = System.IO.File.ReadAllText(jsondir);

            var trees = JArray.Parse(json);

            var curTree = trees.First;

            // iterate through each rune tree (prec, dom, sorc, res, insp)
            for (int k = 0; k < trees.Count(); k++)
            {
                string treeName = curTree["name"].ToString();
                int treeId = Convert.ToInt32(curTree["id"].ToString());
                string treeIconPath = curTree["icon"].ToString();

                var slots = curTree["slots"];
                var curSlot = slots.First;

                List<RuneRow> rows = new List<RuneRow>();

                // iterate through each row of runes (keystones, secondaries 1, secondaries 2, secondaries 3)
                for (int j = 0; j < slots.Count(); j++)
                {
                    List<Rune> runes = new List<Rune>();
                    var curRune = curSlot["runes"].First;

                    // iterate through each rune in a row (pta, lt, fleet, conq)
                    for (int i = 0; i < curSlot["runes"].Count(); i++)
                    {
                        int runeId = Convert.ToInt32(curRune["id"]);
                        string runeIconPath = curRune["icon"].ToString();
                        string runeName = curRune["name"].ToString();
                        string runeKey = curRune["key"].ToString();

                        Rune newRune = new Rune(runeId, runeIconPath, runeName, runeKey);
                        runes.Add(newRune);
                        curRune = curRune.Next; // go to next rune in row
                    }
                    RuneRow row = new RuneRow(runes);
                    rows.Add(row);
                    curSlot = curSlot.Next; // go to next row
                }
                RuneTree set = new RuneTree(rows, treeId, treeName, treeIconPath);
                runeTrees.Add(set);
                curTree = curTree.Next; // go to next tree
            }
        }

        private void UpdatePrimary()
        {
            // clean up previous group boxes and radiobuttons
            for (int i = 0; i < primaryGroupBoxes.Count; i++)
            {
                Controls.Remove(primaryGroupBoxes[i]);
            }

            // iterate through each row of runes
            for (int i = 0; i < primaryTree.runeRows.Count; i++)
            {
                List<Rune> thisRowRunes = primaryTree.runeRows[i].runes;
                List<RadioButton> thisRowRadioButtons = new List<RadioButton>();
                int curX = GLOBAL_START_X;
                if (thisRowRunes.Count == 3) curX += (int)(GLOBAL_X_OFFSET / 2.0f);

                // iterate through each rune in this row
                for (int j = 0; j < thisRowRunes.Count; j++)
                {
                    // create new radio button for each rune
                    RadioButton button = new RadioButton
                    {
                        Location = new System.Drawing.Point(curX, GLOBAL_START_Y),
                        AutoSize = true,
                        Tag = thisRowRunes[j].id,
                        Text = thisRowRunes[j].name
                    };
                    thisRowRadioButtons.Add(button);
                    curX += GLOBAL_X_OFFSET;
                }

                System.Drawing.Rectangle rect = new System.Drawing.Rectangle()
                {
                    X = PRIMARY_GROUPBOX_START_X,
                    Y = PRIMARY_GROUPBOX_START_Y + (PRIMARY_GROUPBOX_OFFSET * i), // move down as number of groupboxes already created increases
                };
                GroupBox box = new GroupBox()
                {
                    Bounds = rect,
                    AutoSize = true,
                    MinimumSize = new System.Drawing.Size(520, 0) // force all sizes of boxes to be 500px wide
                };
                for (int k = 0; k < thisRowRadioButtons.Count; k++)
                {
                    //this.Controls.Add(thisRowRadioButtons[k]);
                    box.Controls.Add(thisRowRadioButtons[k]);
                }
                this.Controls.Add(box);
                primaryGroupBoxes.Add(box);
            }
        }

        private void UpdateSecondary()
        {
            // clean up previous group boxes and radiobuttons
            for (int i = 0; i < secondaryGroupBoxes.Count; i++)
            {
                Controls.Remove(secondaryGroupBoxes[i]);
            }

            // iterate through each row of runes
            // start at 1 to skip keystones
            for (int i = 1; i < secondaryTree.runeRows.Count; i++)
            {
                List<Rune> thisRowRunes = secondaryTree.runeRows[i].runes;
                List<CheckBox> thisRowCheckBoxes = new List<CheckBox>();
                int curX = GLOBAL_START_X;
                if (thisRowRunes.Count == 3) curX += (int)(GLOBAL_X_OFFSET / 2.0f);

                // iterate through each rune in this row
                for (int j = 0; j < thisRowRunes.Count; j++)
                {
                    // create new radio button for each rune
                    CheckBox checkbox = new CheckBox
                    {
                        Location = new System.Drawing.Point(curX, GLOBAL_START_Y),
                        AutoSize = true,
                        Tag = thisRowRunes[j].id,
                        Text = thisRowRunes[j].name
                    };
                    thisRowCheckBoxes.Add(checkbox);
                    curX += GLOBAL_X_OFFSET;
                }

                System.Drawing.Rectangle rect = new System.Drawing.Rectangle()
                {
                    X = SECONDARY_GROUPBOX_START_X,
                    Y = SECONDARY_GROUPBOX_START_Y + (SECONDARY_GROUPBOX_OFFSET * i), // move down as number of groupboxes already created increases
                };
                GroupBox box = new GroupBox()
                {
                    Bounds = rect,
                    AutoSize = true,
                    MinimumSize = new System.Drawing.Size(520, 0) // force all sizes of boxes to be 500px wide
                };
                for (int k = 0; k < thisRowCheckBoxes.Count; k++)
                {
                    //this.Controls.Add(thisRowRadioButtons[k]);
                    box.Controls.Add(thisRowCheckBoxes[k]);
                }
                this.Controls.Add(box);
                secondaryGroupBoxes.Add(box);
            }
        }

        private void RunesForm_Load(object sender, EventArgs e)
        {

        }

        private void primary0_CheckedChanged(object sender, EventArgs e)
        {
            if (primary0.Checked)
            {
                primaryTree = runeTrees[2]; // precision
                UpdatePrimary();
            }
        }

        private void primary1_CheckedChanged(object sender, EventArgs e)
        {
            if (primary1.Checked)
            {
                primaryTree = runeTrees[0]; //domination
                UpdatePrimary();
            }
        }

        private void primary2_CheckedChanged(object sender, EventArgs e)
        {
            if (primary2.Checked)
            {
                primaryTree = runeTrees[4]; // sorcery
                UpdatePrimary();
            }
        }

        private void primary3_CheckedChanged(object sender, EventArgs e)
        {
            if (primary3.Checked)
            {
                primaryTree = runeTrees[3]; // resolve
                UpdatePrimary();
            }
        }

        private void primary4_CheckedChanged(object sender, EventArgs e)
        {
            if (primary4.Checked)
            {
                primaryTree = runeTrees[1]; // inspiration
                UpdatePrimary();
            }
        }


        private void secondary0_CheckedChanged(object sender, EventArgs e)
        {
            if (secondary0.Checked)
            {
                secondaryTree = runeTrees[2]; // prec
                UpdateSecondary();
            }
        }

        private void secondary1_CheckedChanged(object sender, EventArgs e)
        {
            if (secondary1.Checked)
            {
                secondaryTree = runeTrees[0]; // dom
                UpdateSecondary();
            }
        }

        private void secondary2_CheckedChanged(object sender, EventArgs e)
        {
            if (secondary2.Checked)
            {
                secondaryTree = runeTrees[4]; // sorc
                UpdateSecondary();
            }
        }

        private void secondary3_CheckedChanged(object sender, EventArgs e)
        {
            if (secondary3.Checked)
            {
                secondaryTree = runeTrees[3]; // res
                UpdateSecondary();
            }
        }

        private void secondary4_CheckedChanged(object sender, EventArgs e)
        {
            if (secondary4.Checked)
            {
                secondaryTree = runeTrees[1]; // insp
                UpdateSecondary();
            }
        }
        private void RunesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            // TODO: reset all form fields for next time
            e.Cancel = true; // cancel close event
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // TODO: input checking (only 2 checkboxes selected, etc)

            // list containing both primary and secondary groupboxes
            List<GroupBox> allGroupBoxes = new List<GroupBox>();

            // list containing all checked radio buttons
            List<RadioButton> checkedRadios = new List<RadioButton>();
            // list containing all checked checkboxes
            List<CheckBox> checkedBoxes = new List<CheckBox>();

            // add primary groupboxes to allgroupboxes list
            for (int i = 0; i < primaryGroupBoxes.Count; i++)
            {
                allGroupBoxes.Add(primaryGroupBoxes[i]);
            }
            // add secondary groupboxes to allgroupboxes list
            for (int i = 0; i < secondaryGroupBoxes.Count; i++)
            {
                allGroupBoxes.Add(secondaryGroupBoxes[i]);
            }

            // iterate through each groupbox
            for (int i = 0; i < allGroupBoxes.Count; i++)
            {
                // all radio buttons inside of this groupbox
                List<RadioButton> thisRadios = allGroupBoxes[i].Controls.OfType<RadioButton>().ToList();
                // all checkboxes inside of this groupbox
                List<CheckBox> thisBoxes = allGroupBoxes[i].Controls.OfType<CheckBox>().ToList();

                // add all checked radio buttons to checkedRadios list
                for (int j = 0; j < thisRadios.Count; j++)
                {
                    if (thisRadios[j].Checked) checkedRadios.Add(thisRadios[j]);
                }
                // add all checked checkboxes buttons to checkedBoxes list
                for (int j = 0; j < thisBoxes.Count; j++)
                {
                    if (thisBoxes[j].Checked) checkedBoxes.Add(thisBoxes[j]);
                }
            }

            List<int> selectedRuneIds = new List<int>();
            for (int i = 0; i < checkedRadios.Count; i++)
            {
                int runeId = Convert.ToInt32(checkedRadios[i].Tag);
                selectedRuneIds.Add(runeId);
            }
            for (int i = 0; i < checkedBoxes.Count; i++)
            {
                int runeId = Convert.ToInt32(checkedBoxes[i].Tag);
                selectedRuneIds.Add(runeId);
            }

            string runeStr = "";
            for (int i = 0; i < selectedRuneIds.Count; i++)
            {
                runeStr += selectedRuneIds[i].ToString();
                if (i != selectedRuneIds.Count - 1) runeStr += ","; // if not at end, add comma to seperate runes
            }

            selectedRunes = runeStr;

            Form1.RunesSelected();
        }
    }

    
}

/*
 * Keystones and minor runes radio buttons should all
 * be generated on runtime to futureproof
 */

/* https://stackoverflow.com/questions/18547326/how-do-i-get-which-radio-button-is-checked-from-a-groupbox
 * find all checked buttons
 * 
 * var buttons = this.Controls.OfType<RadioButton>().FirstOrDefault(n => n.Checked);
 */
