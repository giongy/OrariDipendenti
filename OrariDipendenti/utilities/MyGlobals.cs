using MahApps.Metro.Controls.Dialogs;

namespace OrariDipendenti
{
    public static class MyGlobals
    {
        public const string approvazione_minore = "NN"; // NON NECESSITA - meno ore
        public const string approvazione_rifiutato = "NO"; // NON APPROVATO - ingiustificato
        public const string approvazione_approvato = "SI"; // APPROVATO dalla coordinatrice
        public const string approvazione_da_verificare = "NV"; // NON VERIFICATO dalla coordinatrice
        public const string exitmissed = "exitmissed"; // cannot change
        public const string alreadyexit = "alreadyexit"; // cannot change

        public const string dip_tutti = "ALL"; // cannot change
        public const string dip_inservizio = "INSERVIZIO"; // cannot change

        public const string ok = "ok"; // cannot change
        public const string log_file = "orariDipendenti_log";
        public const string folder_report = "report_pdf";
        public const string folder_log = "log";
        public const string folder_db = "database";
        public const int log_size = 50000; // cannot change
        public const string initial_password = "0";
        public const string default_db_aperto = "default";
        private static bool admin = false;
        public static string menu_attiva_backup = "Attiva Backup automatico";
        public static string menu_disattiva_backup = "Disattiva Backup automatico";
        public static string backup_auto_file = "no";

        public static uint color_bottom = 0xFFE7F3F7;

        public static bool Admin
        {
            get
            {
                return admin;
            }

            set
            {
                admin = value;
            }
        }

        public static string exitmissednote(string giorno)
        {
            return "Attenzione: Il giorno " + giorno + " hai dimenticato di segnare l'uscita. Fallo adesso , poi potrai di nuovo entrare.";
        }

        public static string alert_db()
        {
            return "Ora stai usando il database: "
                      + Properties.Settings.Default.nomeDBAperto + " \r\nFAI ATTENZIONE, questo non è il database di lavoro, ma una copia personale o un backup. " +
                      "\r\nPer tornare ad usare il database di default usa la funzione -Ritorna al database di default-";
        }

        public static MetroDialogSettings myMetroSettings()
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Va bene.",
                NegativeButtonText = "Annulla",
                FirstAuxiliaryButtonText = "Cancel",
                AnimateShow = false,
                AnimateHide = false,
                 DialogTitleFontSize=45,
                DialogMessageFontSize=30 ,
                ColorScheme = MetroDialogColorScheme.Accented
            };
            return mySettings;
        }
        public static MetroDialogSettings myMetroSettings2()
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ho capito, voglio importare i dati",
                NegativeButtonText = "Annulla, meglio fare prima un backup",
                FirstAuxiliaryButtonText = "Cancel",
                AnimateShow = false,
                AnimateHide = false,
                DialogTitleFontSize = 45,
                DialogMessageFontSize = 30,
                ColorScheme = MetroDialogColorScheme.Accented
            };
            return mySettings;
        }

        public static MetroDialogSettings myMetroSettingsSmall()
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Va bene.",
                NegativeButtonText = "Go away!",
                FirstAuxiliaryButtonText = "Cancel",
                AnimateShow = false,
                AnimateHide = false,
                DialogTitleFontSize = 30,
                DialogMessageFontSize = 20,
                ColorScheme = MetroDialogColorScheme.Accented
            };
            return mySettings;
        }
    }
}