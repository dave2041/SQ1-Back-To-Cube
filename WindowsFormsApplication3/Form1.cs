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
            // The data file holds all of the states the cube can be in
            // Each state is numbered, has a distance to cube, which step is next, top and bottom pics
            string fileName = @"Data.txt";

            try
            {
                // Create a StreamReader  
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    string[] subs;
                    CubeState CS;
                    // Read the data in and convert each line to a cubeState object placing it into a list.
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

            // Grab all the image FileNames
            ImgNames = Directory.GetFiles(@".\Images\", "*.gif", SearchOption.AllDirectories).ToList();

            int imgCount = 0;

            foreach (string s in ImgNames)
            {
                // if the filename has an 'a' in it then we want to use it as the first option of the solve
                // The letters denote a twist of the puzzle layer.
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
            // The user has selected a layer to solve. Search the cubeStates list for a mathcing pair 
            // and populate the second list with the matching layer options
            if (listView1.SelectedItems.Count > 0)
            {
                listView2.Clear();
                selectedTopImageName = listView1.SelectedItems[0].Name;

                // striping file name to just the numbers so orientation doesn't matter.
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

                // a lookup for a list of valid cubestates
                var validStates = CubeStates.Where(CubeState => CubeState.topImageName.Contains(selectedImageName));
                
                // searching the valid list of cubestates and populating the second list.
                foreach (CubeState cs in validStates)
                {
                    ListViewItem lvi = new ListViewItem();
                    int index = ImgNames.FindIndex(x => x.Contains(cs.bottomImageName));
                    lvi.ImageIndex = index;
                    lvi.Name = ImgNames[index];
                    listView2.Items.Add(lvi);
                }
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If we've selected a top and a bottom later to be solved then step through the list of states 
            // to provide the solution to the puzzle
            if (listView1.SelectedItems.Count > 0 && listView2.SelectedItems.Count > 0)
            {
                listView3.Clear();
                string selectedBottomImageNameShort = listView2.SelectedItems[0].Name;
                string selectedTopImageNameShort = "";
                try
                {
                    Regex regexObj = new Regex(@"[^\d]");
                    selectedBottomImageNameShort = regexObj.Replace(selectedBottomImageNameShort, "");
                    selectedTopImageNameShort = regexObj.Replace(selectedTopImageName, "");
                }
                catch (ArgumentException ex)
                {
                    // Syntax error in the regular expression
                }

                // Find the current state using the selected top and bottom images
                var validStates = CubeStates.Where(CubeState => CubeState.topImageName.Contains(selectedTopImageNameShort)
                && CubeState.bottomImageName.Contains(selectedBottomImageNameShort)).ToArray();
                bool firstTime = true;

                if (validStates.Count() > 0)
                {
                    do
                    {
                        // ensure we populate with our first state
                        if (!firstTime)
                        {
                            // Grab the next state of the solve using the "next state" of the current state.
                            validStates = CubeStates.Where(CubeState => CubeState.PairNo == validStates[0].nextState).ToArray();
                        }

                        firstTime = false;
                        // populate the answers list with the state to be solved.
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
                    // the last states "next state" is 999 to indicate we've reached a square. Stop when we get there.
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
