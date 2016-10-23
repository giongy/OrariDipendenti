using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OrariDipendenti
{
    /// <summary>
    /// Interaction logic for LogViewer.xaml
    /// </summary>
    public partial class LogViewer : MetroWindow
    {
        public LogViewer()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var main = App.Current.MainWindow as MainWindow;
            List<LogObject> llo = main.popola_log(tb_filtraLog.Text);
            dg_Log.ItemsSource = llo;
        }

        private void dg_Log_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "entry_time")
            {
                e.Column.CellStyle = (sender as DataGrid).FindResource("datetimecell") as Style;
            }
        }
    }
}