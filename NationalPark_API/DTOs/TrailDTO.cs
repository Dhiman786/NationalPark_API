using NationalPark_API.Models;
using System.ComponentModel.DataAnnotations;
using static NationalPark_API.Models.Trail;

namespace NationalPark_API.DTOs
{
    public class TrailDTO
    {
        public int Id {  get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Distance { get; set; }
        public string Elevation {  get; set; }  
        public DifficultyType Difficulty { get; set; }
        public int NationalParkId { get; set; }
        public NationalPark NationalPark {  get; set; }
    }
}
