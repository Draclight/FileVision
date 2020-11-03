using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileVision.UI
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MenuItemOuvrirFichier.Click += new RoutedEventHandler(OuvrirFichier);
            MenuItemEnregistrerFichier.Click += new RoutedEventHandler(EnregistrerFichier);
            MenuItemQuitter.Click += new RoutedEventHandler(QuitterApplication);
        }
        /// <summary>
        /// Ouvre une boite de sélection d'un fichier texte pour en afficher le contenu
        /// </summary>
        /// <param name="sender">menu item ouvrir un fichier</param>
        /// <param name="e">click</param>
        private void OuvrirFichier(object sender, RoutedEventArgs e)
        {
            //Chemin du fichier
            string filePath = string.Empty;
            //Contenu du fichier
            string fileContent = string.Empty;

            //Dialog
            OpenFileDialog dialog = new OpenFileDialog();
            //Dossier d'ouverture de base
            dialog.InitialDirectory = Environment.CurrentDirectory;
            dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.Multiselect = false;

            //Ouverture de la dialog
            if (dialog.ShowDialog().HasValue)
            {
                //Récupère le chemin du fichier
                filePath = dialog.FileName;

                //Lit le contenu du fichier
                var fileStream = dialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }

                txtContent.Text = fileContent;
            }
        }

        private void EnregistrerFichier(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void QuitterApplication(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
