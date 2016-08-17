/*
 * Created by SharpDevelop.
 * User: ITL91012
 * Date: 30/03/2016
 * Time: 13:54
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace OrariDipendenti
{
    /// <summary>
    /// Description of initTable.
    /// </summary>
    public class initTable
    {
        public initTable()
        {
        }

        public static string initFolder()
        {
            string folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string pathString = System.IO.Path.Combine(folderName, "OrariDipendenti");
            System.IO.Directory.CreateDirectory(pathString);
            return pathString;
        }

        public static string pathPdf()
        {
            string pathString_report = System.IO.Path.Combine(initFolder(), MyGlobals.folder_report);
            return pathString_report;
        }

        public static string pathDatabase()
        {
            string pathString_database = System.IO.Path.Combine(initFolder(), MyGlobals.folder_db);
            return pathString_database;
        }

        public static string pathLog()
        {
            string pathString_log = System.IO.Path.Combine(initFolder(), MyGlobals.folder_log);
            return pathString_log;
        }

        public static string path()
        {
            return path_folder() + ";foreign keys=true;";
        }

        public static string path_folder()
        {
            var fileName = "";
            string nomedb = Properties.Settings.Default.nomeDB;
            if (Properties.Settings.Default.nomeDBAperto == MyGlobals.default_db_aperto)
            {
                Debug.WriteLine("APRO DEFAULT");
                fileName = Path.Combine(pathDatabase(), nomedb);
            }
            else
            {
                Debug.WriteLine("APRO altro");
                fileName = Properties.Settings.Default.nomeDBAperto;
            }

            return fileName;
        }

        public void init()
        {
            try
            {
                System.IO.Directory.CreateDirectory(pathDatabase());
                System.IO.Directory.CreateDirectory(pathLog());
                System.IO.Directory.CreateDirectory(pathPdf());
                //MessageBox.Show(path());

                SQLiteConnection conn = new SQLiteConnection("Data Source=" + path() + ";Version=3;");
                conn.Open();

                string create_entrate_uscite = "CREATE TABLE IF NOT EXISTS entrate_uscite ( " +
                    "id            INTEGER        PRIMARY KEY AUTOINCREMENT " +
                    "                            UNIQUE " +
                    "                             NOT NULL, " +
                    "id_dipendente INTEGER        REFERENCES dipendenti (id) ON DELETE CASCADE " +
                    "                                                        ON UPDATE CASCADE " +
                    "                             NOT NULL, " +
                    "nome_dipendente VARCHAR (50), " +
                    "giorno        DATE           NOT NULL, " +
                    "ore_da_fare TIME, " +
                    "entrata       TIME           NOT NULL, " +
                    "uscita        TIME           DEFAULT ('00:00'), " +
                    "pausa         TIME           DEFAULT ('00:00'), " +
                    "note          VARCHAR (1000),  " +
                    "modificato    VARCHAR(2)   NOT NULL DEFAULT NO" +
                    ");";

                string create_orari = "CREATE TABLE IF NOT EXISTS orari ( " +
                    "id                        INTEGER      PRIMARY KEY AUTOINCREMENT " +
                    "                                      NOT NULL, " +
                    "nome                      VARCHAR (30), " +
                    "ore_settimanali_contratto INTEGER      NOT NULL, " +
                    "ore_giornaliere           TIME         NOT NULL " +
                    "                                       DEFAULT ('00:00')  " +
                    ");	 ";
                string create_dipendenti = "CREATE TABLE IF NOT EXISTS dipendenti ( " +
                    " id          INTEGER        PRIMARY KEY AUTOINCREMENT, " +
                    "nome        VARCHAR (50)   NOT NULL, " +
                    "cognome     VARCHAR (50)   NOT NULL, " +
                    " note        VARCHAR (1000) DEFAULT NULL, " +
                    " in_servizio VARCHAR (2)    NOT NULL " +
                    "                           DEFAULT SI, " +
                    "id_orario   INTEGER (2)    NOT NULL " +
                    "                            DEFAULT 0 " +
                    "                            REFERENCES orari (id)  " +
                    ");";

                string create_view_report1 = "CREATE VIEW IF NOT EXISTS report1 AS " +
                    "SELECT entrate_uscite.id as eu_id, " +
                    "strftime('%w', date(giorno)) as day_of_week, " +
                    "date(giorno)  as giorno,  " +
                    "id_dipendente, " +
                    "nome_dipendente AS nome,  " +
                    "time(entrata) AS entrata, " +
                    "time(uscita) AS uscita, " +
                    "entrate_uscite.note," +
                    "time(strftime('%s', time(uscita)) - strftime('%s', time(entrata)), 'unixepoch') AS ore_dentro, " +
                    "time(pausa) AS pausa, " +
                    "time(strftime('%s', time(uscita)) - strftime('%s', time(entrata)) - strftime('%s', time(pausa)), 'unixepoch')  AS ore_lavorate, " +
                    "time(ore_da_fare) AS ore_da_fare, " +
                    "modificato " +
                    "FROM entrate_uscite; ";

                string create_ix1 = "CREATE UNIQUE INDEX IF NOT EXISTS eu_ix1 ON entrate_uscite (id_dipendente, giorno)";

                SQLiteCommand commandCreate = new SQLiteCommand(create_entrate_uscite, conn);
                commandCreate.ExecuteNonQuery();
                commandCreate = new SQLiteCommand(create_orari, conn);
                commandCreate.ExecuteNonQuery();
                commandCreate = new SQLiteCommand(create_dipendenti, conn);
                commandCreate.ExecuteNonQuery();

                commandCreate = new SQLiteCommand(create_view_report1, conn);
                commandCreate.ExecuteNonQuery();
                commandCreate = new SQLiteCommand(create_ix1, conn);
                commandCreate.ExecuteNonQuery();

                conn.Close();
                Debug.WriteLine("conn chiusa");
            }
            catch (Exception)
            {
            }
        }
    }
}