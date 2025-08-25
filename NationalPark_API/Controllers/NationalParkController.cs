using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NationalPark_API.DTOs;
using NationalPark_API.Models;
using NationalPark_API.Repository.IRepository;

namespace NationalPark_API.Controllers
{
    [Route("api/nationalPark")]
    [ApiController]
    
    public class NationalParkController : Controller
    {
        private readonly INationalParkRepository _nationalParkRepository;
        private readonly IMapper _mapper;
        public NationalParkController(INationalParkRepository nationalParkRepository, IMapper mapper)
        {
            _mapper = mapper;
            _nationalParkRepository = nationalParkRepository;
        }


        [HttpGet]             //Display Ke Liye
        public IActionResult GetNationalParks()
        {
            var nationalParkDTOList = _nationalParkRepository.GetNationalParks().Select(_mapper.Map<NationalParkDTO>);
            return Ok(nationalParkDTOList);//200 success code
        }


        [HttpGet("{nationalParkId:int}", Name = "GetAllNationalPark")] //Find
        public IActionResult GetAllNationalPark(int nationalParkId)
        {
            var nationalPark = _nationalParkRepository.GetNationalPark(nationalParkId);
            if (nationalPark == null) return NotFound();//404
            var nationalParkDTO = _mapper.Map<NationalPark, NationalParkDTO>(nationalPark);
            return Ok(nationalParkDTO);
        }


        [HttpPost]             //Create
        public IActionResult CreateNationalPark([FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null) return BadRequest();//400
            if (!ModelState.IsValid) return BadRequest();
            if (_nationalParkRepository.NationalParkExists(nationalParkDTO.Name))
            {
                ModelState.AddModelError("", "NationalParkInDB!!");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            var nationalPark = _mapper.Map<NationalParkDTO, NationalPark>(nationalParkDTO);
            if (!_nationalParkRepository.CreateNationalPark(nationalPark))
            {
                ModelState.AddModelError("", "Something Went Wrong While Create NationalPark!!");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            //return Ok();
            return CreatedAtRoute("GetAllNationalPark", new { nationalParkId = nationalPark.Id }, nationalPark);//201
        }


        [HttpPut]                   //Update ke liye
        public IActionResult UpdateNationalPark([FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            var nationalPark = _mapper.Map<NationalPark>(nationalParkDTO);
            if (!_nationalParkRepository.UpdateNationalPark(nationalPark))
            {
                ModelState.AddModelError("", "Something Went Wrong While Create NationalPark!!");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return NoContent(); //204
        }

        [HttpDelete("{nationalParkId:int}")]  //Delete ke liye
        public  IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_nationalParkRepository.NationalParkExists(nationalParkId))
                return NotFound();
            var nationlPark = _nationalParkRepository.GetNationalPark(nationalParkId);
            if (nationlPark == null) return NotFound();
            if(!_nationalParkRepository.DeleteNationalPark(nationlPark))
            {
                ModelState.AddModelError("","Something Went Wrong While Create NationalPark!!");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }       
    }
}


    