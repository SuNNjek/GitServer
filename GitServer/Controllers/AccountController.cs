using System;
using System.Threading.Tasks;
using GitServer.Models;
using GitServer.Security;
using GitServer.Settings;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace GitServer.Controllers
{
	[Authorize]
    public class AccountController : Controller
    {
		private readonly IOptions<EmailSettings> _emailSettings;
		private readonly UserManager<GitServerUser> _userManager;
		private readonly RoleManager<GitServerRole> _roleManager;
		private readonly SignInManager<GitServerUser> _signInManager;

		protected EmailSettings EmailSettings => _emailSettings.Value;

		public AccountController(
			IOptions<EmailSettings> emailSettings,
			UserManager<GitServerUser> userManager,
			RoleManager<GitServerRole> roleManager,
			SignInManager<GitServerUser> signInManager)
		{
			_emailSettings = emailSettings;
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
		}

		public async Task<IActionResult> Logout()
		{
			if(_signInManager.IsSignedIn(User))
			{
				await _signInManager.SignOutAsync();
			}

			return RedirectToRoute("Home");
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login(string returnUrl = null)
		{
			if(_signInManager.IsSignedIn(User))
			{
				if(returnUrl != null)
					return LocalRedirect(returnUrl);
				return RedirectToRoute("Home");
			}

			ViewBag.IsLoginPage = true;
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
		{
			ViewBag.ReturnUrl = returnUrl;
			if(ModelState.IsValid)
			{
				SignInResult result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
				if(result.Succeeded)
				{
					if(returnUrl != null)
						return LocalRedirect(returnUrl);
					return RedirectToRoute("Home");
				}
				else
				{
					ModelState.AddModelError(String.Empty, "Invalid login credentials");
					return View(model);
				}
			}

			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register()
		{
			if (_signInManager.IsSignedIn(User))
				return RedirectToRoute("Index");

			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if(ModelState.IsValid)
			{
				GitServerUser user = new GitServerUser
				{
					UserName = model.Username,
					Email = model.Email,
					CreatedAt = DateTime.Now
				};

				IdentityResult result = await _userManager.CreateAsync(user, model.Password);
				if(result.Succeeded)
				{
					IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "user");

					if (roleResult.Succeeded)
					{
						HttpRequest request = HttpContext.Request;

						string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
						string confirmationLink = Url.RouteUrl("ConfirmEmail", new { userId = user.Id, token = confirmationToken }, request.Scheme, request.Host.ToString());

						SendConfirmationMail(request.Host.Host, user.Email, user.UserName, confirmationLink);

						ViewBag.RegistrationSuccessful = true;
					}
				}
			}

			return View(model);
		}

		public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
		{
			GitServerUser user = await _userManager.FindByIdAsync(userId.ToString());
			if(user != null)
			{
				IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
				if(result.Succeeded)
				{
					return View(true);
				}
			}

			return View(false);
		}

		private void SendConfirmationMail(string host, string toMail, string userName, string confirmationLink)
		{
			MimeMessage message = new MimeMessage();
			message.From.Add(new MailboxAddress($"noreply@{host}"));
			message.To.Add(new MailboxAddress(toMail));
			message.Subject = $"GitServer @ {host} - Registration successful";

			BodyBuilder messageBuilder = new BodyBuilder();
			messageBuilder.HtmlBody = $@"
<div style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;"">
	<h1>Thanks for your registration {userName}!</h1>
	<p>The only thing you need to do now is to confirm your e-mail address. To do so, just click on this link:</p>
	<a href=""{confirmationLink}"">{confirmationLink}</a>
	<p>If you didn't register for a GitServer account at {host}, then you can just ignore this message.</p>
</div>
";

			message.Body = messageBuilder.ToMessageBody();

			using (SmtpClient client = new SmtpClient())
			{
				client.Connect(EmailSettings.ServerUri, EmailSettings.ServerPort, EmailSettings.UseSSL);

				if (EmailSettings.RequiresAuthentication)
				{
					client.AuthenticationMechanisms.Remove("XOAUTH2");
					client.Authenticate(EmailSettings.User, EmailSettings.Password);
				}

				client.Send(message);
				client.Disconnect(true);
			}
		}
	}
}