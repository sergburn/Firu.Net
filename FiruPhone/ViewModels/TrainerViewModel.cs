using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using FiruModel;
using System.ComponentModel;

namespace FiruPhone
{
    public class TrainerViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        Vocabulary mVocabulary = null;
        Random mRand = new Random();

        ReverseTest mCurrentTest = null;

        public string Challenge
        {
            get
            {
                if (mCurrentTest != null)
                    return mCurrentTest.Challenge.Text;
                else
                    return "";
            }
        }

        public string CurrentMark
        {
            get
            {
                if (mCurrentTest != null)
                    return Mark.ToString(mCurrentTest.Challenge.ReverseMark);
                else
                    return "nothing to learn";
            }
        }

        public int LivesLeft
        {
            get
            {
                if (mCurrentTest != null)
                    return mCurrentTest.LivesLeft;
                else
                    return 0;
            }
        }

        public int LivesTotal
        {
            get
            {
                if (mCurrentTest != null)
                    return mCurrentTest.MaxLives;
                else
                    return 0;
            }
        }

        public TrainerViewModel(Vocabulary voc)
        {
            mVocabulary = voc;
        }

        public bool StartNextTest()
        {
            Dictionary<MarkValue, List<int>> allIds = new Dictionary<MarkValue, List<int>>();
            int numTriedMarks = 0;
            while (true)
            {
                int random = mRand.Next(100);
                MarkValue mark = MarkValue.Unknown;
                if (random < 15) // 15%
                {
                    mark = MarkValue.AlmostLearned;
                }
                else if (random < 50) // 35%
                {
                    mark = MarkValue.WithHints;
                }
                else // 50%
                {
                    mark = MarkValue.ToLearn;
                }

                if (!allIds.ContainsKey(mark))
                {
                    var idQuery = from t in mVocabulary.Translations
                                  where t.ReverseMark == mark
                                  select t.ID;
                    allIds[mark] = idQuery.ToList();
                    numTriedMarks++;
                }

                if (allIds[mark].Count > 0)
                {
                    random = mRand.Next(allIds[mark].Count);

                    var tranQuery = from t in mVocabulary.Translations
                                    where t.ID == random
                                    select t;

                    mCurrentTest = new ReverseTest(tranQuery.First());
                    return true;
                }
                else
                {
                    // try again unless there is nothing to learn
                    if (numTriedMarks == 3)
                    {
                        mCurrentTest = null;
                        return false;
                    }
                }
            }
        }

        public static List<string> GetKeypadGroups(string language)
        {
            List<string> groups = new List<string>();
            switch (language)
            {
                case "fi": // Finnish
                    groups.Add("\xE4\xF6");
                    groups.Add(",. '-!?");
                    groups.Add("abc");
                    groups.Add("def");
                    groups.Add("ghi");
                    groups.Add("jkl");
                    groups.Add("mno");
                    groups.Add("pqrs");
                    groups.Add("tuv");
                    groups.Add("wxyz");
                    break;

                case "ru": // Russian
                    break;
                default:
                    break;
            }
            return groups;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        protected void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
