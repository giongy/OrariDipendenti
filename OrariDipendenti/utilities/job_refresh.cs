using Quartz;
using System;
using System.Diagnostics;

namespace OrariDipendenti
{
    public class job_refresh : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("-----------------------------------------------------------------------refresh ");
            MainWindow.MyWindow.Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.MyWindow.aggiorna();
                sql_log sl = new sql_log();
                sl.delete_log(); //delete old log, keep last 1000
                sl.vacuum(); //vaacum database
            }));

            Log.LogMessageToDb("-*- eseguito refresh nuovo giorno");
        }
    }
}