using System;
using System.ComponentModel;
using System.Diagnostics;

namespace OrariDipendenti
{
    public class entra : IDataErrorInfo
    {
        public DateTime entrata { get; set; }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == "entrata")
                {
                    Debug.WriteLine("entrataaaaaa: " + entrata);
                    if (entrata == null)
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