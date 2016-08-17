using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OrariDipendenti
{
    /// <summary>
    /// Logica di interazione per addDipendnente.xaml
    /// </summary>
    public partial class addEntrataUscita : MetroWindow
    {
        private addentratauscita aeu = new addentratauscita();
        private int _errors = 0;

        public addEntrataUscita(string id_dip, string id_nome)
        {
            InitializeComponent();
            this.DataContext = aeu;
            aeu.pausa = "00:00";
            label_addpresenza_nome.Content = id_nome;
            label_addpresenza_iddip.Content = id_dip;
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _errors == 0;
            e.Handled = true;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Debug.WriteLine("ssssssssssssssssssssssssss");
            e.Handled = true;
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _errors++;
            else
                _errors--;
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

        public string addpresenza_id
        {
            get { return label_addpresenza_iddip.Content.ToString(); }
        }

        public string addpresenza_nome
        {
            get { return label_addpresenza_nome.Content.ToString(); }
        }

        public string addpresenza_giorno
        {
            get { return dp_addpresenza_giorno.Text; }
        }

        public string addpresenza_entrata
        {
            get { return tb_addpresenza_entrata.Text; }
        }

        public string addpresenza_uscita
        {
            get { return tb_addpresenza_uscita.Text; }
        }

        public string addpresenza_pausa
        {
            get { return tb_addpresenza_pausa.Text; }
        }

        public string addpresenza_note
        {
            get { return tb_addpresenza_note.Text; }
        }
    }
}