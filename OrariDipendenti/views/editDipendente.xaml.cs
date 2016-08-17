using MahApps.Metro.Controls;
using System;
using System.Windows;

namespace OrariDipendenti
{
    /// <summary>
    /// Logica di interazione per editDipendnente.xaml
    /// </summary>
    public partial class editDipendente : MetroWindow
    {
        public editDipendente(string inservizio, string orario)
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // txtAnswer.SelectAll();
            //txtAnswer.Focus();
        }

        public string nome_dipendente
        { get; set; }

        public string cognome_dipendente
        { get; set; }

        public string note_dipendente
        { get; set; }

        public string inServizio_dipendente
        { get; set; }

        public string orario_dipendente
        { get; set; }
    }
}