using Microsoft.AspNetCore.Mvc;
using RedisPerfomTest.Models;
using RedisPerfomTest.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RedisPerfomTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICacheService _cacheService;

        private static int progressV = 0;
        private static int createRecCount = 100000;



        public HomeController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        public async Task<IActionResult> IndexAsync(bool showuser = false)
        {
            List<User> data = null;
            if (showuser)
            {
                var starttime = DateTime.UtcNow;
                data = await _cacheService.GetAsync<List<User>>("users");
                var endtime = DateTime.UtcNow;
                var sec = endtime.Subtract(starttime).TotalMilliseconds;

                if (data != null)
                    data = data.Take(1000).ToList();
                ViewBag.Message = $"Get Data Redis {sec / 100.0} seconds (First 1000 Record Showing)";
            }

            ViewBag.UserData = data;
            return View();
        }

        [HttpPost]
        public async Task<string> CreateDataAsync()
        {
            _cacheService.Remove("users");
            var models = await _cacheService.GetAsync<List<User>>("users") ?? new List<User>();
            TempData["LastValue"] = 0;


            for (var i = 1; i <= createRecCount; i++)
            {
                progressV = i;

                var user = new User()
                {
                    Name = Faker.Name.First(),
                    Suffix = Faker.Name.Suffix(),
                    SurName = Faker.Name.Last()
                };
                models.Add(user);

            }
            var starttime = DateTime.UtcNow;

            await _cacheService.SetAsync("users", models);
            var endtime = DateTime.UtcNow;
            var sec = endtime.Subtract(starttime).TotalMilliseconds;

            return $"Data Upload Redis {sec / 100.0} seconds";
        }

        [HttpPost]
        public async Task<string> AddUserData()
        {
            var models = await _cacheService.GetAsync<List<User>>("users") ?? new List<User>();

            var user = new User()
            {
                Name = Faker.Name.First(),
                Suffix = Faker.Name.Suffix(),
                SurName = Faker.Name.Last()
            };
            models.Add(user);


            await _cacheService.SetAsync("users", models);

            return $"User Added";
        }

        public IActionResult GetData()
        {
            return RedirectToAction("Index", new { showuser = true });
        }

        public int GetLastValue()
        {
            var res = ((double)progressV / (double)createRecCount) * 100.0;
            return (int)res;
        }
        public IActionResult RemoveData()
        {
            _cacheService.Remove("users");
            return RedirectToAction("Index", new { showuser = false });
        }

    }
}
