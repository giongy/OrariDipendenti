using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OrariDipendenti
{
    /// <summary>
    /// Logica di interazione per Entra.xaml
    /// </summary>
    public partial class Esci : MetroWindow
    {
        private string giorno;
        private string ora;
        private string pausa;
        private int _errors = 0;
        public esci es = new esci();

        public Esci(string giorno, string note, string noteentrata)
        {
            InitializeComponent();
            DataContext = es;

            Giorno = giorno; //setto giorno passato
            if (note.StartsWith("Attenzione"))
            {
                label_esci.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label_esci.ClearValue(Control.ForegroundProperty);
            }
            label_esci.Text = note;
            tb_esci_note.Text = noteentrata;

            es.uscita = RoundUp(DateTime.Now, TimeSpan.FromMinutes(5));
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _errors == 0;
            e.Handled = true;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _errors++;
            else
                _errors--;
        }

        public string Giorno
        {
            get
            {
                return giorno;
            }

            set
            {
                giorno = value;
            }
        }

        public string Ora
        {
            get
            {
                return ora;
            }

            set
            {
                ora = value;
            }
        }

        public string Pausa
        {
            get
            {
                return pausa;
            }

            set
            {
                pausa = value;
            }
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Ora = timePicker_esci.Text; //setto ora scelta
            Pausa = timePicker_esci_pausa.Text; //setto pausa scelta
        }

        private DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime(((dt.Ticks) / d.Ticks) * d.Ticks);
        }
    }
}