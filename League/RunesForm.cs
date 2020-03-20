using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace League
{
    public partial class RunesForm : Form
    {

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

            List<RuneTree> runeTrees = new List<RuneTree>();

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

            // TODO: everything

        }
    }
}

/*
 * Keystones and minor runes radio buttons should all
 * be generated on runtime to futureproof
 */

