using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OrariDipendenti
{
    /// <summary>
    /// Logica di interazione per addDipendnente.xaml
    /// </summary>
    public partial class addMalattia : MetroWindow
    {
        private int _errors = 0;
        //public malattia mal = new malattia();

        public addMalattia(malattia m)
        {
            // mal.al_giorno = DateTime.Now;
            InitializeComponent();
            DataContext = m;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string addpresenza_giorno
        {
            get { return dp_dal_giorno.Text; }
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
    }
}