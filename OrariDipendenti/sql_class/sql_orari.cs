using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;

namespace OrariDipendenti
{
    internal class sql_orari
    {
        public string add_new_orario(string nome, double ore)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    var minuti = ore * 12;
                    Debug.WriteLine("minuti " + minuti);
                    TimeSpan ts = TimeSpan.FromMinutes(minuti);
                    Debug.WriteLine("ticks: " + new DateTime(ts.Ticks));
                    string formatted = string.Format(new DateTime(ts.Ticks).ToString("HH:mm"));
                    Log.LogMessageToDb("hai provato ad inserire l'orario " + nome + ". Ore giornaliere: " + formatted);
                    System.Diagnostics.Debug.WriteLine(formatted);

                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    var dic = new Dictionary<string, object>();
                    dic["nome"] = nome;
                    dic["ore_settimanali_contratto"] = ore;
                    dic["ore_giornaliere"] = formatted;

                    try
                    {
                        sh.Insert("orari", dic);
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

        public DataTable select_orario()
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
                        DataTable dt = sh.Select("select id,nome,ore_settimanali_contratto,time(ore_giornaliere) as ore_giornaliere from orari;");
                        return dt;
                    }
                    catch (SQLiteException e)
                    {
                        conn.Close();
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public string edit_orario(string nome, double ore, int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    TimeSpan ts = TimeSpan.FromMinutes(ore * 12);
                    string formatted = string.Format(new DateTime(ts.Ticks).ToString("HH:mm"));
                    System.Diagnostics.Debug.WriteLine(formatted);

                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    var dic = new Dictionary<string, object>();
                    dic["nome"] = nome;
                    dic["ore_settimanali_contratto"] = ore;
                    dic["ore_giornaliere"] = formatted;

                    try
                    {
                        sh.Update("orari", dic, "id", id);
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

        public string delete_orario(int id)
        {
            string result = "OK";
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
                        sh.Execute("delete from orari where id='" + id + "'");
                        // INSERT.....
                        // INSERT.....
                        // UPDATE....
                        // ... skip for another 50,000 queries....
                        // DELETE....
                        // UPDATE...
                        // INSERT.....

                        sh.Commit();
                        return result;
                    }
                    catch (SQLiteException e)
                    {
                        sh.Rollback();
                        Console.Write(e.ResultCode + " " + e.Message + " " + e.ErrorCode);
                        result = e.Message;
                        return result;
                    }
                    finally { conn.Close(); }
                }
            }
        }
    }
}