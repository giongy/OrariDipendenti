using System; using System.Collections.Generic;

namespace OrariDipendenti
{
    public static class Utilities
    {
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public static string giorno_della_settimana(string value)
        {
            // Begin the switch.
            switch (value)
            {
                case "0":
                    return "Domenica";

                case "1":
                    return "Lunedì";

                case "2":
                    return "Martedì";

                case "3":
                    return "Mercoledì";

                case "4":
                    return "Giovedì";

                case "5":
                    return "Venerdì";

                case "6":
                    return "Sabato";

                default:
                    // You can use the default case.
                    return "boh";
            }
        }

        public static string ToHMString(TimeSpan timespan)
        {
            if (timespan.Ticks < 0) return "-" + ToHMString(timespan.Negate());

            return Math.Floor(timespan.TotalHours).ToString("#0") + ":" + timespan.Minutes.ToString("00");
        }
    }
}