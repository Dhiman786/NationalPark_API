using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NationalPark_API.DTOs;
using NationalPark_API.Models;
using NationalPark_API.Repository.IRepository;
using System.Reflection.Metadata.Ecma335;

namespace NationalPark_API.Controllers
{
    [Route("api/trail")]
    [ApiController]
    public class TrailController : Controller
    {
        private readonly ITrailRepository _trailRepository;
        private readonly IMapper _mapper;
        public TrailController(ITrailRepository trailRepository, IMapper mapper)
        {
            _mapper = mapper;
            _trailRepository = trailRepository;
        }


        [HttpGet]  //Display
        public IActionResult GetTrails()
        {
            return Ok(_trailRepository.GetTrails().Select(_mapper.Map<TrailDTO>));
        }


        [HttpGet("{trailId:int}", Name = "GetTrail")]  //Find
        public IActionResult GetTrail(int trailId)
        {
            var trail = _trailRepository.GetTrail(trailId);
            if (trail == null) return NotFound();
            var trailDTO = _mapper.Map<TrailDTO>(trail);
            return Ok(trailDTO);
        }

        [HttpPost]  //Create
        public IActionResult CreateTrail([FromBody] TrailDTO trailDTO)
        {
            if (trailDTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            if (_trailRepository.TrailExists(trailDTO.Name))
            {
                ModelState.AddModelError("", "Trail in Db!!!");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            var trail = _mapper.Map<Trail>(trailDTO);
            if (!_trailRepository.CreateTrail(trail))
            {
                ModelState.AddModelError("", "Something Went Wrong While Create Trail!!");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtRoute("GetTrail", new { trailId = trail.Id }, trail);
        }


        [HttpPut]    //Update
        public IActionResult UpdateTrail([FromBody]TrailDTO trailDTO)
        {
            if (trailDTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            var trail = _mapper.Map<Trail>(trailDTO);
            if(!_trailRepository.UpdateTrail(trail))
            {
                ModelState.AddModelError("", "Something Went Wrong While Create Trail!!");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return NoContent();
        }


        [HttpDelete("{trailId:int}")]   //Delete
        public IActionResult DeleteTrail(int trailId)
        {
            if (!_trailRepository.TrailExists(trailId)) return NotFound();
            var trail = _trailRepository.GetTrail(trailId);
            if (trail == null) return NotFound();
            if(!_trailRepository.DeleteTrail(trail))
            {
                ModelState.AddModelError("", "Something Went Wrong While Create Trail!!");
            }
            return Ok();
        }

    }
}
