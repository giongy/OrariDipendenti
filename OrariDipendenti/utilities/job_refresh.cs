using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OrariDipendenti
{
    public class job_refresh : IJob
    {
        

        public async Task Execute(IJobExecutionContext context)
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