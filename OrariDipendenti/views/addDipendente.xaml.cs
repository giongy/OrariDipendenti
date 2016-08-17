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
    public partial class addDipendente : MetroWindow
    {
        private int _errors = 0;
        public dipendente dip = new dipendente();

        public addDipendente(string question, string defaultAnswer = "cognome")
        {
            InitializeComponent();
            DataContext = dip;
            dip.index = -1;
            nome_dipendente_add.Text = question;
            cognome_dipendente_add.Text = defaultAnswer;
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

        public string nome_dipendente
        {
            get { return nome_dipendente_add.Text; }
        }

        public string cognome_dipendente
        {
            get { return cognome_dipendente_add.Text; }
        }

        public string note_dipendente
        {
            get { return note_dipendente_add.Text; }
        }

        public string inServizio_dipendente
        {
            get { return inServizio_dipendente_add.Text; }
        }

        public string orario_dipendente
        {
            get { return orario_dipendente_add.Text; }
        }
    }
}