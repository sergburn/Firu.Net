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
using System.IO.IsolatedStorage;
using FiruModel;
using System.IO;

namespace FiruPhoneImport
{
    public partial class MainPage : PhoneApplicationPage
    {
        private DslConverter mDslConverter;

        public MainPage()
        {
            InitializeComponent();
        }

        public void DslConverterProgressHandler(object sender, DslConverter.Info e)
        {
            laName.Text = e.Name + "\n" + e.SourceLanguage + " -> " + e.TargetLanguage;
            laWords.Text = e.WordCount.ToString();
            laTranslations.Text = e.TranslationCount.ToString();
            slProgress.Value = e.Progress * 100.0;
        }

        private void ImportDsl(FileStream fs)
        {
            if (mDslConverter == null)
            {
                mDslConverter = new DslConverter();
                mDslConverter.ProgressHandler += new EventHandler<DslConverter.Info>(DslConverterProgressHandler);
            }
            {
                using (Dictionary dict = Dictionary.Open(Dictionary.IsoConnectionString))
                {
                    mDslConverter.Import(fs, dict);
                }
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string[] files = iso.GetFileNames("*.dsl");
                foreach (string fname in files)
                {
                    laFileName.Text = fname;
                    string msg = String.Format("Do you want to import\n{0}?", fname);
                    if (MessageBoxResult.OK == MessageBox.Show(msg, "DSL file found", MessageBoxButton.OKCancel))
                    {
                        using (IsolatedStorageFileStream fs =
                            new IsolatedStorageFileStream(fname, FileMode.Open, iso))
                        {
                            ImportDsl(fs);
                        }
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (mDslConverter != null)
            {
                mDslConverter.Cancel();
            }
        }
    }
}