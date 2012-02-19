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
using FiruModel;

namespace FiruPhoneImport
{
    public partial class Import : PhoneApplicationPage
    {
        public Import()
        {
            InitializeComponent();
        }

        public void DslConverterProgressHandler(object sender, DslConverter.Info e)
        {
        }

        private void ImportDsl(string path)
        {
            DslConverter dsl = new DslConverter(path);
            dsl.ProgressHandler += new EventHandler<DslConverter.Info>(DslConverterProgressHandler);
            using (Dictionary dict = new Dictionary(Dictionary.IsoConnectionString))
            {
                dsl.Import(dict);
            }
        }
    }
}