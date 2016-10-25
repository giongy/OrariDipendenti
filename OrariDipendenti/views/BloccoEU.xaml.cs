using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OrariDipendenti
{
    /// <summary>
    /// Logica di interazione per BloccoEU.xaml
    /// </summary>

    public partial class BloccoEU : UserControl
    {
        private string nomecompleto;
        private MetroWindow metroWindow = (Application.Current.MainWindow as MetroWindow);

        public string Nomecompleto
        {
            get
            {
                return nomecompleto;
            }

            set
            {
                nomecompleto = value;
            }
        }

        public BloccoEU()
        {
            InitializeComponent();
        }

        private async void button_Click_entra(object sender, RoutedEventArgs e)
        {
            sql_entrate_uscite eus = new sql_entrate_uscite();
            Hashtable check_uscita_precedente = eus.check_uscita_precedente(this.Tag.ToString()); //controllo se uscita precedente ok

            if ((string)check_uscita_precedente["check"] == MyGlobals.exitmissed) //se non ancora uscito allora chiama uscita
            {
                Esci esci = new Esci(check_uscita_precedente["giorno_da_sistemare"].ToString(), MyGlobals.exitmissednote(check_uscita_precedente["giorno_da_sistemare"].ToString()), check_uscita_precedente["note"].ToString());
                esci.timePicker_esci.IsOpen = true;
                esci.timePicker_esci.StartTime = RoundUp(DateTime.Today, TimeSpan.FromMinutes(5)).TimeOfDay;
                esci.timePicker_esci.Value = RoundUp(DateTime.Now, TimeSpan.FromMinutes(5));

                if (esci.ShowDialog() == true)
                {
                    double seconds_entrata = TimeSpan.Parse(check_uscita_precedente["entrata"].ToString()).TotalSeconds;
                    double seconds_uscita = TimeSpan.Parse(esci.Ora.ToString()).TotalSeconds;
                    if (seconds_uscita - seconds_entrata < 0) //se uscita è prima di entrata
                    {
                        //MessageBox.Show((seconds_uscita - seconds_entrata).ToString() + " Controlla l'ora di uscita: è precedente all'entrata");
                        //MessageBox.Show("Controlla l'ora di uscita: è precedente all'entrata");
                        await metroWindow.ShowMessageAsync("Qualcosa non mi torna", "Controlla l'ora di uscita: è precedente all'entrata", MessageDialogStyle.Affirmative, MyGlobals.myMetroSettings());
                    }
                    else //tutto ok, esco e faccio update della tabella
                    {
                        eus.add_new_uscita(esci.Ora, esci.Pausa, check_uscita_precedente["id"].ToString(), esci.tb_esci_note.Text);
                        Log.LogMessageToDb("Uscita giorno dopo: " + check_uscita_precedente["nome"].ToString() + " giorno: " +
                                                check_uscita_precedente["giorno_da_sistemare"].ToString() + " uscita: " +
                                                esci.Ora + " pausa: " + esci.Pausa);
                        MainWindow m = (MainWindow)Window.GetWindow(this);
                        m.aggiorna();
                    }
                }
            }
            else //se uscita prec. ok allora chiamo ENTRA
            {
                Entra entra = new Entra();
                entra.timePicker_entra.IsOpen = true;
                //TimeSpan t = DateTime.Now.TimeOfDay;

                entra.timePicker_entra.StartTime = RoundUp(DateTime.Today, TimeSpan.FromMinutes(5)).TimeOfDay;
                //entra.timePicker_entra.Value = RoundUp(DateTime.Now, TimeSpan.FromMinutes(5));
                entra.label_entra.Content = "Buongiorno " + blocco_nome.Text;
                if (entra.ShowDialog() == true)
                {
                    sql_entrate_uscite eus1 = new sql_entrate_uscite();
                    string entrata_result = eus1.add_new_entrata(entra.timePicker_entra.Text, this.Tag.ToString(), Nomecompleto);
                    if (entrata_result != MyGlobals.ok)
                    {
                        MessageBox.Show(entrata_result);
                    }
                    Log.LogMessageToDb("Entrata: " + nomecompleto + "  " + entra.timePicker_entra.Text);
                    MainWindow m = (MainWindow)Window.GetWindow(this);
                    m.aggiorna();
                }
            }
        }

        private async void button_Click_esci(object sender, RoutedEventArgs e)
        {
            string oggi = System.DateTime.Today.ToString("yyyy-MM-dd");
            sql_entrate_uscite eus = new sql_entrate_uscite();
            Hashtable check_oggi = eus.check_oggi(this.Tag.ToString());

            if ((string)check_oggi["alreadyenter"] == "true") //se sei già entrato
            {
                string arrivederci = "Arrivederci " + check_oggi["nome_dipendente"] + ", oggi sei entrato alle " + check_oggi["entrata"];
                Esci esci = new Esci(oggi, arrivederci, check_oggi["note"].ToString());
                esci.timePicker_esci.IsOpen = true;
                esci.timePicker_esci.StartTime = RoundUp(DateTime.Today, TimeSpan.FromMinutes(5)).TimeOfDay;
                esci.timePicker_esci.Value = RoundUp(DateTime.Now, TimeSpan.FromMinutes(5));
                if (esci.ShowDialog() == true)
                {
                    double seconds_entrata = TimeSpan.Parse(check_oggi["entrata"].ToString()).TotalSeconds;
                    double seconds_uscita = TimeSpan.Parse(esci.Ora.ToString()).TotalSeconds;
                    if (seconds_uscita - seconds_entrata < 0) //se uscita è prima di entrata
                    {
                        await metroWindow.ShowMessageAsync("Qualcosa non mi torna", "Controlla l'ora di uscita: è precedente all'entrata", MessageDialogStyle.Affirmative, MyGlobals.myMetroSettings());
                        //MessageBox.Show((seconds_uscita - seconds_entrata).ToString() + " Controlla l'ora di uscita: è precedente all'entrata");
                    }
                    else //tutto ok, esco e faccio update della tabella
                    {
                        eus.add_new_uscita(esci.Ora, esci.Pausa, check_oggi["id"].ToString(), esci.tb_esci_note.Text);
                        Log.LogMessageToDb("Uscita: " + nomecompleto + "  " + esci.Ora + " " + esci.Pausa + " " + esci.tb_esci_note.Text);
                        MainWindow m = (MainWindow)Window.GetWindow(this);
                        m.aggiorna();
                    }
                }
            }
            else //altrimenti ti dico che prima devi entrare per poter cliccare su esci
            {
                
                await metroWindow.ShowMessageAsync("Guarda meglio...", "Prima devi entrare", MessageDialogStyle.Affirmative, MyGlobals.myMetroSettings());
                //MessageBox.Show("Prima devi entrare!");
            }
        }

        private async void blocco_nome_Copy_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var block = sender as TextBlock;

            sql_entrate_uscite eus = new sql_entrate_uscite();
            Hashtable check_oggi = eus.check_oggi(this.Tag.ToString());

            if ((string)check_oggi["alreadyenter"] == "true") //se sei già entrato
            {
                Debug.WriteLine(check_oggi["nome_dipendente"] + " " + check_oggi["entrata"] + " " + check_oggi["id"]);
                Note note = new Note(block.Text.Substring(5));
                if (note.ShowDialog() == true)
                {
                    eus.modify_note(note.tb_esci_note.Text, check_oggi["id"].ToString());
                    MainWindow m = (MainWindow)Window.GetWindow(this);
                    m.aggiorna();
                }
            }
            else
            {
                await metroWindow.ShowMessageAsync("Guarda meglio...", "Per inserire delle note devi prima entrare", MessageDialogStyle.Affirmative, MyGlobals.myMetroSettings());
                //MessageBox.Show("Per inserire delle note devi prima entrare");
            }
        }

        private DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
        }
    }
}