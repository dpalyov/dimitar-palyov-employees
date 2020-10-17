using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dimitar_palyov_employees.ViewModels;
using System.IO;

namespace dimitar_palyov_employees.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculatorController : ControllerBase
    {
        private readonly ILogger<CalculatorController> _logger;

        public CalculatorController(ILogger<CalculatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult<string>> Get()
        {
            // FileInfo file = new FileInfo(Path.Join(Directory.GetCurrentDirectory(), "input.txt"));
            // StreamReader sr = new StreamReader(file.OpenRead());

            // using(sr) 
            // {
            //     string content = await sr.ReadToEndAsync();
            // }
            
            return Ok("Hello");
        }
    }
}
