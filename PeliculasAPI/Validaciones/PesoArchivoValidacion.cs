using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.Validaciones
{
    public class PesoArchivoValidacion : ValidationAttribute
    {
        private readonly int pesoMaximoEnMegaBytes;

        public PesoArchivoValidacion(int pesoMaximoEnMegaBytes)
        {
            this.pesoMaximoEnMegaBytes = pesoMaximoEnMegaBytes;
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

            //evalua el peso del archivo (multiplica 2 veces por 1024, primero pasamos KB y luego a MB)
            if (formFile.Length > (pesoMaximoEnMegaBytes * 1024 * 1024))
            {
                return new ValidationResult($"El peso del archivo, no debe superar los {pesoMaximoEnMegaBytes} MB");
            }

            return ValidationResult.Success;
        }

    }
}
