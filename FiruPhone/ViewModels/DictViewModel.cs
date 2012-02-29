using System;
using System.ComponentModel;
using FiruModel;
using System.Collections.Generic;

namespace FiruPhone
{
    public class DictViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private Dictionary mDictionary;
        private Vocabulary mVocabulary;

        private string mSearchText;
        public string SearchText
        {
            get { return mSearchText; }
            set
            {
                if (mSearchText != value)
                {
                    mSearchText = value;
                    Update();
                }
            }
        }

        public List<Dictionary.Word> Matches
        {
            get;
            private set;
        }

        private Dictionary.Word mSelection;
        public Dictionary.Word Selection
        {
            get
            {
                return mSelection;
            }
            set
            {
                if (mSelection != value)
                {
                    NotifyPropertyChanging("Selection");
                    mSelection = value;
                    NotifyPropertyChanged("Selection");
                }
            }
        }

        public void AddSelectionToLearning()
        {
            mVocabulary.AddWord(Selection, mDictionary);
        }

        public string Stat
        {
            get
            {
                if (Matches != null)
                    return String.Format("Found {0} matches out of {1} words.", 
                        Matches.Count, mDictionary.TotalWords);
                else
                    return String.Format("Total {0} words and {1} translations.",
                        mDictionary.TotalWords, mDictionary.TotalTranslations);
            }
        }

        public string Info
        {
            get
            {
                return mDictionary.Description.Name;
            }
        }

        public DictViewModel(Dictionary dict, Vocabulary voc)
        {
            mDictionary = dict;
            mVocabulary = voc;
        }

        void Update()
        {
            NotifyPropertyChanging("Matches");
            NotifyPropertyChanging("Stat");
            Matches = mDictionary.SearchWords(SearchText);
            NotifyPropertyChanged("Stat");
            NotifyPropertyChanged("Matches");
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
