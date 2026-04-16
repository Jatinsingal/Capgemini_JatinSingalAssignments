using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using WebApilnAsp.Models;

namespace WebApilnAsp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployee _employeeService;

        public EmployeeController(IEmployee employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 5)
        {
            var data = await _employeeService.GetAllEmployeesAsync(pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("basic")]
        public async Task<ActionResult<List<EmployeeBasicDto>>> GetBasicEmployeeList(int page = 1, int pageSize = 5, string? search = null)
        {
            var result = await _employeeService.GetAllEmployeeBasicInfoAsync(page, pageSize, search);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Employee emp, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _employeeService.AddEmployeeAsync(emp, image);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> Update(int id, [FromForm] EmployeeUpdateDto employeeDto, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = new Employee
            {
                Id = id,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                Age = employeeDto.Age,
                ImagePath = employeeDto.ImagePath
            };

            var updated = await _employeeService.UpdateEmployeeAsync(employee, image);

            if (updated == null)
            {
                return NotFound("Employee not found to update");
            }

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _employeeService.DeleteEmployeeAsync(id);

            if (deleted == null)
            {
                return NotFound("Employee not found");
            }

            return Ok("Employee deleted successfully");
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel(string? search = null)
        {
            var employees = await _employeeService.GetAllEmployeeBasicInfoAsync(1, int.MaxValue, search);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Employees");

            worksheet.Cell(1, 1).Value = "First Name";
            worksheet.Cell(1, 2).Value = "Last Name";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Image URL";

            var row = 2;
            foreach (var employee in employees)
            {
                worksheet.Cell(row, 1).Value = employee.FirstName;
                worksheet.Cell(row, 2).Value = employee.LastName;
                worksheet.Cell(row, 3).Value = employee.Email;
                worksheet.Cell(row, 4).Value = employee.ImageUrl;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Employees.xlsx");
        }
    }
}
