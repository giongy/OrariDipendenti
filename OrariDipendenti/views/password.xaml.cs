using MahApps.Metro.Controls;
using System.Windows;

namespace OrariDipendenti
{
    /// <summary>
    /// Interaction logic for password.xaml
    /// </summary>
    public partial class password : MetroWindow
    {
        public password()
        {
            InitializeComponent();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}