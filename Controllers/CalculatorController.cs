using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using dimitar_palyov_employees.Models;
using dimitar_palyov_employees.Util.Extensions;
using dimitar_palyov_employees.ViewModels;
using dimitar_palyov_employees.Util;
using Microsoft.AspNetCore.Http;

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

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {

            if(file.Length == 0) 
            {
                return BadRequest("No file selected!");
            }

            var content = "";
            using(StreamReader sr = new StreamReader(file.OpenReadStream())) 
            {
                content = await sr.ReadToEndAsync();
            }

            IEnumerable<Input> inputCollection = content.DeserializeCsv(", ", "\r\n");

            IEnumerable<IGrouping<int, ProjectGroup>> projects = 
                from input in inputCollection
                group new ProjectGroup {EmployeeId = input.EmployeeId, DateFrom = input.DateFrom, DateTo = input.DateTo}  
                by input.ProjectId into projGroup
                where projGroup.Count() > 1 //take only projects where we have more than one person working.
                select projGroup;
            

            int maxDays = 0;
            int projectId = 0;
            int empA = 0;
            int empB = 0;
            
            //go through each project group
            foreach(var group in projects)
            {
                for(var i = 0; i < group.Count(); i++)
                {
                    //check if each user`s TimeSpan is intersecting his colleagues TimeSpan
                     for(var j = i + 1; j < group.Count(); j++)
                     {
                         bool isIntersecting = Calculate.IsIntersecting(
                             group.ElementAt(i).DateFrom,
                             group.ElementAt(i).DateTo,
                             group.ElementAt(j).DateFrom,
                             group.ElementAt(j).DateTo
                         );

                         if(isIntersecting)
                         {
                             //if we have intersection, calculate the days and reevalute the max days period.
                             int days = Calculate.IntersectionDays(
                                group.ElementAt(i).DateFrom,
                                group.ElementAt(i).DateTo,
                                group.ElementAt(j).DateFrom,
                                group.ElementAt(j).DateTo
                             );
                             
                             if(days > maxDays)
                             {
                                 maxDays = days;
                                 empA = group.ElementAt(i).EmployeeId;
                                 empB = group.ElementAt(j).EmployeeId;
                                 projectId = group.Key;
                             }
                         }
                     }

                }
            }

            ResultView rv = new ResultView 
            {
                EmployeeIdA = empA,
                EmployeeIdB = empB,
                ProjectId = projectId,
                DaysOnProject = maxDays
            };

            Console.WriteLine(rv.ToString());
            
            return new JsonResult(rv);
        }
    }

    class ProjectGroup 
    {
        public int EmployeeId { get; set; }
        public DateTime DateFrom {get; set; }
        public DateTime DateTo {get; set; }
    }
}
