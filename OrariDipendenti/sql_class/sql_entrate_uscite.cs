using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading;

namespace OrariDipendenti
{
    internal class sql_entrate_uscite
    {
        public string add_new_entrata(string ora, string id, string nomecompleto)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    string oggi = System.DateTime.Today.ToString("yyyy-MM-dd");
                    string ore_giornaliere = check_orario_dipendente(id); //mi prendo le sue ore giornaliere
                    Debug.WriteLine(oggi + " " + ora);
                    int esiste = check_entrata(oggi, id);

                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    if (esiste == 0) //NON si è ancora entrati
                    {
                        Debug.WriteLine("non sono ancora entrato oggi");
                        var dic = new Dictionary<string, object>();
                        dic["id_dipendente"] = id;
                        dic["nome_dipendente"] = nomecompleto;
                        dic["giorno"] = oggi;
                        dic["ore_da_fare"] = ore_giornaliere;
                        dic["entrata"] = ora;

                        try
                        {
                            Debug.WriteLine("*************************************** " + Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator + " " + ora);
                            sh.Insert("entrate_uscite", dic);
                            return MyGlobals.ok;
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
                    else //se sono già entrato oggi
                    {
                        Debug.WriteLine("sono già entrato ma voglio cambiare, id riga:" + esiste);
                        var dic = new Dictionary<string, object>();

                        dic["entrata"] = ora;

                        try
                        {
                            sh.Update("entrate_uscite", dic, "id", esiste);
                            return MyGlobals.ok;
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
        }

        public string check_orario_dipendente(string id)

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
                        //cerco se per il giorno corrente e dipendente c'è già un'entrata
                        string sql = "SELECT time(ore_giornaliere) as ore from orari " +
                                        "where id = (select id_orario from dipendenti where dipendenti.id = " + id + ")";

                        DataTable dt = sh.Select(sql);

                        string temp = dt.Rows[0]["ore"].ToString();

                        return temp; //ritorno numero di righe trovate
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

        public int check_entrata(string giorno, string dip)

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
                        //cerco se per il giorno corrente e dipendente c'è già un'entrata
                        string sql = "select id from entrate_uscite where id_dipendente=" + dip + " and giorno='" + giorno + "'";

                        DataTable dt = sh.Select(sql);

                        int num = dt.Rows.Count;
                        System.Diagnostics.Debug.WriteLine("check già entrata: " + num);
                        if (num > 0) //se num maggiore di 0 allora prendo id
                        {
                            System.Diagnostics.Debug.WriteLine(dt.Rows[0]["id"].GetType());

                            string temp = dt.Rows[0]["id"].ToString();
                            num = Int32.Parse(temp);
                        }
                        return num; //ritorno numero di righe trovate
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

        public Hashtable check_uscita_precedente(string dip)

        {
            Hashtable h = new Hashtable();
            string check = "true";
            string oggi = System.DateTime.Today.ToString("yyyy-MM-dd");
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    try
                    {
                        Debug.WriteLine("dipp " + dip);
                        string sql1 = "select id,time(entrata)as e,time(uscita) as u,date(giorno) as g,note,nome_dipendente from entrate_uscite " +
                                        "where id_dipendente=" + dip + " and date(giorno) != '" + oggi + "' order by g DESC limit 1";
                        Debug.WriteLine(sql1);
                        DataTable dt1 = sh.Select(sql1);
                        foreach (DataRow row in dt1.Rows)
                        {
                            if (row["u"].ToString() == "00:00:00")
                            {
                                check = MyGlobals.exitmissed;
                            }
                            Debug.WriteLine("check ultima uscita: " + row["u"].ToString() + " " + row["g"].ToString() + " " + check);
                            h.Add("check", check);
                            h.Add("giorno_da_sistemare", row["g"].ToString());
                            h.Add("entrata", row["e"].ToString());
                            h.Add("note", row["note"].ToString());
                            h.Add("nome", row["nome_dipendente"].ToString());
                            h.Add("id", row["id"].ToString());
                        }
                        return h;
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

        public System.Collections.Hashtable check_oggi(string dip)

        {
            string alreadyenter = "false";
            Hashtable check = new Hashtable();
            string oggi = System.DateTime.Today.ToString("yyyy-MM-dd");
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    try
                    {
                        Debug.WriteLine("dip checkoggi " + dip);
                        string sql1 = "select id,nome_dipendente,time(entrata) as e,time(uscita) as u,time(pausa) as p,time(ore_da_fare) as odf, note from entrate_uscite where id_dipendente=" + dip + " and giorno ='" + oggi + "'";
                        DataTable dt1 = sh.Select(sql1);
                        if (dt1.Rows.Count > 0) //se trovi righe allora sei già entrato e puoi uscire
                        {
                            alreadyenter = "true";
                            foreach (DataRow row in dt1.Rows)
                            {
                                check.Add("id", row["id"].ToString());
                                check.Add("nome_dipendente", row["nome_dipendente"].ToString());
                                check.Add("entrata", row["e"].ToString());
                                check.Add("uscita", row["u"].ToString());
                                check.Add("pausa", row["p"].ToString());
                                check.Add("ore_da_fare", row["odf"].ToString());
                                check.Add("note", row["note"].ToString());
                                check.Add("alreadyenter", alreadyenter);
                            }
                            return check;
                        }
                        else
                        {
                            alreadyenter = "false";
                            check.Add("alreadyenter", alreadyenter);
                            return check;
                        }
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

        public string add_new_uscita(string uscita, string pausa, string id, string note)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    var dic = new Dictionary<string, object>();

                    dic["uscita"] = uscita;
                    dic["pausa"] = pausa;
                    dic["note"] = note;

                    try
                    {
                        sh.Update("entrate_uscite", dic, "id", id);
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

        public string edit_entrata_uscita(string id, string orario, string entrata, string uscita, string pausa, string note)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    var dic = new Dictionary<string, object>();

                    dic["ore_da_fare"] = orario;
                    dic["entrata"] = entrata;
                    dic["uscita"] = uscita;
                    dic["pausa"] = pausa;
                    dic["note"] = note;
                    dic["modificato"] = "SI";

                    try
                    {
                        sh.Update("entrate_uscite", dic, "id", id);
                        return MyGlobals.ok;
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

        public string add_new_entrata_uscita(string id_dip, string nome_dip, string giorno, string entrata, string uscita, string pausa, string note)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    string ore_giornaliere = check_orario_dipendente(id_dip); //mi prendo le sue ore giornaliere
                    Debug.WriteLine("nuova entrata");
                    var dic = new Dictionary<string, object>();
                    dic["id_dipendente"] = id_dip;
                    dic["nome_dipendente"] = nome_dip;
                    dic["giorno"] = giorno;
                    dic["ore_da_fare"] = ore_giornaliere;
                    dic["entrata"] = entrata;
                    dic["uscita"] = uscita;
                    dic["pausa"] = pausa;
                    dic["note"] = note;

                    try
                    {
                        sh.Insert("entrate_uscite", dic);
                        return MyGlobals.ok;
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

        public string modify_note(string note, string id)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source=" + initTable.path()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    var dic = new Dictionary<string, object>();

                    dic["note"] = note;

                    try
                    {
                        sh.Update("entrate_uscite", dic, "id", id);
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

        public void delete_entrata_uscita(string id)
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
                        sh.Execute("delete from entrate_uscite where id='" + id + "'");

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