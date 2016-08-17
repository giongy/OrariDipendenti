using System.Globalization;
using System.Threading;
using System.Windows;

namespace OrariDipendenti

{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter { Level = Common.Logging.LogLevel.Info };
            CultureInfo ci = new CultureInfo("it-IT");
            var dtfInfo = new DateTimeFormatInfo
            {
                ShortDatePattern = "yyyy-MM-dd",
                ShortTimePattern = "HH:mm",
                DateSeparator = "-",
                TimeSeparator = ":"
            };
            ci.DateTimeFormat = dtfInfo;

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            FrameworkElement.LanguageProperty.OverrideMetadata(
                        typeof(FrameworkElement),
                        new FrameworkPropertyMetadata(
                        System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag)));

            // Create the startup window
            // base.OnStartup(e);
            MainWindow wnd = new MainWindow();
            // Do stuff here, e.g. to the window
            // Step 4 - Make sure that the splash screen lasts at least two seconds

            //wnd.Title = "Something else";
            // Show the window
            wnd.Show();
        }
    }
}