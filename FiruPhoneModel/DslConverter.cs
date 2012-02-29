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

        private bool mCancel;

        public DslConverter()
        {
        }

        public void Import(FileStream fs, Dictionary dict)
        {
            mCancel = false;
            using (StreamReader stream = new StreamReader(fs, Encoding.Unicode))
            {
                string line = null;
                string word = null;
                List<string> translations = new List<string>();
                Info info = new Info();

                while ((line = stream.ReadLine()) != null)
                {
                    if (mCancel) break;

                    if (line.StartsWith("#"))
                    {
                        if (line.StartsWith("#INDEX_LANGUAGE"))
                        {
                            info.SourceLanguage = GetSubstringBetween(line, "\"");
                        }
                        else if (line.StartsWith("#CONTENTS_LANGUAGE"))
                        {
                            info.TargetLanguage = GetSubstringBetween(line, "\"");
                        }
                        else if (line.StartsWith("#NAME"))
                        {
                            info.Name = GetSubstringBetween(line, "\"");
                        }
                    }
                    else if (line.Length != 0 && !Char.IsWhiteSpace(line, 0))
                    {
                        word = line;
                    }
                    else if (line.Contains("[trn]"))
                    {
                        translations.Add(GetSubstringBetween(line, "[trn]", "[/trn]"));
                    }
                    else if (line.Trim().Length == 0)
                    {
                        Dictionary.Word w = dict.AddWord(word, translations, false);
                        if (w == null)
                        {
                            Debug.WriteLine("Importing failed on word '{0}'", word);
                            break;
                        }
                        info.WordCount++;
                        info.TranslationCount += translations.Count;

                        if (info.WordCount % 100 == 0)
                        {
                            dict.SubmitChanges();

                            info.Progress = (double) fs.Position / (double) fs.Length;
                            if (ProgressHandler != null)
                            {
                                ProgressHandler.Invoke(this, info);
                            }
                        }

                        translations.Clear();
                    }
                }
                dict.SubmitChanges();

                Dictionary.Information dictInfo = new Dictionary.Information();
                dictInfo.Name = info.Name;
                dictInfo.SourceLanguage = info.SourceLanguage;
                dictInfo.TargetLanguage = info.TargetLanguage;
                dictInfo.OriginalFormat = "dsl";
                dictInfo.OriginalFile = fs.Name;

                dict.Info.InsertOnSubmit(dictInfo);
                dict.SubmitChanges();
            }
        }

        public void Cancel()
        {
            mCancel = true;
        }

        private string GetSubstringBetween(string line, string left, string right = "")
        {
            if (right.Length == 0)
            {
                right = left;
            }
            int start = line.IndexOf(left) + left.Length;
            int end = line.LastIndexOf(right);
            if (start > -1 && end > start)
            {
                return line.Substring(start, end - start);
            }
            return "";
        }
    }
}
