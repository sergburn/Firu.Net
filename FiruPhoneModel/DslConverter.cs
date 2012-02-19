using System;
using System.Net;
using System.Windows;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace FiruModel
{
    public class DslConverter
    {
        public class Info : EventArgs
        {
            public string Name;
            public string SourceLanguage;
            public string TargetLanguage;

            public int WordCount;
            public int TranslationCount;
            public double Progress;
        }

        public event EventHandler<Info> ProgressHandler;

        private string mFilePath;
        private int OpsCount;

        public DslConverter(string filePath)
        {
            mFilePath = filePath;
        }

        public void Import(Dictionary dict)
        {
            using (FileStream fs = new FileStream(mFilePath, FileMode.Open, FileAccess.Read))
            {
            using (StreamReader stream = new StreamReader(fs, Encoding.Unicode))
            {
                string line = null;
                string word = null;
                List<string> translations = new List<string>();
                Info info = new Info();

                while ((line = stream.ReadLine()) != null)
                {
                    if (line.StartsWith("#"))
                    {
                        if ( line.StartsWith( "#INDEX_LANGUAGE" ) )
                        {
                            info.SourceLanguage = line.Substring(line.IndexOf('"'), line.LastIndexOf('"'));
                        }
                        else if ( line.StartsWith( "#CONTENTS_LANGUAGE" ) )
                        {
                            info.TargetLanguage = line.Substring(line.IndexOf('"'), line.LastIndexOf('"'));
                        }
                        else if ( line.StartsWith( "#NAME" ) )
                        {
                            info.Name = line.Substring(line.IndexOf('"'), line.LastIndexOf('"'));
                        }
                    }
                    else if ( line.Length != 0 && !Char.IsSeparator(line[0]) )
                    {
                        word = line;
                    }
                    else if ( line.Contains("[trn]") )
                    {
                        int start = line.IndexOf("[trn]") + "[trn]".Length;
                        int end = line.IndexOf("[/trn]");
                        if ( end > start )
                        {
                            translations.Add( line.Substring( start, end - start ) );
                        }
                    }
                    else if ( line.Trim().Length == 0)
                    {
                        Dictionary.Word w = dict.AddWord(word, translations);
                        if (w == null)
                        {
                            Debug.WriteLine("Importing failed on word '{0}'", word);
                            break;
                        }
                        info.WordCount++;
                        info.TranslationCount += translations.Count;

                        OpsCount++;
                        if (OpsCount % 100 == 0)
                        {
                            dict.SubmitChanges();

                            info.Progress = fs.Position * 100.0 / fs.Length;
                            if (ProgressHandler != null)
                            {
                                ProgressHandler.Invoke(this, info);
                            }
                        }

                        translations.Clear();
                    }
                }
                dict.SubmitChanges();
            }
            }
        }
    }
}
