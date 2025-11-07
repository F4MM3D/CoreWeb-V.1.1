using System;
using System.ComponentModel.DataAnnotations;

// 2. El namespace también es "CoreWeb"
namespace CoreWeb
{
    public class Contacto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; }

        public string Asunto { get; set; }

        [Required(ErrorMessage = "El mensaje no puede estar vacío.")]
        public string Mensaje { get; set; }

        public DateTime FechaEnvio { get; set; }
        public bool Leido { get; set; }
    }
}