namespace REST.Repository;

public interface IEmployeeRepo
{
    Task<Employee?> GetAsync(int employeeId);
    Task<Employee> UpdateAsync(int employeeId, Employee employee);
    Task<bool?> DeleteAsync(int employeeId);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee> CreateAsync(Employee employee);
}