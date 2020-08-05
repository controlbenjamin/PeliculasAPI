using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Migrations;
using PeliculasAPI.Servicios;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        //nombre de la carpeta donde se van a guardar las fotos de los actores
        private readonly string contenedor = "Actores";

        public ActoresController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            _context = context;
            _mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
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

            if (actorCreacionDTO.Foto != null)
            {

                using (var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    entidad.Foto = await almacenadorArchivos.GuardarArchivos(contenido, extension, contenedor,
                        actorCreacionDTO.Foto.ContentType);
                }
            }

            _context.Actores.Add(entidad);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<ActorDTO>(entidad);

            return new CreatedAtRouteResult("obtenerAutor", new { id = entidad.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            /* Esto se comenta, porque el usuario no siempre va a actualizar la foto,
             * sino que podria actualizar otros campos
              
             
            var entidad = _mapper.Map<Actor>(actorCreacionDTO);
            entidad.Id = id;

            //modificar ese objeto
            _context.Entry(entidad).State = EntityState.Modified;
            */


            var actorDB = _context.Actores.FirstOrDefault(x => x.Id == id);

            if (actorDB == null) { return NotFound(); }

            //vamos a mapear lo que nos mando el usuario con lo que tenemos en la base
            //entity framework, solo va a guardar los campos que son distintos
            // los campos que son distintos en el mapeo entre los del objeto de la base
            //y el dto correspondiente van a ser actualizados
            actorDB = _mapper.Map(actorCreacionDTO, actorDB);

            using (var memoryStream = new MemoryStream())
            {
                await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                var contenido = memoryStream.ToArray();
                var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                actorDB.Foto = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor,
                   actorDB.Foto, actorCreacionDTO.Foto.ContentType);
            }

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


        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {

            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entidadDB = await _context.Actores.FirstOrDefaultAsync(x=>x.Id == id);

            if (entidadDB == null)
            {
                return NotFound();
            }

            var entidadDTO = _mapper.Map<ActorPatchDTO>(entidadDB);

            patchDocument.ApplyTo(entidadDTO, ModelState);

            var esValido = TryValidateModel(entidadDTO);


            if (!esValido) {
                return BadRequest(ModelState);
            }

            _mapper.Map(entidadDTO, entidadDB);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    
    
    
    }
}
