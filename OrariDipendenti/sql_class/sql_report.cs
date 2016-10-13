using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;

namespace OrariDipendenti
{
    internal class sql_report
    {
        #region combo

        public DataTable lista_dipendenti(string chi)
        {
            string where_in_servizio = ""; // normalmente where vuota, prendo tutti
            if (chi == MyGlobals.dip_inservizio) // se passo IN_SERVIZIO allora
            {
                where_in_servizio = "where in_servizio='SI'"; //cambio la where
            }
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    try
                    {
                        DataTable dt = sh.Select("SELECT * from dipendenti " + where_in_servizio);
                        return dt;
                    }
                    catch (SQLiteException e)
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public DataTable lista_mesi()
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    try
                    {
                        DataTable dt = sh.Select("select distinct  strftime('%m', giorno) as mese, strftime('%Y', giorno) as anno from entrate_uscite order by giorno desc limit 36; ");
                        return dt;
                    }
                    catch (SQLiteException e)
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public DataTable lista_giorni()
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    try
                    {
                        DataTable dt = sh.Select("select distinct strftime('%d', giorno) as day, strftime('%m', giorno) as mese, strftime('%Y', giorno) as anno from entrate_uscite order by giorno desc limit 60; ");
                        return dt;
                    }
                    catch (SQLiteException e)
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        #endregion combo

        public string add_presenza(string nome, string cognome, string note, string inServizio, string orario)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    var dic = new Dictionary<string, object>();
                    dic["nome"] = nome;
                    dic["cognome"] = cognome;
                    dic["note"] = note;
                    dic["in_servizio"] = inServizio;
                    dic["id_orario"] = orario;

                    try
                    {
                        sh.Insert("dipendenti", dic);
                        return "ok";
                    }
                    catch (SQLiteException e)
                    {
                        Console.Write(e.ResultCode + " " + e.Message);
                        return e.Message;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public DataTable mensile(string dipendente, string mese)
        {
            string[] subStrings = mese.Split('-');
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    try
                    {
                        string sql = "SELECT * FROM report1 WHERE strftime('%m', giorno) = '" + subStrings[0] + "' and strftime('%Y', giorno) = '" + subStrings[1] + "' and id_dipendente = '" + dipendente + "'";
                        Debug.WriteLine(sql);
                        DataTable dt = sh.Select(sql);
                        return dt;
                    }
                    catch (SQLiteException e)
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public DataTable giornaliero(string giorno)
        {
            string[] subStrings = giorno.Split('-');
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    try
                    {
                        string sql = "SELECT * FROM report1 WHERE " +
                                    " strftime('%d', giorno) = '" + subStrings[0] +
                                    "' and strftime('%m', giorno) = '" + subStrings[1] +
                                    "' and strftime('%Y', giorno) = '" + subStrings[2] + "' ";
                        Debug.WriteLine(sql);
                        DataTable dt = sh.Select(sql);
                        return dt;
                    }
                    catch (SQLiteException e)
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public DataTable mensile_tutti(string mese)
        {
            string[] subStrings = mese.Split('-');
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    try
                    {
                        string sql = "SELECT * FROM report1 WHERE strftime('%m', giorno) = '" + subStrings[0] + "' and strftime('%Y', giorno) = '" + subStrings[1] + "' ORDER BY nome,giorno";
                        Debug.WriteLine(sql);
                        DataTable dt = sh.Select(sql);
                        return dt;
                    }
                    catch (SQLiteException e)
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}