using System;
using System.ComponentModel;

namespace OrariDipendenti
{
    public class dipendente : IDataErrorInfo
    {
        public int id_dipendente { get; set; }

        public string nome_dipendente { get; set; }

        public string cognome_dipendente { get; set; }

        public string note_dipendente { get; set; }

        public string inServizio_dipendente { get; set; }

        public string orario_dipendente { get; set; }

        public int index { get; set; }

        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "nome_dipendente")
                {
                    if (string.IsNullOrEmpty(nome_dipendente))
                    {
                        result = "Serve il nome del dipendente";
                    }
                }
                if (columnName == "cognome_dipendente")
                {
                    //Debug.WriteLine("al_giorno "+al_giorno);
                    if (string.IsNullOrEmpty(cognome_dipendente))
                    {
                        result = "Serve il cognome del dipendente";
                    }
                }
                if (columnName == "index")
                {
                    if (index == -1)
                    {
                        result = "Seleziona un orario da assegnare";
                    }
                }
                return result;
            }
        }
    }
}