using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.Servicios
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }


        public Task BorrarArchivo(string ruta, string contenedor)
        {
            if (ruta != null)
            {
                var nombreArchivo = Path.GetFileName(ruta);
                var directorioArchivo = Path.Combine(webHostEnvironment.WebRootPath, contenedor, nombreArchivo);

                if (File.Exists(directorioArchivo))
                {
                    File.Delete(directorioArchivo);
                }

            }
            return Task.FromResult(0);
        }

        public async Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string contentType)
        {
            await BorrarArchivo(ruta, contenedor);

            return await GuardarArchivos(contenido, extension, contenedor, contentType);
        }

        public async Task<string> GuardarArchivos(byte[] contenido, string extension, string contenedor, string contentType)
        {
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(webHostEnvironment.WebRootPath, contenedor);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nombreArchivo);

            await File.WriteAllBytesAsync(ruta, contenido);

            var urlActual = $"{httpContextAccessor.HttpContext.Request.Scheme}://{ httpContextAccessor.HttpContext.Request.Host}";

            var urlParaBD = Path.Combine(urlActual, contenedor, nombreArchivo).Replace("\\", "/");

            return urlParaBD;
        }
    }
}
