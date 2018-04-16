using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DigitalThespian_CNIT_155_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////
        //Variable Declaration
        ////////////////////////////////////////////
        private int arraysize;
        private string[] mTitle = new string[50];//parallel arrays (Rubric Requirement)
        private string[] mGenre = new string[50];
        private string[] mRating = new string[50];
        string[] mFilter = new string[50];//these exist for the "saving filtered list" part
        int mFilterIndex = 0;             //
        private int mIndex = 0;
        private int ctr;

        ////////////////////////////////////////////
        //Helper Methods    (Rubric Requirement)
        ////////////////////////////////////////////

        private bool validate()
        {
            if (txtTitle.Text == "" || txtGenre.Text == "" || txtRating.Text == "")
            {
                displaymessage("Please enter all movie data.", "Incomplete entry");
                txtTitle.Focus();
                return false;
            }

            return true;
        }

        private void displaymessage(string message, string header)
        {
            MessageBox.Show(message, header, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void arraysort() //sorts list by whichever radio button is selected 
        {
            string[] sortingarray = new string[50]; //this exists to allow me to have one loop regardless 
            string chk;                             //of which array is the "key", so to speak
            int ctr;
            int sort;
            if (radTitle.Checked == true)//sort parameters based on radio buttons
            {
                Array.Copy(mTitle, sortingarray, 50); //this copies the "key" array over to the var sortingarray
            }
            else if (radGenre.Checked == true)
            {
                Array.Copy(mGenre, sortingarray, 50);
            }
            else
            {
                Array.Copy(mRating, sortingarray, 50);
            }

            for (sort = 0; sort < mIndex - 1; sort++)
            {
                for (ctr = 0; ctr < mIndex - 1; ctr++)
                {
                    chk = sortingarray[ctr];
                    if (chk.CompareTo(sortingarray[ctr + 1]) > 0)//this is literally just the bubble sort code
                    {                                            //from lab, but changing all 4 arrays at once
                        string temp = mTitle[ctr + 1];
                        chk = mTitle[ctr];
                        mTitle[ctr + 1] = chk;
                        mTitle[ctr] = temp;

                        temp = mGenre[ctr + 1];
                        chk = mGenre[ctr];
                        mGenre[ctr + 1] = chk;
                        mGenre[ctr] = temp;

                        temp = mRating[ctr + 1];
                        chk = mRating[ctr];
                        mRating[ctr + 1] = chk;
                        mRating[ctr] = temp;

                        temp = sortingarray[ctr + 1];
                        chk = sortingarray[ctr];
                        sortingarray[ctr + 1] = chk;
                        sortingarray[ctr] = temp;
                    }
                }
            }
        }

        ////////////////////////////////////////////
        //Main Body
        ////////////////////////////////////////////

        private void Form1_Load(object sender, EventArgs e)//checks if a filtered list has been made, loads it if yes
        {
            if (File.Exists("MoviesFiltered.txt") == true)
            {
                StreamReader SR = new StreamReader("MoviesFiltered.txt");
                arraysize = File.ReadLines("MoviesFiltered.txt").Count();
                if (arraysize > 50)
                {
                    displaymessage("The file is too large.", "Error");//this is in here because i was worried about using  
                    return;                                           //a list, and you HAVE to give arrays a length, so i said 50
                }

                string[] storage; //dummy array to hold values before splitting them into their proper arrays

                string line = SR.ReadLine();
                
                while (line != null)//SR loop
                {
                    storage = line.Split('\t');
                    mTitle[mIndex] = storage[0];
                    mGenre[mIndex] = storage[1];
                    mRating[mIndex] = storage[2];
                    line = SR.ReadLine();
                    mIndex++;
                }

                for (ctr = 0; ctr < mIndex; ctr++) //prints to the listbox
                {
                    lstOutput.Items.Add(mTitle[ctr].PadRight(20) + mGenre[ctr].PadRight(20) + mRating[ctr]);
                }
            }
            else if (File.Exists("Movies.txt") == true)//same deal, but if there isnt a filtered list
            {
                StreamReader SR = new StreamReader("Movies.txt");
                arraysize = File.ReadLines("Movies.txt").Count();
                if (arraysize > 50)
                {
                    displaymessage("The file is too large.", "Error");
                    return;
                }

                string[] storage; //dummy array to hold values before splitting them into their proper arrays

                string line = SR.ReadLine();

                while (line != null)
                {
                    storage = line.Split('\t');
                    mTitle[mIndex] = storage[0];
                    mGenre[mIndex] = storage[1];
                    mRating[mIndex] = storage[2];
                    line = SR.ReadLine();
                    mIndex++;
                }

                for (ctr = 0; ctr < mIndex; ctr++) //prints to the listbox
                {
                    lstOutput.Items.Add(mTitle[ctr].PadRight(20) + mGenre[ctr].PadRight(20) + mRating[ctr]);
                }
            }
            else
            {
                displaymessage("No existing file.", "Notification");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //i dont need this but i'm kind of afraid to try and delete it
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (txtGenreFilter.Text == "" && txtRatingFilter.Text == "")//if there is nothing, it clears back to 
            {                                                           //the original list and clears mFilter
                lstOutput.Items.Clear();
                Array.Clear(mFilter, 0, 50);
                for (ctr = 0; ctr < mIndex; ctr++)
                {
                    lstOutput.Items.Add(mTitle[ctr].PadRight(20) + mGenre[ctr].PadRight(20) + mRating[ctr]);
                }
                return;
            }


            lstOutput.Items.Clear();
            for (ctr = 0; ctr < mIndex; ctr++)//this prints all movies with the entered genre/rating,
            {                                 //and adds them to mFilter to be saved later
                if (txtGenreFilter.Text.ToUpper() == mGenre[ctr].ToUpper() || txtRatingFilter.Text.ToUpper() == mRating[ctr].ToUpper())
                {
                    lstOutput.Items.Add(mTitle[ctr].PadRight(20) + mGenre[ctr].PadRight(20) + mRating[ctr]);
                    mFilter[mFilterIndex] = mTitle[ctr] + '\t' + mGenre[ctr] + '\t' + mRating[ctr];//mFilter is in SW form already here
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (mIndex == 0)
            {
                displaymessage("No data entered.", "Error");
                txtTitle.Focus();
                return;
            }

            if (mFilter[0] != null)//if mFilter exists, it saves it to the filtered text file, otherwise
            {                      //it saves the movies list back over the old movies file (or makes one)
                StreamWriter SW = new StreamWriter("MoviesFiltered.txt");

                for (ctr = 0; ctr < mIndex; ctr++)
                {
                    SW.WriteLine(mFilter[ctr]);
                }

                SW.Close();
            }
            else
            {
                StreamWriter SW = new StreamWriter("Movies.txt");

                for (ctr = 0; ctr < mIndex; ctr++)
                {
                    SW.WriteLine(mTitle[ctr] + '\t' + mGenre[ctr] + '\t' + mRating[ctr]);
                }

                SW.Close();
            }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (validate() == false)
            {
                return;
            }

            mTitle[mIndex] = txtTitle.Text;//i tried doing this with combo boxes and it was NOT having it
            mGenre[mIndex] = txtGenre.Text;//so if you guys can figure that out be my guest
            mRating[mIndex] = txtRating.Text;
            mIndex++;

            arraysort(); //sorts new movie into list

            txtTitle.Clear(); txtGenre.Clear(); txtRating.Clear();
            lstOutput.Items.Clear();
            for (ctr = 0; ctr < mIndex; ctr++) //reprints the listbox with the new list
            {
                lstOutput.Items.Add(mTitle[ctr].PadRight(20) + mGenre[ctr].PadRight(20) + mRating[ctr]);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstOutput.SelectedIndex == -1)
            {
                displaymessage("You must select a movie to remove it.", "Invalid selection");
                return;
            }

            if (mIndex == 0)
            {
                displaymessage("No data entered.", "Error");
                txtTitle.Focus();
                return;
            }

            int index;
            index = lstOutput.SelectedIndex;//this is the removal loop i wrote in lab, i'm pretty sure almost
                                            //everyone did it this way
            for (ctr = index; ctr < mIndex - 1; ctr++)
            {
                mTitle[ctr] = mTitle[ctr + 1];
                mGenre[ctr] = mGenre[ctr + 1];
                mRating[ctr] = mRating[ctr + 1];
            }
            mIndex--;
            lstOutput.Items.Clear();
            for (ctr = 0; ctr < mIndex; ctr++)
            {
                lstOutput.Items.Add(mTitle[ctr].PadRight(20) + mGenre[ctr].PadRight(20) + mRating[ctr]);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();//please dont make me explain this
        }

        private void radTitle_CheckedChanged(object sender, EventArgs e)//these make it so when the user changes 
        {                                                               //sort criteria, it re-sorts immediately
            arraysort();
            lstOutput.Items.Clear();
            for (ctr = 0; ctr < mIndex; ctr++)
            {
                lstOutput.Items.Add(mTitle[ctr].PadRight(20) + mGenre[ctr].PadRight(20) + mRating[ctr]);
            }
        }

        private void radGenre_CheckedChanged(object sender, EventArgs e)
        {
            arraysort();
            lstOutput.Items.Clear();
            for (ctr = 0; ctr < mIndex; ctr++)
            {
                lstOutput.Items.Add(mTitle[ctr].PadRight(20) + mGenre[ctr].PadRight(20) + mRating[ctr]);
            }
        }

        private void radRating_CheckedChanged(object sender, EventArgs e)
        {
            arraysort();
            lstOutput.Items.Clear();
            for (ctr = 0; ctr < mIndex; ctr++)
            {
                lstOutput.Items.Add(mTitle[ctr].PadRight(20) + mGenre[ctr].PadRight(20) + mRating[ctr]);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)//search is based on which radio button is pressed
        {                                                       //yay multifunctionality
            if (radTitle.Checked == true)
            {
                for (ctr = 0; ctr < mIndex; ctr++)//this just checks the list against what is in txtTitle
                {                                 //and prints all of the matches (case-insensitive)
                    if (txtTitle.Text.ToUpper() == mTitle[ctr].ToUpper())
                    {
                        lstOutput.SelectedIndex = ctr;
                    }
                }
            }
            else if (radGenre.Checked == true)
            {
                for (ctr = 0; ctr < mIndex; ctr++)
                {
                    if (txtGenre.Text.ToUpper() == mGenre[ctr].ToUpper())
                    {
                        lstOutput.SelectedIndex = ctr;
                    }
                }
            }
            else
            {
                for (ctr = 0; ctr < mIndex; ctr++)
                {
                    if (txtRating.Text.ToUpper() == mRating[ctr].ToUpper())
                    {
                        lstOutput.SelectedIndex = ctr;
                    }
                }
            }
        }           //ending comments: I've been coding for like, three hours, so I haven't gone over this 
    }               //quite as thoroughly as i'd like, so it very well may have runtime errors
}
