using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;

namespace FiruModel
{
    public class Vocabulary : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string IsoConnectionString =
            "Data Source=isostore:/vocabulary.sdf";

        // Pass the connection string to the base class.
        public Vocabulary(string connectionString)
            : base(connectionString)
        {
        }

        public static Vocabulary Open(string connectionString)
        {
            Vocabulary self = new Vocabulary(connectionString);
            if (!self.DatabaseExists())
            {
                self.CreateDatabase();
            }

            self.Upgrade();

            return self;
        }

        private void Upgrade()
        {
            DatabaseSchemaUpdater dbUpdate = this.CreateDatabaseSchemaUpdater();

            int requiredDbVersion = 0;
            // add upgrade code here
            if (dbUpdate.DatabaseSchemaVersion < requiredDbVersion)
            {
                dbUpdate.DatabaseSchemaVersion = requiredDbVersion;
                dbUpdate.Execute();
            }
        }
        
        public Word AddWord(Dictionary.Word dictWord, Dictionary dict, bool submit = true)
        {
            Word w = new Word { Text = dictWord.Text, SourceLang = dict.Description.SourceLanguage };
            foreach (Dictionary.Translation dt in dictWord.Translations)
            {
                Translation t = new Translation
                    { Word = w, Text = dt.Text, TargetLang = dict.Description.TargetLanguage,
                     ReverseMark = MarkValue.ToLearn, ForwardMark = MarkValue.ToLearn};
                Translations.InsertOnSubmit(t);
                w.Translations.Add(t);
            }
            Words.InsertOnSubmit(w);
            if (submit)
            {
                SubmitChanges();
            }
            return w;
        }

        public Table<Word> Words; // should be map of language
        public Table<Translation> Translations; // should be map of language pair

        [Table(Name = "words")]
        [Index(Columns = "Text")]
        [Index(Columns = "SourceLang")]
        public class Word : WordBase
        {
            private int _ID;
            private string _Text;
            private string _Lang;
            private EntitySet<Translation> _Translations;

            public Word()
            {
                this._Translations = new EntitySet<Translation>();
            }

            [Column(Name = "id", Storage = "_ID", IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
            public int ID
            {
                get
                {
                    return _ID;
                }
            }

            [Column(Name = "text", Storage="_Text")]
            public string Text
            {
                get
                {
                    return _Text;
                }
                set
                {
                    if (_Text != value)
                    {
                        NotifyPropertyChanging("Text");
                        _Text = value;
                        NotifyPropertyChanged("Text");
                    }
                }
            }

            [Column(Name = "lang", Storage="_Lang")]
            public string SourceLang
            {
                get
                {
                    return _Lang;
                }
                set
                {
                    if (_Lang != value)
                    {
                        NotifyPropertyChanging("SourceLang");
                        _Lang = value;
                        NotifyPropertyChanged("SourceLang");
                    }
                }
            }

            [Association(Storage = "_Translations", OtherKey = "WordID")]
            public EntitySet<Translation> Translations
            {
                get { return this._Translations; }
                set { this._Translations.Assign(value); }
            }
        }

        [Table(Name = "translations")]
        [Index(Columns = "ForwardMark")]
        [Index(Columns = "ReverseMark")]
        public class Translation : TranslationBase
        {
            private int _ID;
            private string _Lang;
            private MarkValue _fmark;
            private MarkValue _rmark;
            private string _Text;
            [Column(Name = "word_id")]
            public int WordID;
            private EntityRef<Word> _Word;

            public Translation()
            {
            }

            [Column(Name = "id", Storage = "_ID", IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
            public int ID
            {
                get
                {
                    return _ID;
                }
            }

            [Column(Name = "text")]
            public string Text
            {
                get
                {
                    return _Text;
                }
                set
                {
                    if (_Text != value)
                    {
                        NotifyPropertyChanging("Text");
                        _Text = value;
                        NotifyPropertyChanged("Text");
                    }
                }
            }

            [Association(
                Name = "FK_translations_words", IsForeignKey = true,
                Storage = "_Word", ThisKey = "WordID")]
            public Word Word
            {
                get { return this._Word.Entity; }
                set { this._Word.Entity = value; }
            }
            
            [Column(Name = "lang")]
            public string TargetLang
            {
                get
                {
                    return _Lang;
                }
                set
                {
                    if (_Lang != value)
                    {
                        NotifyPropertyChanging("TargetLang");
                        _Lang = value;
                        NotifyPropertyChanged("TargetLang");
                    }
                }
            }

            [Column(Name = "fmark", Storage = "_fmark", DbType = "TINYINT NOT NULL")]
            public MarkValue ForwardMark
            {
                get
                {
                    return _fmark;
                }
                internal set
                {
                    if (_fmark != value)
                    {
                        NotifyPropertyChanging("ForwardMark");
                        _fmark = value;
                        NotifyPropertyChanged("ForwardMark");
                    }
                }
            }

            public void UpdateForwardMark(TestResult testResult)
            {
                ForwardMark = Mark.UpdateToTestResult(ForwardMark, testResult);
            }

            [Column(Name = "rmark", Storage = "_rmark", DbType = "TINYINT NOT NULL")]
            public MarkValue ReverseMark
            {
                get
                {
                    return _rmark;
                }
                internal set
                {
                    if (_rmark != value)
                    {
                        NotifyPropertyChanging("ReverseMark");
                        _rmark = value;
                        NotifyPropertyChanged("ReverseMark");
                    }
                }
            }

            public void UpdateReverseMark(TestResult testResult)
            {
                ReverseMark = Mark.UpdateToTestResult(ReverseMark, testResult);
            }
        }
    }
}
