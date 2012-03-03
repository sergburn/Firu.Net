using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Text;

namespace FiruModel
{
    enum TestResult
    {
        Incomplete,
        Passed,             // adds 1 to current rate
        PassedWithHints,    // doesn't change rate if 1 or 2, demotes rate 3 to 2
        Failed              // sets current rate 1.
    }

    enum AnswerValue
    {
        Incorrect,
        PartiallyCorrect,
        Correct
    };

    public class ReverseTest
    {
        uint mMaxLives = 0;
        uint mLivesLeft = 0;
        Vocabulary.Translation mChallenge = null;
        TestResult mResult = TestResult.Incomplete;
        Random mRand = new Random();

        public ReverseTest(Vocabulary.Translation challenge, uint maxLives = 3)
        {
            mChallenge = challenge;
            mMaxLives = maxLives;
            mLivesLeft = maxLives;
        }

        public string Question
        {
            get { return mChallenge.Text; }
        }

        public string Answer
        {
            get
            {
                if (mResult > TestResult.Incomplete)
                {
                    return mChallenge.Word.Text;
                }
                return "";
            }
        }

        public string QuestionLang
        {
            get { return mChallenge.TargetLang; }
        }

        public string AnswerLang
        {
            get { return mChallenge.Word.SourceLang; }
        }

        public int AnswerLength
        {
            get
            {
                if (mChallenge.ReverseMark < MarkValue.AlmostLearned)
                {
                    return mChallenge.Text.Length;
                }
                else
                {
                    return -1;
                }
            }
        }

        public AnswerValue CheckAnswer(string answer)
        {
            if (mResult == TestResult.Incomplete)
            {
                if (answer == mChallenge.Word.Text)
                {
                    SetTestResult(true);
                    return AnswerValue.Correct;
                }
                else if (mChallenge.Word.Text.StartsWith(answer))
                {
                    return AnswerValue.PartiallyCorrect;
                }
                else
                {
                    HandleHintOrMistake();
                    return AnswerValue.Incorrect;
                }
            }
            else
            {
                // better raise exception
                return AnswerValue.Incorrect;
            }
        }

        public string GetNextLetterHint(string current, List<string> groups)
        {
            StringBuilder hints = new StringBuilder();
            string answer = mChallenge.Word.Text;

            if ( answer.Length > current.Length &&
                 answer.StartsWith( current ) )
            {
                string next = answer.Substring(current.Length,1);
                foreach( string grp in groups )
                {
                    if (grp.IndexOf(next, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        hints.Append(next);
                    }
                    else
                    {
                        hints.Append(grp.Substring(mRand.Next(grp.Length), 1);
                    }
                }
            }
            return hints.ToString();
        }

        public string GetHint(string current)
        {
            if (mResult != TestResult.Incomplete) return "";

            string answer = mChallenge.Word.Text;
            if (answer.Length > current.Length + 1) // never hint last letter
            {
                HandleHintOrMistake();
                if (answer.StartsWith(current))
                {
                    return Answer.Substring(current.Length + 1);
                }
            }
            return "";
        }

        public void HandleHintOrMistake()
        {
            if (mLivesLeft > 0)
            {
                mLivesLeft--;
                if (mChallenge.ReverseMark == MarkValue.AlmostLearned)
                {
                    mChallenge.UpdateReverseMark(TestResult.PassedWithHints);
                }
            }
            else
            {
                SetTestResult(false);
            }
        }

        void SetTestResult(bool passed)
        {
            if (mResult != TestResult.Incomplete) return; // better - assert or exception

            if (passed)
            {
                mResult = (mLivesLeft < mMaxLives) ?
                    TestResult.PassedWithHints :
                    TestResult.Passed;
            }
            else
            {
                mResult = TestResult.Failed;
            }

            mChallenge.UpdateReverseMark(mResult);
        }
    }
}
