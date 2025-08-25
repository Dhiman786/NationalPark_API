using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NationalParkWebApp.Models;
using NationalParkWebApp.Models.ViewModels;
using NationalParkWebApp.Repository.IRepository;
using System.Threading.Tasks;

namespace NationalParkWebApp.Controllers
{
    public class TrailController : Controller
    {
        private readonly ITrailRepository _trailRepository;
        private readonly INationalParkRepository _nationalParkRepository;
        public TrailController(ITrailRepository trailRepository,INationalParkRepository nationalParkRepository)
        {
            _trailRepository = trailRepository;
            _nationalParkRepository = nationalParkRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            var nationalParkList = await _nationalParkRepository.GetAllAsync(SD.NationalParkAPIPath);
            TrailVM trailVM = new TrailVM()
            {
                NationalParkList = nationalParkList.Select(nl=>new SelectListItem()
                {
                    Text =nl.Name,
                    Value =nl.Id.ToString()
                }),
                Trail =new Trail()
            };
            if (id == null) return View(trailVM);
            trailVM.Trail = await _trailRepository.GetAsync(SD.TrailAPIPath, id.GetValueOrDefault());
            if (trailVM.Trail == null) return NotFound();
            return View(trailVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailVM trailVM)
        {
            if (trailVM == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            if (trailVM.Trail.Id == 0)
                await _trailRepository.CreateAsync(SD.TrailAPIPath, trailVM.Trail);
            else
                await _trailRepository.UpdateAsync(SD.TrailAPIPath, trailVM.Trail);
            return RedirectToAction(nameof(Index));
        }

        #region APIs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _trailRepository.GetAllAsync(SD.TrailAPIPath) });
        }
        [HttpDelete]
        public async Task <IActionResult> Delete(int id)
        {
            var status = await _trailRepository.DeleteAsync(SD.TrailAPIPath, id);
            if (status)
                return Json(new
                {
                    success = true,
                    message = "Data Deleted Successfully!!!"
                });
            return Json(new
            {
                success = false,
                message = "Something Went Wrong While Deleted Data!!"
            });
        }
        #endregion
    }
}
