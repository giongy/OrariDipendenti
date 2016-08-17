using System;
using System.ComponentModel;

namespace OrariDipendenti
{
    public class esci : IDataErrorInfo
    {
        public DateTime uscita { get; set; }
        public DateTime pausa { get; set; }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == "uscita")
                {
                    if (uscita == null)
                        result = "devi inserire un orario valido (hh:mm)";
                }
                if (columnName == "pausa")
                {
                    if (pausa == null)
                        result = "devi inserire un orario valido (hh:mm)";
                }

                return result;
            }
        }

        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}