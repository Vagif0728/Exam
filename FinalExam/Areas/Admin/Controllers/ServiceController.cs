using FinalExam.Areas.Admin.ViewModels;
using FinalExam.DAL;
using FinalExam.Models;
using FinalExam.Utilities.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalExam.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ServiceController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Service> services = await _context.Services.ToListAsync();
            return View(services);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateServiceVM serviceVM)
        {
            if (!ModelState.IsValid)
            {
                return View(serviceVM); 
            }
            if (!serviceVM.Photo.FileType("image/"))
            {
                ModelState.AddModelError("Photo", "File type uygun deyil");
                return View(serviceVM);
            }
            if (!serviceVM.Photo.FileSize(3 * 1024))
            {
                ModelState.AddModelError("Photo", "File olcusu uygun deyil");
                return View(serviceVM);
            }

            string fileName = await serviceVM.Photo.CreateFileAsync(_env.WebRootPath, "img", "icons");

            Service service = new Service
            {
                Image = fileName,
                Name = serviceVM.Name,
                Description = serviceVM.Description,
            };

            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) { return BadRequest(); }

            Service service = await _context.Services.FirstOrDefaultAsync(s=>s.Id==id);

            if (service == null) { return NotFound(); } 

            UpdateServiceVM serviceVM = new UpdateServiceVM
            {
                Name= service.Name,
                Description= service.Description,
                Image= service.Image,
            };
            return View(serviceVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id,UpdateServiceVM serviceVM)
        {
            if (id <= 0) { return BadRequest(); }

            Service service = await _context.Services.FirstOrDefaultAsync(c => c.Id == id);
            if (service == null) { return NotFound();}

            bool result=  await _context.Services.AnyAsync(s => s.Name.ToLower().Trim()==service.Name.ToLower().Trim());

            if(serviceVM.Photo is not null)
            {
                if (!serviceVM.Photo.FileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type uygun deyil");
                    return View(serviceVM);
                }
                if (!serviceVM.Photo.FileSize(3 * 1024))
                {
                    ModelState.AddModelError("Photo", "File olcusu uygun deyil");
                    return View(serviceVM);
                }

                string newImage = await serviceVM.Photo.CreateFileAsync(_env.WebRootPath, "img", "icons");
                service.Image.Delete(_env.WebRootPath, "img", "icons");
                service.Image= newImage;

            }


            service.Name = serviceVM.Name;
            service.Description= service.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int Id)
        {
            if (Id <= 0) return BadRequest();
            Service service= await _context.Services.FirstOrDefaultAsync(s=>s.Id == Id);
            if (service == null) return NotFound();

          
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
