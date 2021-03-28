using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using homework21.Models;
using System.Data.SqlClient;
using Dapper;

namespace homework21.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _conStr;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _conStr = _configuration.GetConnectionString("Default");
        }

        public async Task<IActionResult> Index()
        {
            var people = new List<Person>();
            using (IDbConnection conn = new SqlConnection(_conStr))
                people = (await conn.QueryAsync<Person>("SELECT * FROM Persons;")).ToList();

            return View(people);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
