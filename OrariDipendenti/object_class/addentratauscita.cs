using System;
using System.ComponentModel;
using System.Diagnostics;

namespace OrariDipendenti
{
    public class addentratauscita : IDataErrorInfo
    {
        public string entrata { get; set; }

        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? giorno { get; set; }
        public string note { get; set; }
        public string pausa { get; set; }
        public string uscita { get; set; }

        public string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "entrata")
                {
                    Debug.WriteLine("entrata " + entrata);
                    if (string.IsNullOrEmpty(entrata) || entrata.Contains("_"))
                    {
                        result = "serve un orario di entrata";
                    }
                }
                if (columnName == "uscita")
                {
                    if (string.IsNullOrEmpty(uscita) || uscita.Contains("_"))
                    {
                        result = "serve un orario di uscita";
                    }
                }
                if (columnName == "pausa")
                {
                    if (string.IsNullOrEmpty(pausa) || pausa.Contains("_"))
                    {
                        result = "serve un orario di pausa";
                    }
                }
                if (columnName == "giorno")
                {
                    Debug.WriteLine(giorno);
                    if (giorno == null)
                    {
                        result = "serve inserire un giorno";
                    }
                }
                return result;
            }
        }
    }
}