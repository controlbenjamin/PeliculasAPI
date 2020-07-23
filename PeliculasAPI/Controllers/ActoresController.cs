using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Migrations;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ActoresController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get()
        {

            var entidades = await _context.Actores.ToListAsync();

            return _mapper.Map<List<ActorDTO>>(entidades);
        }

        [HttpGet("{id}", Name = "obtenerAutor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {

            var entidad = await _context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            return _mapper.Map<ActorDTO>(entidad);


        }


        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {

            var entidad = _mapper.Map<Actor>(actorCreacionDTO);

            _context.Actores.Add(entidad);
            // await _context.SaveChangesAsync();

            var dto = _mapper.Map<ActorDTO>(entidad);

            return new CreatedAtRouteResult("obtenerAutor", new { id = entidad.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {

            var entidad = _mapper.Map<Actor>(actorCreacionDTO);
            entidad.Id = id;

            //modificar ese objeto
            _context.Entry(entidad).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {

            var existe = await _context.Actores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            _context.Remove(new Actor() { Id = id });

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
