using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        List<CubeState> CubeStates = new List<CubeState>();
        List<string> ImgNames = new List<string>();
        string selectedTopImageName;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // File name  
            string fileName = @"Data.txt";

            try
            {
                // Create a StreamReader  
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    string[] subs;
                    CubeState CS;
                    // Read line by line  
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        subs = line.Split(',');
                        int pair = int.Parse(subs[0]);
                        string top = subs[1];
                        string bottom = subs[2];
                        int next = int.Parse(subs[3]);
                        int distance = int.Parse(subs[5]);
                        CS = new CubeState(pair, top, bottom, next, distance);
                        CubeStates.Add(CS);
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

            //var Images= Directory.GetFiles(@".\Images\", "*.gif",
            //                       SearchOption.AllDirectories)
            //             .Select(Image.FromFile).ToList();

            ImgNames = Directory.GetFiles(@".\Images\", "*.gif", SearchOption.AllDirectories).ToList();
            ImgNames.Sort();

            int imgCount = 0;

            foreach (string s in ImgNames)
            {
                if (s.Substring(10).Contains('a'))
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.ImageIndex = imgCount;
                    lvi.Name = ImgNames[imgCount];
                    listView1.Items.Add(lvi);
                }
                imgCount++;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                listView2.Clear();
                selectedTopImageName = listView1.SelectedItems[0].Name;
                string selectedImageName = selectedTopImageName.Replace("[^-?0-9]+", " ");
                try
                {
                    Regex regexObj = new Regex(@"[^\d]");
                    selectedImageName = regexObj.Replace(selectedImageName, "");
                }
                catch (ArgumentException ex)
                {
                    // Syntax error in the regular expression
                }
                var validStates = CubeStates.Where(CubeState => CubeState.topImageName.Contains(selectedImageName));
                
                foreach (CubeState cs in validStates)
                {
                    ListViewItem lvi = new ListViewItem();
                    int index = ImgNames.FindIndex(x => x.Contains(cs.bottomImageName));
                    lvi.ImageIndex = index;
                    lvi.Name = ImgNames[index];
                    listView2.Items.Add(lvi);
                    //lvi = new ListViewItem();
                    //index = ImgNames.FindIndex(x => x == cs.bottomImageName);
                    //lvi.ImageIndex = index;
                    //listView3.Items.Add(lvi);
                }
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0 && listView2.SelectedItems.Count > 0)
            {
                listView3.Clear();
                string selectedBottomImageName = listView2.SelectedItems[0].Name;
                string selectedImageName = selectedBottomImageName.Replace("[^-?0-9]+", " ");
                string selectedTopImageNameShort = "";
                try
                {
                    Regex regexObj = new Regex(@"[^\d]");
                    selectedImageName = regexObj.Replace(selectedImageName, "");
                    selectedTopImageNameShort = regexObj.Replace(selectedTopImageName, "");
                }
                catch (ArgumentException ex)
                {
                    // Syntax error in the regular expression
                }

                var validStates = CubeStates.Where(CubeState => CubeState.topImageName.Contains(selectedTopImageNameShort)
                && CubeState.bottomImageName.Contains(selectedImageName)).ToArray();
                bool firstTime = true;

                if (validStates.Count() > 0)
                {
                    do
                    {
                        if (!firstTime)
                        {
                            validStates = CubeStates.Where(CubeState => CubeState.PairNo == validStates[0].nextState).ToArray();
                        }

                        firstTime = false;
                        listView3.Items.Add("Pair #" + validStates[0].PairNo);
                        ListViewItem lvi = new ListViewItem();
                        int index = ImgNames.FindIndex(x => x.Equals(".\\Images\\" + validStates[0].topImageName));
                        lvi.ImageIndex = index;
                        listView3.Items.Add(lvi);
                        lvi = new ListViewItem();
                        index = ImgNames.FindIndex(x => x.Contains(validStates[0].bottomImageName));
                        lvi.ImageIndex = index;
                        listView3.Items.Add(lvi);
                        listView3.Items.Add(validStates[0].nextState + " Next. Distance: " + validStates[0].distance);
                        listView3.Items.Add("");
                    }
                    while (validStates[0].nextState != 999);
                }

            }
        }
    }

    class CubeState
    {
        public int PairNo;
        public string topImageName;
        public string bottomImageName;
        public int nextState;
        public int distance;

        public CubeState()
        {
        }

        public CubeState(int pair, string top, string bottom, int next, int dist)
        {
            nextState = next;
            distance = dist;
            PairNo = pair;
            topImageName = top;
            bottomImageName = bottom;
        }
    }
}
