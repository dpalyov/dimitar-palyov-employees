using System;
using System.Collections.Generic;
using System.Linq;
using dimitar_palyov_employees.Models;

namespace dimitar_palyov_employees.Util.Extensions
{
    public static class CustomExtensions
    {
        public static IEnumerable<Input> DeserializeCsv(this string str, string delimiter, string eol)
        {
            //get all the lines by splitting on the eol delimiter.
            List<string> lines = str.Split(eol,StringSplitOptions.RemoveEmptyEntries).ToList<string>();

            //headers (props) are always first element in the collection.
            lines.RemoveAt(0);

            List<Input> output = new List<Input>();

            //go through the list and parse the csv.
            foreach(string line in lines)
            {
                string[] values = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                int.TryParse(values[0], out int employeeId);
                int.TryParse(values[1], out int projectId);
                DateTime.TryParse(values[2], out DateTime dateFrom);

                //checking DateTo for NULL. If NULL we return todays date, else we sets it to its original value.
                DateTime dateTo;
                if(values[3].Equals("NULL", StringComparison.InvariantCultureIgnoreCase))
                {
                    dateTo = DateTime.Now;
                }
                else {
                    DateTime.TryParse(values[3], out dateTo);
                }

                Input input = new Input 
                {
                    EmployeeId = employeeId,
                    ProjectId = projectId,
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                //append to the result list if we have valid dates.
                if(dateFrom != DateTime.MinValue && dateTo != DateTime.MinValue)
                {
                    output.Add(input);
                }
            }

            return output;

        }
    }
}