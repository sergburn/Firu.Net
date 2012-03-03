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
using FiruModel;
using System.Collections.Generic;

namespace FiruPhone
{
    public class TrainerViewModel
    {
        Vocabulary mVocabulary = null;
        
        private int _currentTestIndex;
        private List<ReverseTest> _tests;

        public ReverseTest CurrentTest
        {
            get
            {
                if (_currentTestIndex >= 0)
                {
                    return _tests[_currentTestIndex];
                }
                return null;
            }
        }

        public TrainerViewModel(Vocabulary voc)
        {
            mVocabulary = voc;
        }

        public void StartNewExercise()
        {
            _tests.AddRange
        }

        public bool StartNextTest()
        {
            return hasMoreTests;
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
    }
}
