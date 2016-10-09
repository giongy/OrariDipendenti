using System;
using System.Diagnostics;
using System.IO;

namespace OrariDipendenti
{
    internal static class Log
    {
        public static string GetTempPath()
        {
            string path = initTable.pathLog();
            if (!path.EndsWith("\\")) path += "\\";
            Debug.WriteLine(path);
            return path;
        }

        public static void LogMessageToFile(string msg)
        {
            string oggi = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");
            var fileInfo = new FileInfo(GetTempPath() + MyGlobals.log_file + ".txt");
            if (fileInfo.Exists)
            {
                Debug.WriteLine("file size: " + fileInfo.Length);
                if (fileInfo.Length > MyGlobals.log_size)
                {
                    System.IO.File.Move(GetTempPath() + MyGlobals.log_file + ".txt", GetTempPath() + MyGlobals.log_file + " " + oggi + ".txt");
                }
            }

            System.IO.StreamWriter sw = System.IO.File.AppendText(GetTempPath() + MyGlobals.log_file + ".txt");

            try
            {
                string logLine = System.String.Format("{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }

        public static void LogMessageToDb(string msg)
        {
            sql_log sl = new sql_log();
            string logLine = System.String.Format("{0:G}: {1}.", System.DateTime.Now, msg);
            string r = sl.add_record(logLine);
        }
    }
}