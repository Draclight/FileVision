using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        private static bool FichierOuvert { get; set; }
        private static bool FichierFermeture { get; set; }
        private static string CheminFichierOuvert { get; set; }
        private static string DossierOuverture { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(ChargementFenetre);
            Closed += new EventHandler(FermetureApplication);
            KeyUp += new KeyEventHandler(ToucheLacher);
            KeyDown += new KeyEventHandler(TouchePresser);
            MenuItemOuvrirFichier.Click += new RoutedEventHandler(OuvrirFichier);
            MenuItemEnregistrerFichier.Click += new RoutedEventHandler(EnregistrerFichier);
            MenuItemQuitter.Click += new RoutedEventHandler(QuitterApplication);
            MenuItemFermerFichier.Click += new RoutedEventHandler(FermerFichier);
            btFermerFichier.Click += new RoutedEventHandler(FermerFichier);
            btRechercher.Click += new RoutedEventHandler(Rechercher);
            txtContent.SelectionChanged += new RoutedEventHandler(InformationsTexte);
        }

        #region Evenements
        /// <summary>
        /// Recherche d'un text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rechercher(object sender, RoutedEventArgs e)
        {
            RechercheTexte();
        }

        /// <summary>
        /// Appui sur une touche
        /// </summary>
        /// <param name="sender">touche du clavier</param>
        /// <param name="e">tpuche appuyer</param>
        private void TouchePresser(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    txtRecherche.Focus();
                    break;
            }
        }

        /// <summary>
        /// Relache sur une touche
        /// </summary>
        /// <param name="sender">touche du clavier</param>
        /// <param name="e">tpuche appuyer</param>
        private void ToucheLacher(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (txtRecherche.IsFocused)
                    {
                        RechercheTexte();
                    }
                    break;
            }
        }

        /// <summary>
        /// Chergement de la fenêtre
        /// </summary>
        /// <param name="sender">la fenêtre</param>
        /// <param name="e">ouverture</param>
        private void ChargementFenetre(object sender, RoutedEventArgs e)
        {
            //Récupération des paramètres
            FichierFermeture = Properties.Settings.Default.FichierFermeture;

            //Vérification si il y avait un fichier ouvert lors de la dernière fermeture
            if (FichierFermeture)
            {
                //Récupération du chemin du fichier ouvert lors de la dernière fermeture
                CheminFichierOuvert = Properties.Settings.Default.DernierFichierOuvert;
                //Fichier en cours de lecture
                FichierOuvert = true;

                //Chergement du contenu du fichier
                ChargementContenuFichier(CheminFichierOuvert);
            }

            //Paramétrage Dossier d'ouverture du fichier
            DossierOuverture = Properties.Settings.Default.DossierOuverture;
        }

        /// <summary>
        /// Fermeture de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FermetureApplication(object sender, EventArgs e)
        {
            //sauvegarde du fichier courrant à la fermeture pour réouverture direct
            Properties.Settings.Default.FichierFermeture = FichierOuvert;
            //Vérification si un fichier est ouvert
            if (FichierOuvert)
            {
                Properties.Settings.Default.DernierFichierOuvert = CheminFichierOuvert;
                Properties.Settings.Default.DossierOuverture = DossierOuverture;
            }
            else
            {
                Properties.Settings.Default.DernierFichierOuvert = string.Empty;
                Properties.Settings.Default.DossierOuverture = Environment.SystemDirectory;
            }
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Ouvre une boite de sélection d'un fichier texte pour en afficher le contenu
        /// </summary>
        /// <param name="sender">menu item ouvrir un fichier</param>
        /// <param name="e">click</param>
        private void OuvrirFichier(object sender, RoutedEventArgs e)
        {
            //Dialog
            OpenFileDialog dialog = new OpenFileDialog();
            //Dossier d'ouverture de base
            dialog.InitialDirectory = DossierOuverture;
            dialog.Filter = "txt files (*.txt)|*.txt";
            dialog.FilterIndex = 1;
            dialog.Multiselect = false;

            //Ouverture de la dialog
            if (dialog.ShowDialog().HasValue)
            {
                //Un fichier est ouvert
                FichierOuvert = true;

                //Récupère le chemin du fichier
                CheminFichierOuvert = dialog.FileName;


                if (string.IsNullOrWhiteSpace(CheminFichierOuvert))
                {
                    Log(true, "Aucun fichier");
                }
                else
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    //Charge le contenu du fichier
                    ChargementContenuFichier(CheminFichierOuvert);

                    //Actualisation du dossier d'ouverture
                    DossierOuverture = System.IO.Path.GetDirectoryName(CheminFichierOuvert);
                }
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Enregisre les modifcations au fichier
        /// </summary>
        /// <param name="sender">menu item enregistrer</param>
        /// <param name="e">click</param>
        private void EnregistrerFichier(object sender, RoutedEventArgs e)
        {
            //Vérification d'un fichier ouvert
            if (FichierOuvert)
            {
                try
                {
                    //Pass the filepath and filename to the StreamWriter Constructor
                    StreamWriter sw = new StreamWriter(CheminFichierOuvert);
                    //Write a second line of text
                    sw.Write(txtContent.Text);
                    //Close the file
                    sw.Close();
                }
                catch (Exception ex)
                {
                    Log(true, ex.Message);
                }
            }
        }

        /// <summary>
        /// Quitte l'application
        /// </summary>
        /// <param name="sender">menuitem quitter</param>
        /// <param name="e">click</param>
        private void QuitterApplication(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Ferme le fichier en cours de lecture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FermerFichier(object sender, RoutedEventArgs e)
        {
            //Vide le contenu de la zone de saisie
            txtContent.Text = string.Empty;
            //Pas de fichier ouvert
            FichierOuvert = false;
            CheminFichierOuvert = string.Empty;

            //Affichage des options fichiers
            OptionsFichier();

            //Informations
            Log(false, "fichier fermé");
        }

        /// <summary>
        /// Récupère la postion dans la textebox
        /// </summary>
        /// <param name="sender">textbox</param>
        /// <param name="e">click</param>
        private void InformationsTexte(object sender, RoutedEventArgs e)
        {
            if (FichierOuvert)
                txtContent.IsEnabled = true;
            else
                txtContent.IsEnabled = false;

            //Cursor position
            int row = txtContent.GetLineIndexFromCharacterIndex(txtContent.CaretIndex);
            int col = txtContent.CaretIndex - txtContent.GetCharacterIndexFromLineIndex(row);
            txtBkCursorPosition.Text = "Line " + (row + 1) + ", Char " + (col + 1);

            //Mots
            var mots = txtContent.Text.Split(' ', '\r', '\n', '\t');
            int nbMots = mots.Where(m => !string.IsNullOrWhiteSpace(m)).Count();
            txtBkNbMots.Text = $"{nbMots} mot(s)";

            //Paragraphes
            Regex rx = new Regex(@"\r\n\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(txtContent.Text);

            txtBkNbParagrapghes.Text = $"{matches.Count + 1} paragraphe(s)";
        }
        #endregion

        #region Méthodes
        /// <summary>
        /// Recherche de chaine de caractère 
        /// </summary>
        private void RechercheTexte()
        {
            if (txtContent.Text.ToLower().Contains(txtRecherche.Text.ToLower()))
            {
                txtContent.Select(txtContent.Text.ToLower().IndexOf(txtRecherche.Text.ToLower()), txtRecherche.Text.Length);
                txtContent.Focus();
            }

            /*Regex rx = new Regex(txtRecherche.Text.ToLower(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(txtContent.Text.ToLower());

            // Report on each match.
            foreach (Match match in matches)
            {
                MatchList.Add(match);
            }*/
        }

        /// <summary>
        /// Charge le contenu du fichier
        /// </summary>
        /// <param name="cheminFichier">lien du fichier à ouvrir</param>
        private void ChargementContenuFichier(string cheminFichier)
        {
            if (string.IsNullOrWhiteSpace(cheminFichier))
            {
                Log(false, "Pas de fichier");
            }
            else
            {
                try
                {
                    Stream fileStream = new FileStream(cheminFichier, FileMode.Open);
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        txtContent.Text = reader.ReadToEnd();
                    }
                    fileStream.Close();

                    //Affichage des options fichiers
                    OptionsFichier();

                    //Info utilisateur
                    Log(false, $"Fichier {System.IO.Path.GetFileName(cheminFichier)} chargé");
                }
                catch (Exception ex)
                {
                    Log(true, ex.Message);
                }
            }
        }

        /// <summary>
        /// Affichage des options fichiers
        /// </summary>
        private void OptionsFichier()
        {
            if (FichierOuvert)
            {
                spOptionsFichiers.Visibility = Visibility.Visible;
                MenuItemEnregistrerFichier.IsEnabled = true;
                MenuItemFermerFichier.IsEnabled = true;
                btFermerFichier.Visibility = Visibility.Visible;
            }
            else
            {
                spOptionsFichiers.Visibility = Visibility.Collapsed;
                MenuItemEnregistrerFichier.IsEnabled = false;
                MenuItemFermerFichier.IsEnabled = false;
                btFermerFichier.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Log les informations dans la bar de status
        /// </summary>
        /// <param name="isErreur">flag d'erreur</param>
        /// <param name="msg">message à afficher</param>
        private void Log(bool isErreur, string msg)
        {
            if (isErreur)
            {
                imgErreur.Visibility = Visibility.Visible;
                imgPasErreur.Visibility = Visibility.Collapsed;
                txtBkErreur.Text = $"Erreur: {msg}";
            }
            else
            {
                imgErreur.Visibility = Visibility.Collapsed;
                imgPasErreur.Visibility = Visibility.Visible;
                txtBkErreur.Text = $"Information: {msg}";
            }

            Mouse.OverrideCursor = null;
        }
        #endregion
    }
}
