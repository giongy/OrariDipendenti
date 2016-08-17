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
    public partial class editEntrataUscita : MetroWindow
    {
        private addentratauscita aeu = new addentratauscita();
        private int _errors = 0;

        public editEntrataUscita(string id_eu, string id_dip, string nome, string giorno, string entrata, string uscita, string pausa, string note)
        {
            InitializeComponent();
            this.DataContext = aeu;
            Debug.WriteLine("entrata " + entrata);
            label_editpresenza_iddip.Content = id_dip;
            label_editpresenza_nome.Content = nome;
            label_editpresenza_giorno.Content = giorno;
            //tb_editpresenza_entrata.Text = entrata.Substring(0,5);
            aeu.entrata = entrata.Substring(0, 5);
            //tb_editpresenza_uscita.Text = uscita.Substring(0,5);
            aeu.uscita = uscita.Substring(0, 5);
            //tb_editpresenza_pausa.Text = pausa.Substring(0,5);
            aeu.pausa = pausa.Substring(0, 5);
            tb_editpresenza_note.Text = note;
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
    }
}