using MahApps.Metro.Controls;
using System.Windows;

namespace OrariDipendenti
{
    /// <summary>
    /// Logica di interazione per Entra.xaml
    /// </summary>
    public partial class Note : MetroWindow
    {
        public Note(string note)
        {
            InitializeComponent();
            tb_esci_note.Text = note;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}