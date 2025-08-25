using Microsoft.AspNetCore.Mvc;
using NationalParkWebApp.Models;
using NationalParkWebApp.Repository.IRepository;
using NuGet.Protocol.Plugins;
using System.Threading.Tasks;

namespace NationalParkWebApp.Controllers
{
    public class NationalParkController : Controller
    {
        private readonly INationalParkRepository _nationalParkRepository;
        public NationalParkController(INationalParkRepository nationalParkRepository)
        {
            _nationalParkRepository = nationalParkRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark nationalPark = new NationalPark();
            if (id == null) return View(nationalPark);
            nationalPark = await _nationalParkRepository.GetAsync
                (SD.NationalParkAPIPath, id.GetValueOrDefault());
            if (nationalPark == null) return NotFound();
            return View(nationalPark);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark nationalPark)
        {
            if (nationalPark == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest();
            //******
            var files = HttpContext.Request.Form.Files;
            if (files.Count() > 0)
            {
                byte[] p1 = null;
                using (var fs = files[0].OpenReadStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);
                        p1 = ms.ToArray();
                    }
                }
                nationalPark.Picture = p1;
            }
            else
            {
                var nationalParkInDb = await _nationalParkRepository.GetAsync
                    (SD.NationalParkAPIPath, nationalPark.Id);
                nationalPark.Picture = nationalParkInDb.Picture;
            }
            if (nationalPark.Id == 0)
                await _nationalParkRepository.CreateAsync(SD.NationalParkAPIPath, nationalPark);
            else
                await _nationalParkRepository.UpdateAsync(SD.NationalParkAPIPath, nationalPark);
            return RedirectToAction(nameof(Index));
        }

        #region APIs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var nationalParkList = await _nationalParkRepository.GetAllAsync(SD.NationalParkAPIPath);
            return Json(new { data = nationalParkList });
        }
        [HttpDelete]
        public async Task<IActionResult>Delete(int id)
        {
            var status = await _nationalParkRepository.DeleteAsync(SD.NationalParkAPIPath, id);
            if (status)
                return Json(new { success = true, message = "Data Deleted Successfully!!!" });
            return Json(new { success = false, message = "Something Went Wrong While Delete Data!!!" });
        }
        #endregion


    }
}
