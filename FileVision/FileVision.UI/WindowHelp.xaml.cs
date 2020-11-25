using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileVision.UI
{
    /// <summary>
    /// Logique d'interaction pour WindowHelp.xaml
    /// </summary>
    public partial class WindowHelp : Window
    {
        public WindowHelp()
        {
            InitializeComponent();

            listBoxOptions.SelectionChanged += new SelectionChangedEventHandler(SelectionChanged);
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxOptions.SelectedIndex >= 0)
            {
                var ele = listBoxOptions.SelectedItem as FrameworkElement;
                string fileName = ele.Name;

                switch (fileName)
                {
                    case "aideDescription":
                        txtContent.Text = Properties.Resources.aideDescription;
                        break;
                    case "aideOuvertureFichier":
                        txtContent.Text = Properties.Resources.aideOuvertureFichier;
                        break;
                    case "aideFermetureFichier":
                        txtContent.Text = Properties.Resources.aideFermetureFichier;
                        break;
                    case "aideRecherche":
                        txtContent.Text = Properties.Resources.aideRecherche;
                        break;
                    case "aideEnregistrer":
                        txtContent.Text = Properties.Resources.aideEnregistrer;
                        break;
                }

                //string filePath = $"../../Aides/{fileName}.txt";
                //Stream fileStream = new FileStream(filePath, FileMode.Open);
                //using (StreamReader reader = new StreamReader(fileStream))
                //{
                //    txtContent.Text = reader.ReadToEnd();
                //}
                //fileStream.Close();
            }
        }
    }
}
