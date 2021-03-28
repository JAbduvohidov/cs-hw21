using System.Data;
using System.Collections.Generic;
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
    public class PersonController : Controller
    {
        private readonly ILogger<PersonController> _logger;
        private readonly string connectionString;
        private readonly IConfiguration _configuration;

        public PersonController(ILogger<PersonController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            connectionString = _configuration.GetConnectionString("Default");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var people = new List<Person>();
            using (var conn = new SqlConnection(connectionString))
                people = (await conn.QueryAsync<Person>("SELECT * FROM Persons;")).ToList();

            return View(people);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Person person)
        {
            if (person == null)
                return RedirectToAction("Index");

            using (var conn = new SqlConnection(connectionString))
                await conn.ExecuteAsync(@"INSERT INTO Persons(FirstName, LastName, MiddleName)
                                          VALUES(@FirstName, @LastName, @MiddleName);", person);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpGet]
        public async Task<IActionResult> FindById(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index", "Home");

            var people = new List<Person>();
            using (IDbConnection conn = new SqlConnection(connectionString))
                people = (await conn.QueryAsync<Person>($"SELECT * FROM Persons WHERE Id = {id};")).ToList();

            return View("Index", people);
        }

        [HttpGet]
        public async Task<IActionResult> FindByFullName(string fullName)
        {
            if (fullName == null || fullName == "")
                return RedirectToAction("Index", "Home");

            var people = new List<Person>();
            using (IDbConnection conn = new SqlConnection(connectionString))
                people = (await conn.QueryAsync<Person>(@$"SELECT * FROM Persons
                                                           WHERE (LastName+MiddleName+FirstName)
                                                           LIKE '%{fullName}%';")).ToList();

            return View("Index", people);
        }
    }
}