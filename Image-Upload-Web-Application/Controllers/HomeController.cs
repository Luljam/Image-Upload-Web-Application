using Image_Upload_Web_Application.Models;
using ImageUploadWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ImageUploadWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;
        public HomeController()
        {
            _connectionString = "Server=localhost;Database=PictureAlbum;Trusted_Connection=True;";
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ImageUploadModel model)
        {
            if (ModelState.IsValid)
            {
                // Salvar a imagem e a descrição
                if (model.Imagem != null && model.Imagem.Length > 0)
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "C:\\", "uploads", model.Imagem.FileName);
                    
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await model.Imagem.CopyToAsync(stream);
                    }
                }

                // Você pode salvar a descrição em um banco de dados ou em outro local, conforme necessário

                ViewBag.Message = "Upload realizado com sucesso!";
                return View("Index");
            }

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadSQl(ImageUploadModel model)
        {
            if (ModelState.IsValid)
            {
                // Salvar a imagem e a descrição
                if (model.Imagem != null && model.Imagem.Length > 0)
                {
                    // Converter a imagem em bytes
                    byte[] imagemBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.Imagem.CopyToAsync(memoryStream);
                        imagemBytes = memoryStream.ToArray();
                    }

                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            //command.CommandText = "INSERT INTO Imagens (Descricao, ImagemBytes) VALUES (@Descricao, @ImagemBytes)";
                            command.CommandText = "INSERT INTO Imagens (Descricao, ImagemBytes) VALUES (@Descricao, @ImagemBytes); SELECT SCOPE_IDENTITY();";

                            command.Parameters.AddWithValue("@Descricao", model.Descricao);
                            command.Parameters.AddWithValue("@ImagemBytes", imagemBytes);
                            //command.ExecuteNonQuery();
                            var insertedId = command.ExecuteScalar(); // Obtém o ID da imagem recém-inserida
                            ViewBag.InsertedId = insertedId; // Envia o ID para a View
                        }
                    }

                    ViewBag.Message = "Upload realizado com sucesso!";
                    return View("Index");
                }
            }

            return View("Index");
        }

        public IActionResult MostrarImagem(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT ImagemBytes FROM Imagens WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var imagemBytes = (byte[])reader["ImagemBytes"];
                            return File(imagemBytes, "image/jpeg"); // Substitua "image/jpeg" pelo tipo de conteúdo apropriado da sua imagem
                        }
                    }
                }
            }

            return NotFound();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
