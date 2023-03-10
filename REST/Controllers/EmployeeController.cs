using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.Repository;

namespace REST.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class EmployeeController : Controller
{
    private IEmployeeRepo repo;
    public EmployeeController(IEmployeeRepo repo)
    {
        this.repo = repo;
    }
    
    [HttpGet]
    [ProducesResponseType(200,Type = typeof(IEnumerable<Employee>))]
    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        return await repo.GetAllAsync();
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Employee))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetEmployee(int id)
    {
        Employee e = await repo.GetAsync(id);
        if (e is null)
        {
            return NotFound();
        }
        return Ok(e);
    }  
    
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Employee))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] Employee e)
    {
        if (e is null)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Employee added = await repo.CreateAsync(e);
        return CreatedAtRoute(
            routeName: nameof(GetEmployee),
            routeValues: new { id = added.EmployeeId },
            value: added);
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] Employee e)
    {
        if (e.EmployeeId != id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existing = await repo.GetAsync(id);
        if (existing is null)
        {
            return NotFound();
        }
        await repo.UpdateAsync(id, e);
        return new NoContentResult();
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await repo.GetAsync(id);
        if (existing is null)
        {
            return NotFound();
        }
        bool? deleted = await repo.DeleteAsync(id);
        if (deleted.HasValue && deleted.Value)
        {
            return new NoContentResult();
        }
        return BadRequest($"Customer {id} was found but failed to delete.");
    }
}