using FinalExam.Areas.Admin.ViewModels;
using FinalExam.Models;
using FinalExam.Utilities.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalExam.Controllers
{
   // UserName = Vagif555
   // Parol = Vagif555      ---- Admin account





    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            AppUser user = new AppUser
            {
                Name = registerVM.Name,
                Email = registerVM.Email,
                UserName = registerVM.Username
            };



            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerVM);
            }


            await _userManager.AddToRoleAsync(user,UserRole.Member.ToString());
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index","Home", new {Area =" " });


        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if(!ModelState.IsValid)
            {
                return View(loginVM);
            }

            AppUser user = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "username ,password or email false");
                    return View(loginVM);
                }
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Username ,Email or Password false");
                return View(loginVM);
            }

            if(result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account blocked");
                return View(loginVM);
            }


            return RedirectToAction("Index", "Home",new {Area = " "});

            
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { Area = " " });
        }
        
        public async Task<IActionResult> CreateRole()
        {
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if(!(await _roleManager.RoleExistsAsync(role.ToString())))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
                }

            }

            return RedirectToAction("Index", "Home", new { Area = " " });
           
            
        }
    }
}
