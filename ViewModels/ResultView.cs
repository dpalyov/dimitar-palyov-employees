

namespace dimitar_palyov_employees.ViewModels 
{
    public class ResultView
    {
        public int EmployeeIdA { get; set; }
        public int EmployeeIdB { get; set; }
        public int ProjectId { get; set; }
        public double DaysOnProject { get; set; }

        public override string ToString()
        {
            return $"EmployeeIdA: {EmployeeIdA.ToString()}\r\nEmployeeIdB: {EmployeeIdB.ToString()}\r\nProjectId: {ProjectId.ToString()}\r\nDaysOnProject: {DaysOnProject.ToString()}";
        }
    }
}