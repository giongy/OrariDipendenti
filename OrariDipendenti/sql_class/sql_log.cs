using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;

namespace OrariDipendenti
{
    internal class sql_log
    {
        public string add_record(string record)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    var dic = new Dictionary<string, object>();
                    dic["log_entry"] = record;

                    try
                    {
                        sh.Insert("log", dic);
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

        public DataTable select_log()
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
                        string sql = "SELECT log_entry,log_type FROM log;  ";
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

        public void delete_log()
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
                        Log.LogMessageToDb("-*- provo a cancellare vecchi log ");
                        sh.Execute("delete from log where id not in (select id from log order by id desc limit 10)");

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

        public void vacuum()
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
                        Log.LogMessageToDb("-*- provo VAACUM db");
                        sh.Execute("VACUUM");
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