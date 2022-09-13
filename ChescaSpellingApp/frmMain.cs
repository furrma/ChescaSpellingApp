using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Timers;

namespace ChescaSpellingApp
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Activated += AfterLoading;
        }

        private void AfterLoading(object sender, EventArgs e)
        {
            this.Activated -= AfterLoading;
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            synth.Speak("Time for spelling, Chesca. Click start when you're ready.");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            txtWord.Visible = true;
            lblWord.Visible = true;
            lblTitle.Visible = false;
            btnStart.Visible = false;
            loadTest();
        }

        private async void loadTest()
        {
            int score = 0;
            string[] words = new string[5];
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            for (int i = 0; i < words.Length; i++)
            {
                var newWord = GenerateWord();
                while (MatchedWord(words, newWord) == newWord)
                {
                    newWord = GenerateWord();
                }
                words[i] = newWord;

                synth.Speak("I will give you 20 seconds to spell the word..." + words[i] + ". Again, the word is " + words[i] + ". Go.");
                txtWord.Enabled = true;
                await Task.Delay(20000);
                ShowWord(words[i]);
                await Task.Delay(500);
                synth.Speak("Let's see if you got the spelling correct. The correct spelling for the word " + words[i] + " is " + string.Join(", ", words[i].ToCharArray()));
                lblCorrectWord.Hide();
                if (words[i].ToLower() == txtWord.Text.Trim().ToLower())
                    score++;
                txtWord.Text = "";
                await Task.Delay(500);
                if (i != 4)
                    synth.Speak("It's time for the next word.");
            }
            if (score > 2)
                synth.Speak("Good job today, Chesca. You got " + Convert.ToString(score) + " out of 5. See you again tomorrow.");
            else
                synth.Speak("Try harder next time, Chesca. You got " + Convert.ToString(score) + " out of 5. See you again tomorrow.");
            Application.Exit();
        }

        private string GenerateWord()
        {
            string text = System.IO.File.ReadAllText(@"D:\Projects\ChescaSpellingApp\words.txt");

            Random random = new Random();
            string[] split = text.Split(' ');
            string randomString = split[random.Next(0, split.Length)];
            return randomString;
        }

        private string MatchedWord(string[] arr, string word)
        {
            if (arr[0] == null)
                return String.Empty;
            return Array.Find(arr, element => element == word);
        }

        private void txtWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void ShowWord(string word)
        {
            lblCorrectWord.Show();
            lblCorrectWord.Text = word;
        }
    }
}
