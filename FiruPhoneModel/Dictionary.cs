using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;

namespace FiruModel
{
    public class Dictionary : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string IsoConnectionString = 
            "Data Source=isostore:/dict_fi_ru.sdf";
        public static string AppConnectionString =
            "Data Source=appdata:/dict_fi_ru.sdf; File Mode = read only;";

        public int TotalWords { get; private set; }
        public int TotalTranslations { get; private set; }
        public Information Description { get; private set; }

        // Pass the connection string to the base class.
        Dictionary(string connectionString)
            : base(connectionString)
        {
        }

        public static Dictionary Open(string connectionString)
        {
            Dictionary self = new Dictionary(connectionString);
            if (!self.DatabaseExists())
            {
                self.CreateDatabase();
            }

            self.Upgrade();

            self.TotalWords = self.Words.Count();
            self.TotalTranslations = self.Translations.Count();
            self.Description = self.Info.FirstOrDefault();

            return self;
        }

        private void Upgrade()
        {
            DatabaseSchemaUpdater dbUpdate = this.CreateDatabaseSchemaUpdater();
            
            int requiredDbVersion = 1;
            if (dbUpdate.DatabaseSchemaVersion < 1)
            {
                dbUpdate.AddTable<Information>();
            }
            if (dbUpdate.DatabaseSchemaVersion < requiredDbVersion)
            {
                dbUpdate.DatabaseSchemaVersion = requiredDbVersion;
                dbUpdate.Execute();
            }
        }

        public Word AddWord(string word, List<string> translations, bool submit = true)
        {
            Word w = new Word { Text = word };
            foreach (string ts in translations)
            {
                Translation t = new Translation { Word = w, Text = ts };
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

        public List<Word> SearchWords(string startsWith)
        {
            var words = from w in Words 
                        where w.Text.StartsWith(startsWith)
                        select w;

            return words.ToList<Word>();
        }

        public Table<Word> Words; // should be map of language
        public Table<Translation> Translations; // should be map of language pair
        public Table<Information> Info;

        [Table(Name = "words")]
        [Index(Columns = "Text")]
        public class Word : WordBase
        {
            private int _ID;
            private string _Text;
            private EntitySet<Translation> _Translations;

            public Word()
            {
                this._Translations = new EntitySet<Translation>();
            }

            [Column(Name = "id", Storage="_ID", IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
            public int ID
            {
                get
                {
                    return _ID;
                }
            }

            [Column(Name = "text", Storage = "_Text")]
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
                        //NotifyPropertyChanging("Text");
                        _Text = value;
                        //NotifyPropertyChanged("Text");
                    }
                }
            }

            [Association(Storage = "_Translations", OtherKey = "WordID")]
            public EntitySet<Dictionary.Translation> Translations
            {
                get { return this._Translations; }
                set { this._Translations.Assign(value); }
            }
        }

        [Table(Name = "translations")]
        public class Translation : TranslationBase
        {
            private int _ID;
            private string _Text;

            [Column(Name = "id", Storage = "_ID", IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
            public int ID
            {
                get
                {
                    return _ID;
                }
            }

            [Column(Name = "text", Storage = "_Text")]
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
                        //NotifyPropertyChanging("Text");
                        _Text = value;
                        //NotifyPropertyChanged("Text");
                    }
                }
            }
            
            [Column(Name = "word_id")]
            private int WordID;

            private EntityRef<Word> _Word;
            [Association(
                Name = "FK_translations_words", IsForeignKey = true,
                Storage = "_Word", ThisKey = "WordID", OtherKey="ID")]
            public Word Word
            {
                get { return this._Word.Entity; }
                set { this._Word.Entity = value; }
            }
        }

        [Table(Name = "info")]
        public class Information
        {
            [Column(Name = "source")]
            public string OriginalFile;

            [Column(Name = "sourceFormat")]
            public string OriginalFormat;

            [Column(Name = "name")]
            public string Name;

            [Column(Name = "src_lang")]
            public string SourceLanguage;

            [Column(Name = "trg_lang")]
            public string TargetLanguage;
        }
    }
}
