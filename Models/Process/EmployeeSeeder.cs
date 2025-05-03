using tao_project.Models.Entities;
using tao_project.Data;
using Bogus; // Thêm namespace này cho Faker
using System.Collections.Generic; // Thêm cho List<T>
using System; // Thêm cho DateTime

namespace tao_project.Models.Process
{
    public class EmployeeSeeder
    {
        private readonly ApplicationDbContext _context;

        public EmployeeSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SeedEmployees(int n)
        {
            // Kiểm tra nếu bảng đã có dữ liệu thì không seed lại
            if (_context.Employees.Any()) 
                return;

            var employees = GenerateEmployees(n);
            _context.Employees.AddRange(employees); // Sửa thành Employees thay vì Employee
            _context.SaveChanges();
        }

        private List<Employee> GenerateEmployees(int n)
        {
            var faker = new Faker<Employee>()
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.Address, f => f.Address.FullAddress()) // Sửa lỗi chính tả Address -> Address
                .RuleFor(e => e.DateOfBirth, f => f.Date.Past(30, DateTime.Now.AddYears(-20)))
                .RuleFor(e => e.Position, f => f.Name.JobTitle())
                .RuleFor(e => e.Email, (f, e) => f.Internet.Email(e.FirstName, e.LastName))
                .RuleFor(e => e.HireDate, f => f.Date.Past(10));
            
            return faker.Generate(n);
        }
    }
}