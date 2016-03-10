using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StarterMvc.Models;
using StarterMvc.Services;
using StarterMvc.ViewModels.Manage;

#pragma warning disable 1998
namespace StarterMvc.Controllers
{
    //[Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        private const string _fakePhoneNumber = "fakePhoneNumber";
        private const string _fakeCode = "fakeCode";
        private const string _fakePwd = "fakePwd";
        private const string _fakeNewPwd = "fakeNewPwd";

        public ManageController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            return View(new IndexViewModel
                            {
                               Logins = new List<UserLoginInfo>()
                            }
                       );
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
        {
            if (ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            else
                return View("Error");
        }

        //
        // GET: /Manage/AddPhoneNumber
        public IActionResult AddPhoneNumber()
        {
            return View(new AddPhoneNumberViewModel {
                PhoneNumber = _fakePhoneNumber
            });
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            return ModelState.IsValid? View(model) : View("Error");
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            if (ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            else
                return View("Error");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            if (ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            else
                return View("Error");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber, Code = _fakeCode });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            return ModelState.IsValid ? View(model) : View("Error");
        }

        //
        // GET: /Manage/RemovePhoneNumber
        [HttpGet]
        public async Task<IActionResult> RemovePhoneNumber()
        {
            return View(nameof(Index), 
                new IndexViewModel
                {
                    Logins = new List<UserLoginInfo>()
                });
        }

        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel {
                OldPassword = _fakePwd,
                NewPassword = _fakeNewPwd,
                ConfirmPassword = _fakeNewPwd
            });
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            return ModelState.IsValid ? View(model) : View("Error");
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View(new SetPasswordViewModel {
                NewPassword = _fakeNewPwd,
                ConfirmPassword = _fakeNewPwd
            });
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            return ModelState.IsValid ? View(model) : View("Error");
        }

        //GET: /Manage/ManageLogins
        [HttpGet]
        public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        {
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = new List<UserLoginInfo>(),
                OtherLogins = new List<Microsoft.AspNetCore.Http.Authentication.AuthenticationDescription> ()
            });

        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            return RedirectToAction(nameof(ManageLogins));
        }

        //
        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            return RedirectToAction(nameof(ManageLogins));
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(_userManager.GetUserId(User));
        }

        #endregion
    }
}
