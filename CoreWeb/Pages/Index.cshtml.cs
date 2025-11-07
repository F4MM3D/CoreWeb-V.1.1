// Pages/Index.cshtml.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

using CoreWeb; // <-- CAMBIO: AÑADE ESTA LÍNEA

// 4. El namespace aquí es "CoreWeb.Pages" porque está en la carpeta "Pages"
namespace CoreWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        // Las clases "Proyecto" y "Contacto" ahora son visibles
        public List<Proyecto> Proyectos { get; set; } = new List<Proyecto>();

        [BindProperty]
        public Contacto ContactoForm { get; set; } = new Contacto();

        [TempData]
        public string? FormMessage { get; set; }

        public IndexModel(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection")!;
        }

        public void OnGet()
        {
            Proyectos = ObtenerProyectos();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Proyectos = ObtenerProyectos();
                return Page();
            }

            bool exito = await GuardarContactoAsync(ContactoForm);

            if (exito)
            {
                FormMessage = "¡Mensaje enviado con éxito! Gracias por contactarme.";
            }
            else
            {
                FormMessage = "Hubo un error al enviar tu mensaje. Inténtalo de nuevo.";
            }

            return RedirectToPage();
        }

        private List<Proyecto> ObtenerProyectos()
        {
            List<Proyecto> listaProyectos = new List<Proyecto>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string sqlQuery = "SELECT * FROM proyectos ORDER BY fecha_creacion DESC LIMIT 6";
                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaProyectos.Add(new Proyecto
                                {
                                    Id = reader.GetInt32("id"),
                                    NombreProyecto = reader.GetString("nombre_proyecto"),
                                    Descripcion = reader.GetString("descripcion"),
                                    Tecnologias = reader.GetString("tecnologias"),
                                    UrlProyecto = reader.GetString("url_proyecto"),
                                    ImagenUrl = reader.GetString("imagen_url"),
                                    FechaCreacion = reader.GetDateTime("fecha_creacion")
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return listaProyectos;
        }

        private async Task<bool> GuardarContactoAsync(Contacto nuevoContacto)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    string sqlQuery = @"
                        INSERT INTO contactos (nombre, email, asunto, mensaje) 
                        VALUES (@nombre, @email, @asunto, @mensaje)";

                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nuevoContacto.Nombre);
                        cmd.Parameters.AddWithValue("@email", nuevoContacto.Email);
                        cmd.Parameters.AddWithValue("@asunto", nuevoContacto.Asunto ?? string.Empty);
                        cmd.Parameters.AddWithValue("@mensaje", nuevoContacto.Mensaje);

                        int filasAfectadas = await cmd.ExecuteNonQueryAsync();
                        return filasAfectadas > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }
}