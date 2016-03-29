// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StarterMvc.Models;
using StarterMvc.Services;
using StarterMvc.ViewModels.Account;

#pragma warning disable 1998
namespace StarterMvc.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        private const string _fakeEmail = "fake@test.com";
        private const string _fakePwd = "fakePwd";
        private const string _fakeCode = "fakeCode";
        private const string _fakeProvider = "fakeProvider";
        private const string _fakeProviderValue = "fakeProviderValue";
        private const string _fakeGroup = "fakeGroup";

        public AccountController(
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
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel
            {
                Email = _fakeEmail,
                Password = _fakePwd,
                RememberMe = true
            });

        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (model == null)
            {
                model = new LoginViewModel
                {
                    Email = _fakeEmail,
                    Password = _fakePwd,
                    RememberMe = true
                };
            }
            return ModelState.IsValid ? View(model) : View("Error");
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel {
                Email = _fakeEmail,
                Password = _fakePwd,
                ConfirmPassword = _fakePwd
            });
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            return ModelState.IsValid ? View(model) : View("Error");
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            return ModelState.IsValid ?
                View("Login",
                new LoginViewModel
                {
                    Email = _fakeEmail,
                    Password = _fakePwd,
                    RememberMe = true
                }) : View("Error");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            return ModelState.IsValid ?
                View("Login",
                new LoginViewModel
                {
                    Email = _fakeEmail,
                    Password = _fakePwd,
                    RememberMe = true
                }) : View("Error");
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            return ModelState.IsValid ?
                View("Login",
                new LoginViewModel
                {
                    Email = _fakeEmail,
                    Password = _fakePwd,
                    RememberMe = true
                }) : View("Error");
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            return ModelState.IsValid ? View(model) : View("Error");
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel {
                Email = _fakeEmail
            });
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View(new ResetPasswordViewModel {
                Email = _fakeEmail,
                Password = _fakePwd,
                ConfirmPassword = _fakePwd,
                Code = _fakeCode
            });
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            return ModelState.IsValid ? View(model) : View("Error");
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var tmpSendCodeModel = new SendCodeViewModel
            {
                SelectedProvider = _fakeProvider,
                Providers = new List<SelectListItem>(),
                ReturnUrl = "",
                RememberMe = true,
            };
            tmpSendCodeModel.Providers.Add(new SelectListItem
            {
                Disabled = false,
                Selected = true,
                Text = _fakeProvider,
                Value = _fakeProviderValue,
                Group = new SelectListGroup
                {
                    Disabled = false,
                    Name = _fakeGroup,
                }
            });
            return View(tmpSendCodeModel);
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            return ModelState.IsValid ? View(model) : View("Error");
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            return View(new VerifyCodeViewModel {
                Provider = _fakeProvider,
                ReturnUrl = "",
                Code = _fakeCode,
                RememberMe = true,
                RememberBrowser = false
            });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            return ModelState.IsValid ? View(model) : View("Error");
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(_userManager.GetUserId(HttpContext.User));
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
