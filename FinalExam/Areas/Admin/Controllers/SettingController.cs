using FinalExam.Areas.Admin.ViewModels;
using FinalExam.DAL;
using FinalExam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalExam.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class SettingController : Controller
	{
		private readonly AppDbContext _context;

		public SettingController(AppDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			List<Setting> settings = await _context.Settings.ToListAsync();

			return View(settings);
		}

		
		
		public async Task<IActionResult> Update(int id)
		{
			if (id <= 0) return BadRequest();

			Setting setting = await _context.Settings.FirstOrDefaultAsync(s=>s.Id == id);

			if (setting == null) return NotFound();

			UpdateSettingVM settingVM = new UpdateSettingVM
			{
				Key= setting.Key,
				Value = setting.Value,
			};

			return View(settingVM);
		}
        [HttpPost]

        public async Task<IActionResult> Update(int id,UpdateSettingVM settingVM)
		{
			if (!ModelState.IsValid)
			{
				return View(settingVM);
			}

			Setting setting = await _context.Settings.FirstOrDefaultAsync(setting=>setting.Id == id);
			if (setting == null) return NotFound();

			setting.Key= settingVM.Key;
			setting.Value= settingVM.Value;
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));	
		}
	}
}
