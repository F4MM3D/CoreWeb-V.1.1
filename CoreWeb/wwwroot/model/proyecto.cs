using System;


namespace CoreWeb
{
    public class Proyecto
    {
        public int Id { get; set; }
        public string NombreProyecto { get; set; }
        public string Descripcion { get; set; }
        public string Tecnologias { get; set; }
        public string UrlProyecto { get; set; }
        public string ImagenUrl { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}