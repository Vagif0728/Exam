﻿using Microsoft.AspNetCore.Identity;

namespace FinalExam.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
       
    }
}
