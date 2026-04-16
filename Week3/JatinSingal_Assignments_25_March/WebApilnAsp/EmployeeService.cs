using Microsoft.EntityFrameworkCore;
using WebApilnAsp.Models;

namespace WebApilnAsp
{
    public class EmployeeService : IEmployee
    {
        private readonly EmpContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeeService(EmpContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<Employee> AddEmployeeAsync(Employee employee, IFormFile? image)
        {
            if (image != null && image.Length > 0)
            {
                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                var folderPath = Path.Combine(_env.WebRootPath, "uploads");
                var imagePath = Path.Combine(folderPath, imageName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                employee.ImagePath = "/uploads/" + imageName;
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync(int pageNumber, int pageSize)
        {
            return await _context.Employees
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<Employee?> UpdateEmployeeAsync(Employee employee, IFormFile? image)
        {
            var existing = await _context.Employees.FindAsync(employee.Id);
            if (existing == null)
            {
                return null;
            }

            existing.FirstName = employee.FirstName;
            existing.LastName = employee.LastName;
            existing.Email = employee.Email;
            existing.Age = employee.Age;

            if (image != null && image.Length > 0)
            {
                if (!string.IsNullOrEmpty(existing.ImagePath))
                {
                    var oldImagePath = Path.Combine(_env.WebRootPath, existing.ImagePath.TrimStart('/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                var folderPath = Path.Combine(_env.WebRootPath, "uploads");
                var imagePath = Path.Combine(folderPath, imageName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                existing.ImagePath = "/uploads/" + imageName;
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        //public async Task<Employee?> DeleteEmployeeAsync(int id)
        //{
        //    var emp = await _context.Employees.FindAsync(id);
        //    if (emp == null) return null;

        //    _context.Employees.Remove(emp);
        //    await _context.SaveChangesAsync();
        //    return emp;
        //}
        public async Task<Employee?> DeleteEmployeeAsync(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return null;

       
            if (!string.IsNullOrEmpty(emp.ImagePath))
            {
                var imagePath = Path.Combine(_env.WebRootPath, emp.ImagePath.TrimStart('/'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }


            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();

            return emp;
        }
    }
}