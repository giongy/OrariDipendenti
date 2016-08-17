using Quartz;
using System;
using System.Diagnostics;

namespace OrariDipendenti
{
    public class job_bk : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            string savefile = dataMap.GetString("savefile");
            string sourcefile = dataMap.GetString("sourcefile");
            /*
            Common.Logging.LogManager.Adapter.GetLogger("LoggingJob").Info(
                string.Format("Logging job : {0} {1}, and proceeding to log",
                    DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    */
            Debug.WriteLine("backup *************************: " + savefile + " ------------------------------------------");
            System.IO.File.Copy(sourcefile, savefile, true);

            var next = context.NextFireTimeUtc;
            Log.LogMessageToFile("-*- backup automatico eseguito. il prossimo sarà: " + TimeZone.CurrentTimeZone.ToLocalTime(next.Value.DateTime));
            Debug.WriteLine("prossimo job_bk: " + TimeZone.CurrentTimeZone.ToLocalTime(next.Value.DateTime));
        }
    }
}