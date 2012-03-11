using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace FiruPhone
{
    public partial class TrainerView : PhoneApplicationPage
    {
        string mAnswerText;
        List<string> mKeypadGroups = null;
        List<Button> mKeypad = null;

        public TrainerView()
        {
            InitializeComponent();
            DataContext = App.TrainerModel;
            List<string> mKeypadGroups = TrainerViewModel.GetKeypadGroups("fi");
            List<Button> mKeypad = new List<Button>();
            mKeypad.Add(btn0);
            mKeypad.Add(btn1);
            mKeypad.Add(btn2);
            mKeypad.Add(btn3);
            mKeypad.Add(btn4);
            mKeypad.Add(btn5);
            mKeypad.Add(btn6);
            mKeypad.Add(btn7);
            mKeypad.Add(btn8);
            mKeypad.Add(btn9);
        }

        private void MarkAsLearned_Click(object sender, EventArgs e)
        {

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.TrainerModel.Challenge.Length == 0)
            {
                App.TrainerModel.StartNextTest();
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                CheckNextLetter(btn.Content.ToString());
            }
        }

        void CheckNextLetter(string letter)
        {
            CheckAnswer(mAnswerText + letter);
        }

        void CheckAnswer(string answer)
        {
            FiruModel.AnswerValue av = App.TrainerModel.CurrentTest.CheckAnswer(answer);
            if (av == FiruModel.AnswerValue.PartiallyCorrect )
            {
                mAnswerText = answer;
                laWord.Text = mAnswerText;
                slLivesLeft.Value = App.TrainerModel.CurrentTest.LivesLeft;
                ShowNextLetters();
            }
            else if ( av == FiruModel.AnswerValue.Correct )
            {
                mAnswerText = answer;
                ShowResult();
            }
            slLivesLeft.Value = App.TrainerModel.CurrentTest.LivesLeft;
        }

        void ShowNextLetters()
        {
            string letters = App.TrainerModel.CurrentTest.GetNextLetterHint(mAnswerText, mKeypadGroups);
            for ( int i = 0; i < letters.Length; i++ )
            {
                mKeypad[i].Content = letters[i];
            }
            frmKeypad.Visibility = System.Windows.Visibility.Visible;
        }

        void ShowHints()
        {
            string hint = App.TrainerModel.CurrentTest.GetHint(mAnswerText);
            if (hint.Length > 0)
            {
                CheckAnswer(hint);
            }
        }

        void ShowResult()
        {
            laWord.Text = App.TrainerModel.CurrentTest.Answer;
            frmKeypad.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}