using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApilnAsp.Models;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly EmpContext _context;

    public EmployeeController(EmpContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Employee>>> getemployees()
    {
        return Ok(await _context.Employees.ToListAsync());
    }

    [HttpGet("emp2")]
    public List<Employee> getemployees2()
    {
        return _context.Employees.ToList();
    }

    [HttpPost]
    public async Task<ActionResult<List<Employee>>> AddEmployee(Employee emp)
    {
        _context.Employees.Add(emp);
        await _context.SaveChangesAsync();
        return Ok(await _context.Employees.ToListAsync());
    }

    [HttpPost("emp_post2")]
    public async Task<ActionResult<Employee>> AddEmployee2(Employee emp)
    {
        await _context.Employees.AddAsync(emp);
        await _context.SaveChangesAsync();
        return Ok(emp);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, Employee emp)
    {
        if (id != emp.Id)
        {
            return BadRequest("ID mismatch");
        }

        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound("Employee not found");
        }

        employee.FirstName = emp.FirstName;
        employee.LastName = emp.LastName;
        employee.Email = emp.Email;
        employee.Age = emp.Age;

        await _context.SaveChangesAsync();

        return Ok(employee);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound("Employee not found");
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return Ok("Employee deleted successfully");
    }
}