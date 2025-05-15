namespace PhoenixContact.Core.Model
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public decimal Salary { get; set; }

        public string PositionLevel { get; set; }

        public string Residence { get; set; }

        public decimal GrossSalary => Salary + (Salary * 0.19m);
    }
}
