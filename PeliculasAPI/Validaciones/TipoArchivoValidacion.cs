using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.Validaciones
{
    public class TipoArchivoValidacion : ValidationAttribute
    {
        private readonly string[] tipoValidos;

        public TipoArchivoValidacion(string[] tipoValidos)
        {
            this.tipoValidos = tipoValidos;
        }

        public TipoArchivoValidacion(TipoArchivo grupoTipoArchivo)
        {
            if (grupoTipoArchivo == TipoArchivo.Imagen)
            {
                tipoValidos = new string[] { "image/jpeg", "image/png", "image/gif" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //si no me trae imagen no tiene sentido validar su peso porque no existe
            if (value == null)
            {
                return ValidationResult.Success;
            }

            //transformacion del valor a un Iformfile
            IFormFile formFile = value as IFormFile;

            //si esa transformacion no es exitosa tambien retornamos success
            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            //si no se encuentra en el listado de tipos de archivos valido (jpeg,gif, png)
            if (!tipoValidos.Contains(formFile.ContentType))
            {

                return new ValidationResult($"Solo se admiten los siguientes tipos de archivo: {string.Join(", ", tipoValidos)}");
            }

            //si las anteriores pruebas pasaron se retorna exitoso
            return ValidationResult.Success;


        }
    }
}
