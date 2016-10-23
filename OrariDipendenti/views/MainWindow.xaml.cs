using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace OrariDipendenti
{
    public partial class MainWindow : MetroWindow
    {
        private static MainWindow _myWindow;
        private Popup codePopup;

        private DispatcherTimer timer = new DispatcherTimer();

        private IScheduler sched = new StdSchedulerFactory().GetScheduler();

        private ITrigger trigger = TriggerBuilder.Create().WithIdentity("aaa")
                .WithCronSchedule("0 0 4 * * ?")
                .Build();

        //   "0 0 4 * * ?"
        private ITrigger trigger_0100 = TriggerBuilder.Create().WithIdentity("refresh")
                .WithCronSchedule("0 0 1 * * ?")
                .Build();

        public MainWindow()
        {
            Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter { Level = Common.Logging.LogLevel.Info };

            InitializeComponent();

            _myWindow = this;

            initTable it = new initTable();
            it.init();

            Log.LogMessageToDb("-*- ------------------------- APP START ----------------");

            label_version.Content = "Versione " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.95);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth * 1);
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            //nome db
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Debug.WriteLine("-------------------------------SETTINGS UPGRADE");
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
            provaquartz();
            check_db();

            tabella_orari();
            tabella_dipendenti();

            popola_lista_dipendenti();
            Properties.Settings.Default.admin = false;
        }

        public static MainWindow MyWindow
        {
            get
            {
                return _myWindow;
            }
        }

        private void provaquartz()
        {
            sched.Start();

            IJobDetail job1 = JobBuilder.Create<job_refresh>()
                 .WithIdentity("myJob1")
                 .Build();

            sched.ScheduleJob(job1, trigger_0100);
        }

        private void refresh_db()
        {
            aggiorna();
            check_db();
        }

        private void check_db()
        {
            label_db_aperto.Content = "DataBase in uso: " + Properties.Settings.Default.nomeDBAperto;
            Log.LogMessageToDb("-*- database refresh eseguito. in uso: " + Properties.Settings.Default.nomeDBAperto);
            if (Properties.Settings.Default.nomeDBAperto != MyGlobals.default_db_aperto)
            {
                MessageBox.Show(MyGlobals.alert_db());
                label_db_aperto.Background = Brushes.Red;
            }
            else
            {
                // label_db_aperto.ClearValue(TextBox.BackgroundProperty); //resetto label
                label_db_aperto.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xDE, 0xED, 0xF1)); //#FFDEEDF1
                //scheduler
                if (Properties.Settings.Default.backupAuto == true)
                {
                    Properties.Settings.Default.backupAuto = false; //imbroglio la funzione in modo da attivarlo
                }
                else
                {
                    Properties.Settings.Default.backupAuto = true; //imbroglio la funzione in modo da disattivarlo
                }
                function_backup_scheduler_db();
            }
        }

        //************************************************************
        // GRIGLIA ENTRATE
        //************************************************************
        private void griglia_entrate()
        {
            //svuoto grilgia
            mainWrap.Children.Clear();
            sql_dipendenti dipendenti = new sql_dipendenti();
            DataTable dipendenti_dt = dipendenti.select_dipendenti(MyGlobals.dip_inservizio);
            foreach (DataRow row in dipendenti_dt.Rows)
            {
                BloccoEU beu = new BloccoEU();
                beu.blocco_nome.Text = row["nome"].ToString() + " " + row["cognome"].ToString().Substring(0, 1) + ".";
                beu.Nomecompleto = row["nome"].ToString() + " " + row["cognome"].ToString();
                beu.SetValue(FrameworkElement.TagProperty, row["id"].ToString()); //IMPORTANTE: setto l'id dipendente nel tag del blocco
                sql_entrate_uscite eus = new sql_entrate_uscite();
                Hashtable h = eus.check_oggi(row["id"].ToString()); //controllo entrate uscite note di oggi
                beu.tb_blocco_note.Text = "Note: " + (string)h["note"];
                if (!string.IsNullOrEmpty((string)h["entrata"]))
                { // se per oggi ho una riga entrata non vuota allora metto label
                    beu.label_entrata.Text = "Entrata " + h["entrata"].ToString().Substring(0, 5); //label con orario di entrata
                    beu.btn_entro.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, 0x94, 0xf3, 0x68)); //e faccio verde il bottone ##FF94F368
                    //anche uscita
                    if ((string)h["uscita"] != "00:00:00")  //se uscita diverso da 00:00:00 allora sei anche uscito
                    {
                        beu.label_uscita.Text = "Uscita " + h["uscita"].ToString().Substring(0, 5); //label con orario di uscita
                        beu.label_pausa.Text = "Pausa " + h["pausa"].ToString().Substring(0, 5); //label con orario di pausa
                        beu.btn_esco.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, 0xf7, 0x6e, 0x6e)); //e rosso #FFF76E6E
                    }
                }

                mainWrap.Children.Add(beu); // finalmente aggiungo il blocco EU alla griglia principale
            }
        }

        #region orari

        private void Button_Click_orario_modifica(object sender, RoutedEventArgs e)
        {
            orari o = ((FrameworkElement)sender).DataContext as orari;

            edit_orario eo = new edit_orario();
            eo.orari_nome_edit = o.nome_orario.ToString();
            eo.orari_ore_edit = o.ore_settimanali_orario.ToString();
            if (eo.ShowDialog() == true)
            {
                sql_orari oo = new sql_orari();
                oo.edit_orario(eo.orari_nome_edit, double.Parse(eo.orari_ore_edit), o.id_orario);
                tabella_orari();
            }
        }

        private void Button_Click_orario_delete(object sender, RoutedEventArgs e)
        {
            orari o = ((FrameworkElement)sender).DataContext as orari;
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Sei sicuro?", "Conferma", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                sql_orari oo = new sql_orari();
                string result = oo.delete_orario(o.id_orario);
                if (result != "OK")
                {
                    if (result.Contains("FOREIGN KEY constraint failed"))
                    {
                        System.Windows.MessageBox.Show("Non puoi cancellare questo orario perchè è in uso da un dipendente");
                    }
                    else { System.Windows.MessageBox.Show(result); }
                }
                tabella_orari();
            }
        }

        private void add_orario_click(object sender, RoutedEventArgs e)
        {
            addOrario inputDialog = new addOrario("", 30);
            if (inputDialog.ShowDialog() == true)
            {
                sql_orari o = new sql_orari();
                string r = o.add_new_orario(inputDialog.nome_orario, double.Parse(inputDialog.ore_settimanali));
                tabella_orari();
            }
        }

        private void tabella_orari()
        {
            sql_orari os = new sql_orari();
            DataTable dt = os.select_orario();
            List<orari> orari = new List<orari>();
            if (dt.Rows.Count == 0)
            {
                Help_iniziale hi = new Help_iniziale();
                hi.Show();
            }
            foreach (DataRow row in dt.Rows)
            {
                orari.Add(new orari()
                {
                    id_orario = int.Parse(row["id"].ToString()),
                    nome_orario = row["nome"].ToString(),
                    ore_giornaliere_orario = row["ore_giornaliere"].ToString(),
                    ore_settimanali_orario = int.Parse(row["ore_settimanali_contratto"].ToString())
                });
                Debug.WriteLine(row["nome"]);
            }
            dataGrid_orari.ItemsSource = orari;
        }

        #endregion orari

        #region dipendenti

        private void add_dipendente_click(object sender, RoutedEventArgs e)
        {
            sql_orari ds = new sql_orari();
            DataTable dorario = ds.select_orario();
            Debug.WriteLine(dorario.Rows.Count);
            if (dorario.Rows.Count == 0)
            {
                System.Windows.MessageBox.Show("aggiungi prima almeno un orario");
            }
            else
            {
                sql_orari o = new sql_orari();
                DataTable d = o.select_orario();
                List<orariDipendentiCombo> combodipor = new List<orariDipendentiCombo>();
                foreach (DataRow row in d.Rows)
                {
                    combodipor.Add(new orariDipendentiCombo() { Id = row["id"].ToString(), Name = row["nome"].ToString() });
                }

                addDipendente inputDialog = new addDipendente("", "");
                inputDialog.orario_dipendente_add.ItemsSource = combodipor;
                if (inputDialog.ShowDialog() == true)
                {
                    //lblName.Text = inputDialog.Answer;

                    orariDipendentiCombo cbp = (orariDipendentiCombo)inputDialog.orario_dipendente_add.SelectedItem;
                    string idOrario = cbp.Id;
                    string nomeOrario = cbp.Name;

                    sql_dipendenti dd = new sql_dipendenti();
                    string r = dd.add_new_dipendente(inputDialog.nome_dipendente, inputDialog.cognome_dipendente, inputDialog.note_dipendente, inputDialog.inServizio_dipendente, idOrario);

                    tabella_dipendenti();
                }
            }
        }

        private void Button_Click_dipendenti_modifica(object sender, RoutedEventArgs e)
        {
            sql_orari oo = new sql_orari();
            DataTable d = oo.select_orario();
            List<orariDipendentiCombo> combodipor = new List<orariDipendentiCombo>();
            foreach (DataRow row in d.Rows)
            {
                combodipor.Add(new orariDipendentiCombo() { Id = row["id"].ToString(), Name = row["nome"].ToString() });
            }

            dipendente o = ((FrameworkElement)sender).DataContext as dipendente;

            editDipendente ed = new editDipendente("", "");
            ed.nome_dipendente = o.nome_dipendente.ToString();
            ed.cognome_dipendente = o.cognome_dipendente.ToString();
            ed.note_dipendente = o.note_dipendente.ToString();
            ed.inServizio_dipendente = o.inServizio_dipendente.ToString();
            ed.orario_dipendente_edit.ItemsSource = combodipor;
            ed.orario_dipendente = o.orario_dipendente.ToString();
            if (ed.ShowDialog() == true)
            {
                sql_dipendenti dd = new sql_dipendenti();
                dd.edit_dipendente(ed.nome_dipendente, ed.cognome_dipendente, ed.note_dipendente, ed.inServizio_dipendente, ed.orario_dipendente_edit.SelectedValue.ToString(), o.id_dipendente);
                tabella_dipendenti();
            }
        }

        private void Button_Click_dipendenti_delete(object sender, RoutedEventArgs e)
        {
            dipendente o = ((FrameworkElement)sender).DataContext as dipendente;
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Sei sicuro? Se cancelli il dipendente perderai anche tutte le sue entrate. Puoi anche modificarlo come NON IN SERVIZIO", "Conferma", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                sql_dipendenti oo = new sql_dipendenti();
                oo.delete_dipendente(o.id_dipendente);
                tabella_dipendenti();
            }
        }

        private void tabella_dipendenti()
        {
            sql_dipendenti os = new sql_dipendenti();
            DataTable dt = os.select_dipendenti(MyGlobals.dip_tutti);
            List<dipendente> dip = new List<dipendente>();
            foreach (DataRow row in dt.Rows)
            {
                dip.Add(new dipendente()
                {
                    id_dipendente = int.Parse(row["id"].ToString()),
                    nome_dipendente = row["nome"].ToString(),
                    cognome_dipendente = row["cognome"].ToString(),
                    note_dipendente = row["note"].ToString(),
                    inServizio_dipendente = row["in_servizio"].ToString(),
                    orario_dipendente = row["orario"].ToString()
                });
                Debug.WriteLine(row["nome"]);
            }
            dataGrid_dipendenti.ItemsSource = dip;
        }

        #endregion dipendenti

        //************************************************************
        // gestione cambio TAB per refresh oggetti creati
        //************************************************************
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //do the code of main tab selection  here..
            if (EUTab.IsSelected)
            {
                Debug.WriteLine("griglia perchè eutab selected");
                griglia_entrate();
            }

            if (tab_report.IsSelected)
            {
                Debug.WriteLine("rifaccio combo");
                popola_lista_dipendenti();
            }
        }

        //************************************************************
        //  LOG CLICK
        //************************************************************
        private void MenuItem_Click(object sender, RoutedEventArgs e) //log click
        {
            //Process.Start("IExplore.exe", Log.GetTempPath() + MyGlobals.log_file + ".txt");
            LogViewer lv = new LogViewer();
            List<LogObject> llo = popola_log("");
            lv.dg_Log.ItemsSource = llo;
            lv.dg_Log.ScrollIntoView(llo[llo.Count - 1]);
            lv.ShowDialog();
        }

        internal List<LogObject> popola_log(string filtro)
        {
            sql_log sl = new sql_log();
            DataTable d = sl.select_log(filtro);
            List<LogObject> entryList = new List<LogObject>();
            foreach (DataRow row in d.Rows)
            {
                string entry_time = row["log_entry"].ToString().Substring(0, 20);
                string entry = row["log_entry"].ToString().Substring(20);
                entryList.Add(new LogObject() { entry = entry, entry_time = entry_time });
            }
            return entryList;
        }

        //************************************************************
        //  MALATTIA CLICK
        //************************************************************
        private void MenuItem_Malattia_Click(object sender, RoutedEventArgs e) //log click
        {
            malattia m = new malattia();
            m.combo_lista_dip = new List<listaDipendentiCombo>();
            m.index = -1;

            sql_report o = new sql_report();
            DataTable d = o.lista_dipendenti(MyGlobals.dip_inservizio);
            List<listaDipendentiCombo> combo_lista_dip = new List<listaDipendentiCombo>();
            foreach (DataRow row in d.Rows)
            {
                m.combo_lista_dip.Add(new listaDipendentiCombo() { Id = row["id"].ToString(), Name = row["nome"].ToString() + " " + row["cognome"].ToString() });
            }

            addMalattia addmalattia = new addMalattia(m);

            if (addmalattia.ShowDialog() == true)
            {
                sql_entrate_uscite eusql = new sql_entrate_uscite();
                string ore = eusql.check_orario_dipendente(addmalattia.combo_dipendenti.SelectedValue.ToString());
                DateTime dal = DateTime.Parse(addmalattia.dp_dal_giorno.ToString());
                DateTime al = DateTime.Parse(addmalattia.dp_al_giorno.ToString());
                foreach (DateTime day in Utilities.EachDay(dal, al))
                {
                    Debug.WriteLine(" malattia " + day + " " + day.DayOfWeek + " " + addmalattia.combo_dipendenti.SelectedValue + " " + ore + " " + addmalattia.combo_dipendenti.Text);
                    if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday)
                    {
                        string r = eusql.add_new_entrata_uscita(addmalattia.combo_dipendenti.SelectedValue.ToString(),
                                                            addmalattia.combo_dipendenti.Text,
                                                            day.ToString("yyyy-MM-dd"),
                                                            "00:00:00",
                                                            ore,
                                                            "00:00:00",
                                                            addmalattia.tb_malattia_note.Text);
                    }
                }
            }
            griglia_entrate();
        }

        //************************************************************
        //  LOGIN
        //************************************************************
        private async void MenuItem_Click_1(object sender, RoutedEventArgs e) //amministrazione click
        {
            if (Properties.Settings.Default.admin == false) //se non sono loggato
            {
                password pw = new password();
                pw.passwordBox.Focus();
                if (pw.ShowDialog() == true)
                {
                    if (pw.passwordBox.Password == Properties.Settings.Default.powerpw || pw.passwordBox.Password == Properties.Settings.Default.userpw || pw.passwordBox.Password == Properties.Settings.Default.rescuepw)
                    {
                        Properties.Settings.Default.admin = true;
                        Log.LogMessageToDb("-*- admin ha fatto login");
                        if (pw.passwordBox.Password == Properties.Settings.Default.rescuepw)
                        {
                            var path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

                            MessageBox.Show(path);
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Password sbagliata");
                        await this.ShowMessageAsync("Attenzione", "Password sbagliata");
                    }
                }
            }
            else // se sono già loggato
            {
                Properties.Settings.Default.admin = false;
                Log.LogMessageToDb("-*- admin ha fatto logoff");
            }

            admin();
        }

        private void timer_Tick(object sender, EventArgs e)//timer amministrazione
        {
            Debug.WriteLine("nascondo admin");
            Log.LogMessageToDb("-*- Admin logoff dopo timeout");
            Properties.Settings.Default.admin = false;
            (sender as DispatcherTimer).Stop();
            admin();
        }

        private void admin()
        {
            if (Properties.Settings.Default.admin == true)
            {
                //db menu
                menu_db.IsEnabled = true;
                login.Header = "Logoff";
                //tab
                tab_report.IsEnabled = true;
                Orari.IsEnabled = true;
                Dipendenti.IsEnabled = true;

                //timeout
                timer.Interval = TimeSpan.FromHours(8);
                timer.Tick += timer_Tick;
                timer.Start();
            }
            else //logoff oppure tempo scaduto
            {
                mainTab.SelectedIndex = 0;
                tab_report.IsEnabled = false;
                login.Header = "Login";
                menu_db.IsEnabled = false;
                Orari.IsEnabled = false;
                Dipendenti.IsEnabled = false;
            }
        }

        //************************************************************
        //  CAMBIO PASSWORD
        //************************************************************
        private void MenuItem_Click_changepw(object sender, RoutedEventArgs e) //password change
        {
            password_change pwc = new password_change();
            pwc.textBox_password_now.Focus();
            if (Properties.Settings.Default.userpw == MyGlobals.initial_password)
            {
                System.Windows.MessageBox.Show("Non hai ancora cambiato la password iniziale che è: " + MyGlobals.initial_password);
                pwc.textBox_password_now.Password = MyGlobals.initial_password;
            }
            if (pwc.ShowDialog() == true)
            {
                Properties.Settings.Default.userpw = pwc.textBox_password_new2.Password;
                Properties.Settings.Default.Save();
                MessageBox.Show("Password cambiata con successo");
                Log.LogMessageToDb("-*- cambio password");
            }
        }

        //************************************************************
        //  news
        //************************************************************
        private void MenuItem_Click_news(object sender, RoutedEventArgs e) //password change
        {
            News_edit newsEdit = new News_edit(Properties.Settings.Default.news);
            newsEdit.tb_news_edit.Focus();
            if (newsEdit.ShowDialog() == true)
            {
                if (Properties.Settings.Default.news != "")
                {
                    img_news.Visibility = Visibility.Visible;
                }
                else
                {
                    img_news.Visibility = Visibility.Hidden;
                }
            }
        }

        //************************************************************
        //  POPOLA COMBO DIPENDENTI E MESI PER REPORT MENSILE
        //************************************************************
        public void popola_lista_dipendenti()
        {
            sql_report o = new sql_report();

            //DIPENDENTI COMBO
            DataTable d = o.lista_dipendenti(MyGlobals.dip_tutti);
            List<listaDipendentiCombo> combo_lista_dip = new List<listaDipendentiCombo>();
            foreach (DataRow row in d.Rows)
            {
                combo_lista_dip.Add(new listaDipendentiCombo() { Id = row["id"].ToString(), Name = row["nome"].ToString() + " " + row["cognome"].ToString() });
            }
            var combodipindex = combo_lista_dipendenti.SelectedIndex;
            combo_lista_dipendenti.ItemsSource = combo_lista_dip;
            if (combodipindex != -1)
            {
                combo_lista_dipendenti.SelectedIndex = combodipindex;
            }
            else
            {
                combo_lista_dipendenti.SelectedIndex = 0;
            }

            // MESI COMBO
            DataTable d1 = o.lista_mesi();
            List<listaMesiCombo> combo_lista_mesi = new List<listaMesiCombo>();
            foreach (DataRow row in d1.Rows)
            {
                combo_lista_mesi.Add(new listaMesiCombo() { Id = row["mese"].ToString() + "-" + row["anno"].ToString(), Name = row["mese"].ToString() + "-" + row["anno"].ToString() });
            }
            var combomesiindex = comboBox_lista_mesi.SelectedIndex;
            var combomesituttiindex = comboBox_lista_mesi_tutti.SelectedIndex;
            comboBox_lista_mesi.ItemsSource = combo_lista_mesi;
            comboBox_lista_mesi_tutti.ItemsSource = combo_lista_mesi;
            //comboBox_lista_mesi.SelectedIndex = 0;
            if (combomesiindex != -1)
            {
                comboBox_lista_mesi.SelectedIndex = combomesiindex;
            }
            else
            {
                comboBox_lista_mesi.SelectedIndex = 0;
            }
            if (combomesituttiindex != -1)
            {
                comboBox_lista_mesi_tutti.SelectedIndex = combomesiindex;
            }
            else
            {
                comboBox_lista_mesi_tutti.SelectedIndex = 0;
            }
            //GIORNI COMBO
            DataTable d2 = o.lista_giorni();
            List<listaGiorniCombo> combo_lista_giorni = new List<listaGiorniCombo>();
            foreach (DataRow row in d2.Rows)
            {
                combo_lista_giorni.Add(new listaGiorniCombo() { Id = row["day"].ToString() + "-" + row["mese"].ToString() + "-" + row["anno"].ToString(), Name = row["day"].ToString() + "-" + row["mese"].ToString() + "-" + row["anno"].ToString() });
            }
            var combogiorniindex = comboBox_lista_giorni.SelectedIndex;
            comboBox_lista_giorni.ItemsSource = combo_lista_giorni;
            //comboBox_lista_mesi.SelectedIndex = 0;
            if (combogiorniindex != -1)
            {
                comboBox_lista_giorni.SelectedIndex = combogiorniindex;
            }
            else
            {
                comboBox_lista_giorni.SelectedIndex = 0;
            }
        }

        //************************************************************
        //  REPORT MENSILE - CERCA
        //************************************************************
        public void button_cerca_mensile_Click(object sender, RoutedEventArgs e)
        {
            double totale_secondi_banca_ore = 0;
            if (String.IsNullOrEmpty((string)combo_lista_dipendenti.SelectedValue) || String.IsNullOrEmpty((string)comboBox_lista_mesi.SelectedValue))
            {
                System.Windows.MessageBox.Show("seleziona dipendente e mese");
            }
            else
            {
                sql_report rs = new sql_report();

                DataTable dt = rs.mensile(combo_lista_dipendenti.SelectedValue.ToString(), comboBox_lista_mesi.SelectedValue.ToString());
                List<Report> reportlist = new List<Report>();
                foreach (DataRow row in dt.Rows)
                {
                    string ore_lavorate = row["ore_lavorate"].ToString();
                    string ore_dentro = row["ore_dentro"].ToString();
                    double seconds_oredafare = TimeSpan.Parse(row["ore_da_fare"].ToString()).TotalSeconds;
                    double seconds_orelavorate = TimeSpan.Parse(row["ore_lavorate"].ToString()).TotalSeconds;
                    if (row["uscita"].ToString() == "00:00:00")
                    {
                        seconds_orelavorate = 0;
                        ore_lavorate = "NO USCITA";
                        ore_dentro = "No USCITA";
                    }
                    double seconds_bancaore = seconds_orelavorate - seconds_oredafare;
                    totale_secondi_banca_ore = totale_secondi_banca_ore + seconds_bancaore;
                    string bancaore = Utilities.ToHMString(TimeSpan.FromSeconds(seconds_bancaore));

                    reportlist.Add(new Report()
                    {
                        report_giorno = row["giorno"].ToString(),
                        report_giorno_dayofweek = row["giorno"].ToString() + " " + Utilities.giorno_della_settimana(row["day_of_week"].ToString()),
                        report_nome = row["nome"].ToString(),
                        report_id_dip = row["id_dipendente"].ToString(),
                        report_orario = row["ore_da_fare"].ToString(),
                        report_entrata = row["entrata"].ToString(),
                        report_uscita = row["uscita"].ToString(),
                        report_note = row["note"].ToString(),
                        report_ore_dentro = ore_dentro,
                        report_pausa = row["pausa"].ToString(),
                        report_ore_lavorate = ore_lavorate,
                        report_modificato = row["modificato"].ToString(),
                        report_bancaore = bancaore,
                        report_eu_id = row["eu_id"].ToString()
                    });
                }

                tabellamensile.dataGrid_report_mensile.ItemsSource = reportlist;
                label_report_dipname.Text = combo_lista_dipendenti.Text + " " + comboBox_lista_mesi.SelectedValue.ToString();
                label_report_dipname.SetValue(FrameworkElement.TagProperty, combo_lista_dipendenti.Text); //IMPORTANTE: setto il nome
                combo_lista_dipendenti.SetValue(FrameworkElement.TagProperty, combo_lista_dipendenti.SelectedValue.ToString()); //IMPORTANTE: setto l'id dipendente nel tag del blocco
                btn_aggiungi_presenza.IsEnabled = true;
                btn_stampa.IsEnabled = true;
                label_tot_bancaore.Content = Utilities.ToHMString(TimeSpan.FromSeconds(totale_secondi_banca_ore)).Trim();
            }
        }

        //************************************************************
        //  REPORT GIORNALIERO - CERCA
        //************************************************************
        public void button_cerca_giornaliero_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty((string)comboBox_lista_giorni.SelectedValue))
            {
                System.Windows.MessageBox.Show("seleziona un giorno");
            }
            else
            {
                sql_report rs = new sql_report();

                DataTable dt = rs.giornaliero(comboBox_lista_giorni.SelectedValue.ToString());
                List<Report> reportlist = new List<Report>();
                foreach (DataRow row in dt.Rows)
                {
                    double seconds_oredafare = TimeSpan.Parse(row["ore_da_fare"].ToString()).TotalSeconds;
                    double seconds_orelavorate = TimeSpan.Parse(row["ore_lavorate"].ToString()).TotalSeconds;
                    double seconds_bancaore = seconds_orelavorate - seconds_oredafare;
                    string bancaore = Utilities.ToHMString(TimeSpan.FromSeconds(seconds_bancaore));

                    reportlist.Add(new Report()
                    {
                        report_giorno = row["giorno"].ToString(),
                        report_giorno_dayofweek = row["giorno"].ToString() + " " + Utilities.giorno_della_settimana(row["day_of_week"].ToString()),
                        report_nome = row["nome"].ToString(),
                        report_id_dip = row["id_dipendente"].ToString(),
                        report_orario = row["ore_da_fare"].ToString(),
                        report_entrata = row["entrata"].ToString(),
                        report_uscita = row["uscita"].ToString(),
                        report_note = row["note"].ToString(),
                        report_ore_dentro = row["ore_dentro"].ToString(),
                        report_pausa = row["pausa"].ToString(),
                        report_ore_lavorate = row["ore_lavorate"].ToString(),
                        report_modificato = row["modificato"].ToString(),
                        report_bancaore = bancaore,
                        report_eu_id = row["eu_id"].ToString()
                    });
                }

                tabellagiorno.dataGrid_report_mensile.ItemsSource = reportlist;
            }
        }

        //************************************************************
        //  REPORT MENSILE TUTTI- CERCA
        //************************************************************
        public void button_cerca_mensile_tutti_Click(object sender, RoutedEventArgs e)
        {
            double totale_secondi_banca_ore = 0;
            if (String.IsNullOrEmpty((string)comboBox_lista_mesi_tutti.SelectedValue))
            {
                System.Windows.MessageBox.Show("seleziona mese");
            }
            else
            {
                sql_report rs = new sql_report();

                DataTable dt = rs.mensile_tutti(comboBox_lista_mesi_tutti.SelectedValue.ToString());
                List<Report> reportlist = new List<Report>();

                string nome_precedente = "";
                string nome_corrente = "";
                Boolean sommario = false;
                foreach (DataRow row in dt.Rows)
                {
                    nome_corrente = row["nome"].ToString();
                    if (nome_corrente != nome_precedente && nome_precedente != "")
                    {
                        reportlist.Add(sommarioReport(totale_secondi_banca_ore));
                        totale_secondi_banca_ore = 0;
                    }

                    string ore_lavorate = row["ore_lavorate"].ToString();
                    string ore_dentro = row["ore_dentro"].ToString();
                    double seconds_oredafare = TimeSpan.Parse(row["ore_da_fare"].ToString()).TotalSeconds;
                    double seconds_orelavorate = TimeSpan.Parse(row["ore_lavorate"].ToString()).TotalSeconds;
                    if (row["uscita"].ToString() == "00:00:00")
                    {
                        seconds_orelavorate = 0;
                        ore_lavorate = "NO USCITA";
                        ore_dentro = "No USCITA";
                    }
                    double seconds_bancaore = seconds_orelavorate - seconds_oredafare;
                    totale_secondi_banca_ore = totale_secondi_banca_ore + seconds_bancaore;
                    string bancaore = Utilities.ToHMString(TimeSpan.FromSeconds(seconds_bancaore));

                    reportlist.Add(new Report()
                    {
                        report_giorno = row["giorno"].ToString(),
                        report_giorno_dayofweek = row["giorno"].ToString() + " " + Utilities.giorno_della_settimana(row["day_of_week"].ToString()),
                        report_nome = row["nome"].ToString(),
                        report_id_dip = row["id_dipendente"].ToString(),
                        report_orario = row["ore_da_fare"].ToString(),
                        report_entrata = row["entrata"].ToString(),
                        report_uscita = row["uscita"].ToString(),
                        report_note = row["note"].ToString(),
                        report_ore_dentro = ore_dentro,
                        report_pausa = row["pausa"].ToString(),
                        report_ore_lavorate = ore_lavorate,
                        report_modificato = row["modificato"].ToString(),
                        report_bancaore = bancaore,
                        report_eu_id = row["eu_id"].ToString()
                    });

                    nome_precedente = nome_corrente;
                }
                reportlist.Add(sommarioReport(totale_secondi_banca_ore));

                tabellatutti.dataGrid_report_mensile.ItemsSource = reportlist;
                //label_report_dipname.Text = combo_lista_dipendenti.Text + " " + comboBox_lista_mesi.SelectedValue.ToString();
                //label_report_dipname.SetValue(FrameworkElement.TagProperty, combo_lista_dipendenti.Text); //IMPORTANTE: setto il nome
                //combo_lista_dipendenti.SetValue(FrameworkElement.TagProperty, combo_lista_dipendenti.SelectedValue.ToString()); //IMPORTANTE: setto l'id dipendente nel tag del blocco
                //btn_aggiungi_presenza.IsEnabled = true;
                btn_stampa_tutti.IsEnabled = true;
            }
        }

        private Report sommarioReport(double tot)
        {
            return new Report()
            {
                report_giorno = "---",
                report_giorno_dayofweek = "---",
                report_nome = "---",
                report_id_dip = "---",
                report_orario = "---",
                report_entrata = "---",
                report_uscita = "---",
                report_note = "TOTALE BANCA ORE:",
                report_ore_dentro = "---",
                report_pausa = "---",
                report_ore_lavorate = "---",
                report_modificato = "---",
                report_eu_id = "---",
                report_bancaore = Utilities.ToHMString(TimeSpan.FromSeconds(tot)).Trim()
            };
        }

        //************************************************************
        //  AGGIUNGI PRESENZA MANUALE.
        //************************************************************
        private void add_presenza_click(object sender, RoutedEventArgs e)
        {
            addEntrataUscita aeuw = new addEntrataUscita(combo_lista_dipendenti.Tag.ToString(), label_report_dipname.Tag.ToString());

            aeuw.dp_addpresenza_giorno.Focus();
            if (aeuw.ShowDialog() == true)
            {
                sql_entrate_uscite eusql = new sql_entrate_uscite();
                Log.LogMessageToDb("Admin aggiunta manuale: " + aeuw.addpresenza_nome + " " + aeuw.addpresenza_giorno + " " + aeuw.addpresenza_entrata + " " + aeuw.addpresenza_uscita + " " + aeuw.addpresenza_pausa);
                string r = eusql.add_new_entrata_uscita(aeuw.addpresenza_id, aeuw.addpresenza_nome, aeuw.addpresenza_giorno, aeuw.addpresenza_entrata, aeuw.addpresenza_uscita, aeuw.addpresenza_pausa, aeuw.addpresenza_note);

                button_cerca_mensile_Click(this, new RoutedEventArgs()); // REFRESH DELLA TABELLA REPORT, cercando di nuovo
            }
        }

        //******************** ****************************************
        //  PDF
        //************************************************************
        private void button1_Click(object sender, RoutedEventArgs e) //stampa
        {
            List<Report> reportlist = tabellamensile.dataGrid_report_mensile.ItemsSource as List<Report>;
            string nome = label_report_dipname.Text;
            string totbo = label_tot_bancaore.Content.ToString();

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                Document d = pdf.generaPdf(reportlist, nome, totbo);
                System.Windows.MessageBox.Show("Report pdf salvato in :" + System.IO.Path.Combine(initTable.initFolder(), MyGlobals.folder_report) + "\n\nApri la cartella report per trovarlo.");
                //string ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(d);
                //preview p = new preview();
                //p.migra.Ddl = ddl;
                //p.Show();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                _busyIndicator.IsBusy = false;
            };
            _busyIndicator.IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void pdf_tutti(object sender, RoutedEventArgs e) //stampa
        {
            List<Report> reportlist = tabellatutti.dataGrid_report_mensile.ItemsSource as List<Report>;
            string mese = comboBox_lista_mesi_tutti.SelectedValue.ToString();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                Document d = pdf_all.generaPdf(reportlist, mese);
                System.Windows.MessageBox.Show("REPORT PER TUTTI I DIPENDENTI CREATO CORRETTAMENTE.\n\nApri la cartella report per trovarlo.");
                //use the Dispatcher to delegate the listOfStrings collection back to the UI
                // Dispatcher.Invoke((Action)(() => _listBox.ItemsSource = listOfString));
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                _busyIndicator.IsBusy = false;
            };
            _busyIndicator.IsBusy = true;
            worker.RunWorkerAsync();

            /*
        string ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(d);

        preview p = new preview();
        p.migra.Ddl = ddl;
        p.Show();
    */
        }

        //************************************************************
        //  apri report folder
        //************************************************************
        private void button_Click_aprireportfolder(object sender, RoutedEventArgs e) //apri folder log
        {
            var folder = System.IO.Path.Combine(initTable.initFolder(), MyGlobals.folder_report); //seleziono la cartella log
            Process.Start(@folder); //apro log folder
        }

        //************************************************************
        //  trucco per TAB selection change- per non propagare gli event dai combo e dalle sotto-tab
        //************************************************************

        private void combo_lista_dipendenti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void comboBox_lista_mesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        //************************************************************
        //  menu aggiorna
        //************************************************************
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)  // aggiorna
        {
            aggiorna();
        }

        public void aggiorna()
        {
            griglia_entrate();
            mainTab.SelectedIndex = 0;
        }

        //************************************************************
        // IMPORTA DATABASE
        //************************************************************
        private void importaDb(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "sqlite files (*.sqlite)|*.sqlite";
            dialog.Title = "Select a sqlite file";
            if (dialog.ShowDialog() == true)
            {
                string nomedb = Properties.Settings.Default.nomeDB;
                var destfile = Path.Combine(initTable.pathDatabase(), nomedb);

                string fname = dialog.FileName;

                try
                {
                    System.IO.File.Copy(fname, destfile, true);
                    Properties.Settings.Default.nomeDBAperto = MyGlobals.default_db_aperto;
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Il database di default è stato sovrascritto correttamente da quello che hai scelto");
                    Log.LogMessageToDb("-*- database di default SOVRASCRITTO da: " + fname);
                }
                catch (IOException e1)
                {
                    MessageBox.Show("Si è verificato un errore: " + e1.Message);
                }

                refresh_db();
            }
        }

        //************************************************************
        // APRI DATABASE
        //************************************************************
        private void apriDb(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "sqlite files (*.sqlite)|*.sqlite|All files (*.*)|*.*";
            string dir = initTable.pathDatabase();
            dialog.InitialDirectory = dir;
            dialog.Title = "Select a sqlite file";
            if (dialog.ShowDialog() == true)
            {
                string fname = dialog.FileName;
                Properties.Settings.Default.nomeDBAperto = fname;
                Properties.Settings.Default.Save();

                refresh_db();
            }
        }

        //************************************************************
        // DEFAULT DATABASE
        //************************************************************
        private void defaultDb(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.nomeDBAperto = MyGlobals.default_db_aperto;
            Properties.Settings.Default.Save();
            MessageBox.Show("Stai usando di nuovo il database di default, quello buono!");
            Log.LogMessageToDb("-*- database restored.");

            refresh_db();
        }

        //************************************************************
        // BACKUP DATABASE
        //************************************************************
        private void backupDb(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "sqlite (*.sqlite)|*.sqlite";
            if (saveFileDialog.ShowDialog() == true)
            {
                string sourceFile = initTable.path_folder();

                try
                {
                    System.IO.File.Copy(sourceFile, saveFileDialog.FileName, true);
                    System.Windows.MessageBox.Show("Backup eseguito con successo");
                    Log.LogMessageToDb("-*- backup manuale eseguito.");
                }
                catch
                {
                    throw;
                }
            }
        }

        #region scheduler

        //*******************************************************
        // backup scheduler
        //***************************************************
        // private CancellationTokenSource m_ctSource;
        private void backup_scheduler_Db(object sender, RoutedEventArgs e)
        {
            function_backup_scheduler_db();
        }

        private void function_backup_scheduler_db()
        {
            // int hour = 4;
            //int minutes = 0;
            string backupFile = "";

            if (Properties.Settings.Default.backupAuto == false)
            {
                //allora lo devo attivare
                Properties.Settings.Default.backupAuto = true;

                if (Properties.Settings.Default.backupFile == MyGlobals.backup_auto_file)//se uguale a no
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog(); //scelgo il file del backup
                    saveFileDialog.Filter = "sqlite (*.sqlite)|*.sqlite";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        backupFile = saveFileDialog.FileName;
                        Properties.Settings.Default.backupFile = backupFile;
                    }
                }
                else
                {
                    backupFile = Properties.Settings.Default.backupFile; //altrimenti lo prendo da quello che ho già
                }

                Properties.Settings.Default.Save();

                string sourceFile = initTable.path_folder();

                scheduler_Database.Header = MyGlobals.menu_disattiva_backup;
                label_db_backup.Content = "Backup automatico attivo su: " + Properties.Settings.Default.backupFile;

                sched.DeleteJob(new JobKey("myJob"));
                IJobDetail job = JobBuilder.Create<job_bk>()
                 .WithIdentity("myJob")
                 .UsingJobData("savefile", backupFile)
                 .UsingJobData("sourcefile", sourceFile)
                 .Build();

                sched.ScheduleJob(job, trigger);

                Log.LogMessageToDb("-*- backup scheduler partito.");
            }
            else//è attivo, lo disattivo
            {
                Debug.WriteLine("provo a disattivare scheduler");
                Properties.Settings.Default.backupAuto = false;
                Properties.Settings.Default.backupFile = MyGlobals.backup_auto_file; //no
                Properties.Settings.Default.Save();

                scheduler_Database.Header = MyGlobals.menu_attiva_backup;
                label_db_backup.Content = "Backup automatico non attivo";
                Log.LogMessageToDb("-*- backup scheduler stoppato.");
                //sched.Standby();
                sched.PauseTrigger(new TriggerKey("aaa"));
            }
        }

        #endregion scheduler

        //************************************************************
        //  menu help
        //************************************************************
        private void MenuItem_hi(object sender, RoutedEventArgs e)  // aggiorna
        {
            Help_iniziale hi = new Help_iniziale();
            hi.Show();
        }

        private void MenuItem_h(object sender, RoutedEventArgs e)  // aggiorna
        {
            Help h = new Help();
            h.Show();
        }

        //*******************************************************
        // close
        //***************************************************
        private void Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("shutdown");
            Log.LogMessageToDb("-*- ------------------------- APP STOP ----------------");
            sched.Shutdown();
        }

        private void img_news_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            codePopup = new Popup();
            WrapPanel wp = new WrapPanel();
            wp.Orientation = System.Windows.Controls.Orientation.Vertical;

            TextBlock popupText1 = new TextBlock();
            popupText1.Text = "Clicca ovunque fuori per chiudermi";
            popupText1.Background = Brushes.Black;
            popupText1.Foreground = Brushes.White;

            TextBlock popupText = new TextBlock();
            popupText.Padding = new Thickness(10, 10, 10, 10);
            popupText.Margin = new Thickness(2, 2, 2, 2);
            popupText.FontSize = 35;
            popupText.MaxWidth = 800;
            popupText.TextWrapping = TextWrapping.Wrap;

            popupText.Text = Properties.Settings.Default.news;

            popupText.Background = Brushes.White;
            popupText.Foreground = Brushes.Chocolate;

            wp.Children.Add(popupText);
            wp.Children.Add(popupText1);
            codePopup.Child = wp;
            codePopup.PlacementTarget = this;

            codePopup.Placement = PlacementMode.Center;
            codePopup.StaysOpen = false;
            codePopup.IsOpen = true;
        }

        private void img_news_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)

        {
            codePopup = null;
        }
    }//fine class

    public class orariDipendentiCombo
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class listaDipendentiCombo
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class listaMesiCombo
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class listaGiorniCombo
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}