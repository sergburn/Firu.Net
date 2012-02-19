using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;

namespace FiruModel
{
    public class Vocabulary : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Data Source=isostore:/firu.sdf";

        // Pass the connection string to the base class.
        public Vocabulary(string connectionString)
            : base(connectionString)
        {
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
            private short _Lang;
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
            public short SourceLang
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
        [Index(Columns = "fmark")]
        [Index(Columns = "rmark")]
        public class Translation : TranslationBase
        {
            private int _ID;
            private short _Lang;
            private byte _fmark;
            private byte _rmark;
            private string _Text;
            [Column(Name = "word_id")]
            public int WordID;
            private EntityRef<Word> _Word;

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
                        //NotifyPropertyChanging("Text");
                        _Text = value;
                        //NotifyPropertyChanged("Text");
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
            public short TargetLang
            {
                get
                {
                    return _Lang;
                }
                set
                {
                    if (_Lang != value)
                    {
                        //NotifyPropertyChanging("TargetLang");
                        _Lang = value;
                        //NotifyPropertyChanged("TargetLang");
                    }
                }
            }

            [Column(Name = "fmark", Storage="_fmark")]
            public byte Fmark
            {
                get
                {
                    return _fmark;
                }
                set
                {
                    if (_fmark != value)
                    {
                        //NotifyPropertyChanging("Fmark");
                        _fmark = value;
                        //NotifyPropertyChanged("Fmark");
                    }
                }
            }

            [Column(Name = "rmark", Storage = "_rmark")]
            public byte Rmark
            {
                get
                {
                    return _rmark;
                }
                set
                {
                    if (_rmark != value)
                    {
                        //NotifyPropertyChanging("Rmark");
                        _rmark = value;
                        //NotifyPropertyChanged("Rmark");
                    }
                }
            }
        }
    }
}
