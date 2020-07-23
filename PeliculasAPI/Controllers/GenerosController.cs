using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        //constructor
        public GenerosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> Get()
        {
            //retorno la lista de generos
            var entidades = await _context.Generos.ToListAsync();

            //se mapean las entidades a DTOs
            var dtos = _mapper.Map<List<GeneroDTO>>(entidades);

            return dtos;
        }

        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            //compara el id con el que esta en la base
            var entidad = await _context.Generos.FirstOrDefaultAsync(x => x.Id.Equals(id));

            //si no encuentra nada, enviar notfound de respuesta
            if (entidad == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<GeneroDTO>(entidad);

            return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            //mapeamos los que nos mandan los usuarios hacia una entidad
            var entidad = _mapper.Map<Genero>(generoCreacionDTO);

            //agregamos un nuevo registro
            _context.Add(entidad);

            //guardamos los cambios asincronicamente
            await _context.SaveChangesAsync();

            //tenemos que volver a convertirlo en otro dto para una respuesta al usuario
            var generoDTO = _mapper.Map<GeneroDTO>(entidad);

            return new CreatedAtRouteResult("obtenerGenero", new { id = generoDTO.Id }, generoDTO);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var entidad = _mapper.Map<Genero>(generoCreacionDTO);
            entidad.Id = id;

            //se le dice que la entidad fue modificada
            _context.Entry(entidad).State = EntityState.Modified;

            //guardamos cambios
            await _context.SaveChangesAsync();

            return NoContent();


        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {

            var existe = await _context.Generos.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            _context.Remove(new Genero() { Id = id});

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
