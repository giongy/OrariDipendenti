using MahApps.Metro.Controls;
using System.Windows;

namespace OrariDipendenti
{
    /// <summary>
    /// Logica di interazione per edit_orario.xaml
    /// </summary>
    public partial class edit_orario : MetroWindow
    {
        public string orari_nome_edit { get; set; }
        public string orari_ore_edit { get; set; }

        public edit_orario()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}