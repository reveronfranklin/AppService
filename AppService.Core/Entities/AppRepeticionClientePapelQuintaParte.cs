﻿namespace AppService.Core.Entities
{
    public class AppRepeticionClientePapelQuintaParte
    {

        public int Id { get; set; }
        public string IdCliente { get; set; }
        public string NombreProducto { get; set; }
        public string NombreForma { get; set; }
        public string BasicaHumano { get; set; }
        public string OpuestaHumano { get; set; }
        public int partes_formula { get; set; }
        public int Cant_Tintas { get; set; }
        public string PAPELPRIMERAPARTE { get; set; }
        public string PAPELSEGUNDAPARTE { get; set; }
        public string PAPELTERCERAPARTE { get; set; }
        public string PAPELCUARTAPARTE { get; set; }
        public string PAPELQUINTAPARTE { get; set; }

    }
}