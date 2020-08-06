using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.DTOs
{
    public class PaginacionDTO
    {

        public int Pagina { get; set; } = 1;

        private int cantidadDeRegistrosPorPagina = 10;

        private readonly int cantidadMaximaDeRegistrosPorPagina = 50;

        public int CantidadRegistrosPorPagina{ 
            
            get => cantidadDeRegistrosPorPagina;
            set {
                cantidadDeRegistrosPorPagina = (value > cantidadMaximaDeRegistrosPorPagina) ? cantidadMaximaDeRegistrosPorPagina : value;
            }
        }

    }
}
