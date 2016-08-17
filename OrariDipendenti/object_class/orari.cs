using System;
using System.ComponentModel;
using System.Diagnostics;

namespace OrariDipendenti
{
    public class orari : IDataErrorInfo
    {
        public int id_orario { get; set; }

        private string _nome_orario;

        private int _ore_settimanali_orario;

        private string _ore_giornaliere_orario;

        public string nome_orario
        {
            get
            {
                //Debug.WriteLine("sono nel get _ore_settimanali_orario");
                return _nome_orario;
            }
            set
            {
                _nome_orario = value;
            }
        }

        public int ore_settimanali_orario
        {
            get
            {
                Debug.WriteLine("sono nel get _ore_settimanali_orario");
                return _ore_settimanali_orario;
            }
            set
            {
                _ore_settimanali_orario = value;
            }
        }

        public string ore_giornaliere_orario
        {
            get
            {
                Debug.WriteLine("sono nel get _ore_giornaliere_orario");
                return _ore_giornaliere_orario;
            }
            set
            {
                _ore_giornaliere_orario = value;
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
                if (columnName == "ore_settimanali_orario")
                {
                    if (ore_settimanali_orario > 100)
                        result = "troppe ore, max 100";
                }
                if (columnName == "nome_orario")
                {
                    if (string.IsNullOrEmpty(nome_orario))
                        result = "Inserire un nome per l'orario , per favore";
                }
                return result;
            }
        }
    }
}