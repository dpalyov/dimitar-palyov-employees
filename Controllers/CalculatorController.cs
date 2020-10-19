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
            
            /*Here we build a map between the key (employee1_employee2) and another dictionary mapping projects to days working together.
            */
            Dictionary<string, Dictionary<int, int>> dict = new Dictionary<string, Dictionary<int, int>>();

            //Go through each project group.
            foreach(var group in projects)
            {
                for(var i = 0; i < group.Count(); i++)
                {

                    int projectId;
                    int empA;
                    int empB;

                    //Check if each user`s TimeSpan is intersecting his colleagues TimeSpan.
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
                            //If we have intersection, calculate the days and reevalute the max days period.
                            int days = Calculate.IntersectionDays(
                                group.ElementAt(i).DateFrom,
                                group.ElementAt(i).DateTo,
                                group.ElementAt(j).DateFrom,
                                group.ElementAt(j).DateTo
                            );

                            empA = group.ElementAt(i).EmployeeId;
                            empB = group.ElementAt(j).EmployeeId;
                            projectId = group.Key;

                            string key = $"{empA}_{empB}";
                            
                            //Filling the dictionary.
                            if(!dict.ContainsKey(key))
                            {
                                Dictionary<int, int> projectDict = new Dictionary<int, int>();
                                projectDict.Add(projectId, days);
                                dict.Add(key, projectDict);
                            }
                            else
                            {
                                //Check if we already have the project on the dictionary.
                                if(!dict[key].Keys.Contains(projectId))
                                {
                                    dict[key].Add(projectId, days);
                                }
                                else
                                {
                                    //Summing all days working together on that specific project.
                                    dict[key][projectId] += days;
                                }
                            }
                             
                         }
                     }

                }
            }

            string bestDuoKey = "";
            int maxDays = 0;

            foreach(KeyValuePair<string, Dictionary<int,int>> kv in dict)
            {
                //summing all the days spent per project to get the total days each pair`s been working together.
                int totalDaysOnProjects = kv.Value.Aggregate(0,(acc, next) => {
                    acc += next.Value;
                    return acc;
                });

                //getting the max of all and assigning the winning pair.
                if(totalDaysOnProjects > maxDays)
                {
                    maxDays = totalDaysOnProjects;
                    bestDuoKey = kv.Key;
                }
            }

            //Splitting the key to get employee 1 and 2.
            string[] employees = bestDuoKey.Split("_");

            List<ResultView> projectsResult = new List<ResultView>();

            //Go through all projects of the employee pair and prepare the result.
            foreach(KeyValuePair<int,int> projectDays in dict[bestDuoKey])
            {
                ResultView rv = new ResultView 
                {
                    EmployeeIdA = int.Parse(employees[0]),
                    EmployeeIdB = int.Parse(employees[1]),
                    ProjectId = projectDays.Key,
                    DaysOnProject = projectDays.Value
                };

                //Logging to console as well.
                Console.WriteLine(rv.ToString() + "\r\n-----------------");

                projectsResult.Add(rv);
            }
            
            return new JsonResult(projectsResult);
        }
    }

    class ProjectGroup 
    {
        public int EmployeeId { get; set; }
        public DateTime DateFrom {get; set; }
        public DateTime DateTo {get; set; }
    }
}
