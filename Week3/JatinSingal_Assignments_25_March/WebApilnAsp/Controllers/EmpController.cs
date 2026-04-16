//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using WebApilnAsp.Models;

//[ApiController]
//[Route("api/[controller]")]
//public class EmployeeController : ControllerBase
//{
//    private readonly EmpContext _context;

//    public EmployeeController(EmpContext context)
//    {
//        _context = context;
//    }

//    [HttpGet]
//    public async Task<ActionResult<List<Employee>>> getemployees()
//    {
//        return Ok(await _context.Employees.ToListAsync());
//    }

//    [HttpGet("emp2")]
//    public List<Employee> getemployees2()
//    {
//        return _context.Employees.ToList();
//    }

//    [HttpPost]
//    public async Task<ActionResult<List<Employee>>> AddEmployee(Employee emp)
//    {
//        _context.Employees.Add(emp);
//        await _context.SaveChangesAsync();
//        return Ok(await _context.Employees.ToListAsync());
//    }

//    [HttpPost("emp_post2")]
//    public async Task<ActionResult<Employee>> AddEmployee2(Employee emp)
//    {
//        await _context.Employees.AddAsync(emp);
//        await _context.SaveChangesAsync();
//        return Ok(emp);
//    }

//    [HttpPut("{id}")]
//    public async Task<IActionResult> UpdateEmployee(int id, Employee emp)
//    {
//        if (id != emp.Id)
//        {
//            return BadRequest("ID mismatch");
//        }

//        var employee = await _context.Employees.FindAsync(id);

//        if (employee == null)
//        {
//            return NotFound("Employee not found");
//        }

//        employee.FirstName = emp.FirstName;
//        employee.LastName = emp.LastName;
//        employee.Email = emp.Email;
//        employee.Age = emp.Age;

//        await _context.SaveChangesAsync();

//        return Ok(employee);
//    }

//    [HttpDelete("{id}")]
//    public async Task<IActionResult> DeleteEmployee(int id)
//    {
//        var employee = await _context.Employees.FindAsync(id);

//        if (employee == null)
//        {
//            return NotFound("Employee not found");
//        }

//        _context.Employees.Remove(employee);
//        await _context.SaveChangesAsync();

//        return Ok("Employee deleted successfully");
//    }
//}



using Microsoft.AspNetCore.Mvc;
using WebApilnAsp;
using WebApilnAsp.Models;

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

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetById(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);

        if (employee == null)
            return NotFound("Employee not found");

        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] Employee emp, IFormFile? image)
    {
        var created = await _employeeService.AddEmployeeAsync(emp, image);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] Employee emp, IFormFile? image)
    {
        if (id != emp.Id)
            return BadRequest("ID mismatch");

        var updated = await _employeeService.UpdateEmployeeAsync(emp, image);

        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _employeeService.DeleteEmployeeAsync(id);

        if (deleted == null)
            return NotFound("Employee not found");

        return Ok("Employee deleted successfully");
    }
}