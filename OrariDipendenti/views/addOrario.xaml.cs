using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace OrariDipendenti
{
    /// <summary>
    /// Logica di interazione per addDipendnente.xaml
    /// </summary>
    public partial class addOrario : MetroWindow
    {
        private int _errors = 0;
        public orari or = new orari();

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

        public addOrario(string question, int defaultAnswer = 25)
        {
            InitializeComponent();
            grid_orari_add.DataContext = or;

            nome_orario_add.Text = question;
            //ore_settimanali_add.Value = defaultAnswer;
            or.ore_settimanali_orario = defaultAnswer;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("ttttttttttttttttttttttttttttttttttt");
            this.DialogResult = true;
        }

        public string nome_orario
        {
            get { return nome_orario_add.Text; }
        }

        public string ore_settimanali
        {
            get { return ore_settimanali_add.Text; }
        }
    }
}