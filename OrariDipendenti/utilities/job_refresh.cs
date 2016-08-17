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
            }));

            Log.LogMessageToFile("-*- eseguito refresh nuovo giorno");
        }
    }
}