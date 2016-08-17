using MahApps.Metro.Controls;
using System.Windows;

namespace OrariDipendenti
{
    /// <summary>
    /// Interaction logic for password.xaml
    /// </summary>
    public partial class password_change : MetroWindow
    {
        public password_change()
        {
            InitializeComponent();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            if (textBox_password_now.Password == Properties.Settings.Default.userpw) //se hai messo la corretta password corrente
            {
                if (textBox_password_new.Password == textBox_password_new2.Password) // se hai confermato la nuova passw
                {
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Verifica se hai inserito le stessa password"); //ripeti non corretto
                }
            }
            else // hai messo la vecchia password sbagliata
            {
                MessageBox.Show("Controlla la password corrente");
            }
        }
    }
}