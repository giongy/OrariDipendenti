using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace OrariDipendenti
{
    public class malattia : IDataErrorInfo
    {
        public List<listaDipendentiCombo> combo_lista_dip { get; set; }
        public int index { get; set; }
        public DateTime? dal_giorno { get; set; }
        private DateTime? _al_giorno { get; set; }
        public string note { get; set; }

        public DateTime? al_giorno
        {
            get
            {
                return _al_giorno;
            }
            set
            {
                _al_giorno = value;
            }
        }

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

                if (columnName == "al_giorno")
                {
                    //Debug.WriteLine("al_giorno "+al_giorno);
                    if (al_giorno.Equals(null))
                    {
                        result = "serve una data di fine malattia";
                    }
                }
                if (columnName == "dal_giorno")
                {
                    //Debug.WriteLine("al_giorno "+al_giorno);
                    if (dal_giorno.Equals(null))
                    {
                        result = "serve una data di inizio malattia";
                    }
                }
                if (columnName == "index")
                {
                    Debug.WriteLine("combo " + index);
                    if (index == -1)
                    {
                        result = "Seleziona un dipendente";
                    }
                }

                return result;
            }
        }
    }
}