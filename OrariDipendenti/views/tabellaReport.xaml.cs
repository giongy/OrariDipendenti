using System.Windows;
using System.Windows.Controls;

namespace OrariDipendenti
{
    /// <summary>
    /// Interaction logic for tabellaReport.xaml
    /// </summary>
    public partial class tabellaReport : UserControl
    {
        public tabellaReport()
        {
            InitializeComponent();
        }

        private void Button_Click_report_modifica(object sender, RoutedEventArgs e)
        {
            Report r = ((FrameworkElement)sender).DataContext as Report;

            editEntrataUscita ed = new editEntrataUscita(r.report_eu_id, r.report_id_dip, r.report_nome, r.report_giorno, r.report_entrata, r.report_uscita, r.report_pausa, r.report_note);
            //ed.nome_dipendente = o.nome_dipendente.ToString();

            if (ed.ShowDialog() == true)
            {
                sql_entrate_uscite eusql = new sql_entrate_uscite();
                string ok = eusql.edit_entrata_uscita(r.report_eu_id, ed.tb_editpresenza_entrata.Text, ed.tb_editpresenza_uscita.Text, ed.tb_editpresenza_pausa.Text, ed.tb_editpresenza_note.Text);
                if (ok == MyGlobals.ok)
                {
                    refresh();
                }
            }
        }

        private void Button_Click_report_elimina(object sender, RoutedEventArgs e)
        {
            Report r = ((FrameworkElement)sender).DataContext as Report;

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Sei proprio sicuro? ", "Conferma", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                sql_entrate_uscite eusql = new sql_entrate_uscite();
                eusql.delete_entrata_uscita(r.report_eu_id);
                refresh();
            }
        }

        public DataGrid get_tabella_mensile
        {
            get { return dataGrid_report_mensile; }
        }

        private void refresh()
        {
            MainWindow m = (MainWindow)Window.GetWindow(this);
            if (m.tabitem_report_mensile.IsSelected)
                m.button_cerca_mensile_Click(this, new RoutedEventArgs()); // REFRESH DELLA TABELLA REPORT, cercando di nuovo
            if (m.tabitem_report_giornaliero.IsSelected)
                m.button_cerca_giornaliero_Click(this, new RoutedEventArgs()); // REFRESH DELLA TABELLA REPORT, cercando di nuovo
        }
    }
}