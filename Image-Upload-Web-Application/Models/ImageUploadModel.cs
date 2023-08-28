using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ImageUploadWebApplication.Models
{
    public class ImageUploadModel
    {
        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Selecione uma imagem para fazer o upload.")]
        [Display(Name = "Imagem")]
        public IFormFile Imagem { get; set; }
    }
}