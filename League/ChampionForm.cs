using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace League
{
    public partial class ChampionForm : Form
    {
        private static Dictionary<string, int> Champions = new Dictionary<string, int>();
        private static Dictionary<string, int>.KeyCollection ChampNames = Champions.Keys;

        public int selectedId = 0;
        public string selectedName = "";

        public ChampionForm()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            //string jsondir = Directory.GetCurrentDirectory() + "/data/champion.json";
            string jsondir = "C:/Users/kelvi/Documents/Visual Studio 2019/Projects/League/League/data/champion.json";
            string json = System.IO.File.ReadAllText(jsondir);

            // very top of json file
            var top = JObject.Parse(json);
            // inside of "data"
            var data = top["data"];

            // JProperty layer of the "data"
            // https://stackoverflow.com/questions/19200353/why-do-i-need-to-call-first-in-a-foreach-select-in-json-net
            var JpropLevel = data.Children().First();

            for (int i = 0; i < data.Count(); i++)
            {
                // current champion json data
                var curChamp = JpropLevel.First(); // call first again to step into the JToken layer

                string name = curChamp["id"].ToString();
                string id = curChamp["key"].ToString();

                Champions.Add(name, Convert.ToInt32(id));

                // next champion (next on the JProperty layer)
                JpropLevel = JpropLevel.Next;
                
            }

            foreach (string name in ChampNames)
            {
                listBox1.Items.Add(name);
            }

            // default to value 1 so we can never return something empty on close
            listBox1.SelectedIndex = 0;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            foreach (string name in ChampNames)
            {
                if (name.StartsWith(textBox1.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    listBox1.Items.Add(name);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectChamp();
        }
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            SelectChamp();
        }

        private void SelectChamp()
        {
            string champ = (string)listBox1.SelectedItem;
            if (Champions.TryGetValue(champ, out selectedId))
            {
                selectedName = champ;
                Form1.ChampionSelected();
            }
            else
            {
                MessageBox.Show("error selecting champ");
            }
        }
        private void ChampionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true; // cancel close event
            textBox1.Text = "";
        }

        
    }
}
