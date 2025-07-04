﻿using IronGym.Shared.Entities;
using IronGym.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Services;
using System.Security.Claims;
using System.Text.Json;
using Domain.Entities;
using System.Text;

namespace MVC.Controllers
{
    [Authorize(Roles = "Employee,Admin")]
    public class EmployeeController : Controller
    {
        private readonly IAESService _aesService;
        private readonly IRequestSenderService _requestSenderService;
        private readonly HttpClient _httpClient;
        public EmployeeController(IAESService aesService, IRequestSenderService requestSenderService)
        {
            _aesService = aesService;
            _requestSenderService = requestSenderService;
            _httpClient = new HttpClient();
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = GetJwtTokenFromCookie();
            var client = _httpClient;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://localhost:7175/api/employee/GetAll");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var userList = JsonSerializer.Deserialize<List<ShowUsersModel>>(responseData);
                return View(userList);
            }
            return View(new List<ShowUsersModel>());
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var response = await _requestSenderService.PostRequest(login, "https://localhost:7175/api/employee/Login");

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ViewBag.Errors = "Credenciales incorrectas";
                return View();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewBag.Errors = "This employee was not found";
                return View();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ViewBag.Errors = "No tienes permisos para acceder a esta sección";
                return View();
            }
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var employeeLogin = JsonSerializer.Deserialize<EmployeeLoginModel>(responseString);

                var token = employeeLogin.Token;
                var role = employeeLogin.Role;

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(3)
                };
                Response.Cookies.Append("JWToken", token, cookieOptions);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, login.Email),
                    new Claim(ClaimTypes.Role, role)
                };
                var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index");
            }
            ViewBag.Errors = "Ocurrio un errror inesperado";
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginDefaultEmployee()
        {
            LoginViewModel login = new LoginViewModel
            {
                Email = "employee@example.com",
                Password = "Password"
            };
            var response = await _requestSenderService.PostRequest(login, "https://localhost:7175/api/employee/Login");

            var responseString = await response.Content.ReadAsStringAsync();
            var employeeLogin = JsonSerializer.Deserialize<EmployeeLoginModel>(responseString);

            var token = employeeLogin.Token;
            var role = employeeLogin.Role;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(3)
            };
            Response.Cookies.Append("JWToken", token, cookieOptions);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, login.Email),
                new Claim(ClaimTypes.Role, role)
            };
            var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index");
        }




        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = GetJwtTokenFromCookie();
            var client = _httpClient;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"https://localhost:7175/api/employee/GetUserInfo/{id}");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<UserInfo>(responseData);
                return View(userInfo);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] UserInfo userInfo)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var token = GetJwtTokenFromCookie();
            var client = _httpClient;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            string jsonClient = JsonSerializer.Serialize(userInfo);
            var content = new StringContent(jsonClient, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7175/api/employee/UpdateUser", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var token = GetJwtTokenFromCookie();
            var client = _httpClient;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"https://localhost:7175/api/employee/DeleteUser/{id}");

            return RedirectToAction("Index");
        }
        private string GetJwtTokenFromCookie()
        {
            if (Request.Cookies.TryGetValue("JWToken", out var token))
            {
                return token;
            }
            return null;
        }

    }
}
