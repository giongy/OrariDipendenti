using MahApps.Metro.Controls;
using System.Windows;

namespace OrariDipendenti
{
    /// <summary>
    /// Logica di interazione per Entra.xaml
    /// </summary>
    public partial class News_edit : MetroWindow
    {
        public News_edit(string note)
        {
            InitializeComponent();
            tb_news_edit.Text = note;
        }

        private void btnDialogRemove_Click(object sender, RoutedEventArgs e)

        {
            tb_news_edit.Text = "";
            Properties.Settings.Default.news = tb_news_edit.Text;
            Properties.Settings.Default.Save();
            this.DialogResult = true;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.news = tb_news_edit.Text;
            Properties.Settings.Default.Save();
            this.DialogResult = true;
        }
    }
}