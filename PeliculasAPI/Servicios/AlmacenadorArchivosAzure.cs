using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.Servicios
{
    public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
    {
        private readonly string connectionString;

        public AlmacenadorArchivosAzure(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorage");
        }

       

        public Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string contentType)
        {
            throw new NotImplementedException();
        }

        public Task<string> GuardarArchivos(byte[] contenido, string extension, string contenedor, string contentType)
        {
            throw new NotImplementedException();
        }

        Task IAlmacenadorArchivos.BorrarArchivo(string ruta, string contenedor)
        {
            throw new NotImplementedException();
        }
    }
}
