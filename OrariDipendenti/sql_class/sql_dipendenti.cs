using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;

namespace OrariDipendenti
{
    internal class sql_dipendenti
    {
        public string add_new_dipendente(string nome, string cognome, string note, string inServizio, string orario)
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

        public DataTable select_dipendenti(string chi)
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
                        string sql = "SELECT dipendenti.id,dipendenti.nome,cognome,note,in_servizio,orari.nome as orario FROM dipendenti  JOIN orari ON dipendenti.id_orario = orari.id " + where_in_servizio + "; ";
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

        public string edit_dipendente(string nome, string cognome, string note, string inServizio, string orario, int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    Console.Write("dsadsa " + orario);
                    var dic = new Dictionary<string, object>();
                    dic["nome"] = nome;
                    dic["cognome"] = cognome;
                    dic["note"] = note;
                    dic["in_servizio"] = inServizio;
                    dic["id_orario"] = orario;

                    try
                    {
                        sh.Update("dipendenti", dic, "id", id);
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

        public void delete_dipendente(int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    sh.BeginTransaction();

                    try
                    {
                        sh.Execute("delete from dipendenti where id='" + id + "'");
                        // INSERT.....
                        // INSERT.....
                        // UPDATE....
                        // ... skip for another 50,000 queries....
                        // DELETE....
                        // UPDATE...
                        // INSERT.....

                        sh.Commit();
                    }
                    catch
                    {
                        sh.Rollback();
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