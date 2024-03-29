﻿using FinalExam.DAL;
using Microsoft.EntityFrameworkCore;

namespace FinalExam.Services
{
	public class LayoutService
	{
		private readonly AppDbContext _context;

		public LayoutService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Dictionary<string,string>> GetSettingAsync()
		{
			var setting = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
			return setting;
		}
	}
}
