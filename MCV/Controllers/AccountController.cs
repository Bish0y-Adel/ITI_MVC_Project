using Entities.Models;
using Entities.Repositories;

using MCV.ViewModels.Account;
using MCV.ViewModels.Account.Addresses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;

namespace MCV.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<User> signInManager;

        IUnitOfWork unitOfWork;

        public AccountController(UserManager<User> _userManager, RoleManager<IdentityRole> _roleManager, SignInManager<User> _signInManager, IUnitOfWork _unitOfWork)
        {
            userManager = _userManager;
            roleManager = _roleManager;
            signInManager = _signInManager;
            unitOfWork = _unitOfWork;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User { Email = model.Email, UserName = model.Email, FullName = model.FullName };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Customer");
                await signInManager.SignInAsync(user, isPersistent: true);
                TempData["Success"] = "Registration successful!";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", $"{error.Code}: {error.Description}");
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {

            if (!ModelState.IsValid)
                return View(model);

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                TempData["Success"] = "Login successful!";
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Login Failed.";

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            TempData["Success"] = "Logout successful!";
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        //=============================Addresses=====================================
        //[Authorize(Roles = "Customer")]
        public IActionResult Addresses()
        {
            var addresses = unitOfWork.AddressRepo.GetAll(q => q.Include(a => a.User)).Where(a => a.UserId == userManager.GetUserId(User)).ToList();
            return View(addresses);
        }

        public IActionResult CreateAddress()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAddress(CreateAddressVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors in the form.";
                return View(model);
            }
            //check if user has default address and if the new address is set as default, if so unset the old default address
            if (model.IsDefault)
            {
                var defaultAddress = unitOfWork.AddressRepo.FindAll(a => a.UserId == userManager.GetUserId(User) && a.IsDefault).FirstOrDefault();
                if (defaultAddress != null)
                {
                    defaultAddress.IsDefault = false;
                    unitOfWork.AddressRepo.Update(defaultAddress);
                }
            }

            var address = new Address
            {
                Country = model.Country,
                City = model.City,
                Street = model.Street,
                Zip = model.Zip,
                IsDefault = model.IsDefault,
                UserId = userManager.GetUserId(User)
            };
            unitOfWork.AddressRepo.Add(address);
            unitOfWork.AddressRepo.SaveChanges();
            TempData["Success"] = "Address created successfully!";
            return RedirectToAction("Addresses");
        }

        public IActionResult EditAddress(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var address = unitOfWork.AddressRepo.GetById(id.Value);
            if (address == null || address.UserId != userManager.GetUserId(User))
            {
                return NotFound();
            }
            var model = new EditAddressVM
            {
                Id = address.Id,
                Country = address.Country,
                City = address.City,
                Street = address.Street,
                Zip = address.Zip,
                IsDefault = address.IsDefault
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditAddress(EditAddressVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors in the form.";
                return View(model);
            }

            var address = unitOfWork.AddressRepo.GetById(model.Id);
            if (address == null || address.UserId != userManager.GetUserId(User))
            {
                return NotFound();
            }

            if (model.IsDefault)
            {
                var defaultAddress = unitOfWork.AddressRepo.FindAll(a => a.UserId == userManager.GetUserId(User) && a.IsDefault).FirstOrDefault();
                if (defaultAddress != null && defaultAddress.Id != model.Id)
                {
                    defaultAddress.IsDefault = false;
                    unitOfWork.AddressRepo.Update(defaultAddress);
                }
            }

            address.Country = model.Country;
            address.City = model.City;
            address.Street = model.Street;
            address.Zip = model.Zip;
            address.IsDefault = model.IsDefault;
            unitOfWork.AddressRepo.Update(address);
            unitOfWork.AddressRepo.SaveChanges();
            TempData["Success"] = "Address updated successfully!";
            return RedirectToAction("Addresses");
        }

        public IActionResult DetailsAddress(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var address = unitOfWork.AddressRepo.GetById(id.Value);
            if (address == null || address.UserId != userManager.GetUserId(User))
            {
                return NotFound();
            }

            return View(address);
        }

        public IActionResult DeleteAddress(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var address = unitOfWork.AddressRepo.GetById(id.Value);
            if (address == null || address.UserId != userManager.GetUserId(User))
            {
                return NotFound();
            }
            unitOfWork.AddressRepo.Delete(id.Value);
            unitOfWork.AddressRepo.SaveChanges();
            TempData["Success"] = "Address deleted successfully!";
            return RedirectToAction("Addresses");
        }
    }
}