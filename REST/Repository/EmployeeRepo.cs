using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace REST.Repository;

public class EmployeeRepo: IEmployeeRepo
{
    private static ConcurrentDictionary<int, Employee> customersCache;
    private NorthwindContext db;

    public EmployeeRepo(NorthwindContext db)
    {
        this.db = db;
        customersCache = new ConcurrentDictionary<int, Employee>(db.Employees.ToDictionary(e=>e.EmployeeId));
    }
    private Employee UpdateCache(int id, Employee employee)
    {
        Employee old;
        if (customersCache.TryGetValue(id, out old))
        {
            if (customersCache.TryUpdate(id, employee, old))
            {
                return employee;
            }
        }
        return null;
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        EntityEntry<Employee> added = await db.Employees.AddAsync(employee);
        int affectedRows = await db.SaveChangesAsync();
        if(affectedRows > 0)
        {
            return customersCache.AddOrUpdate(employee.EmployeeId, employee, UpdateCache);
        }
        return null;
    }

    public async Task<bool?> DeleteAsync(int employeeId)
    {
        Employee? e = db.Employees.Find(employeeId);
        if(e != null)
        {
            db.Employees.Remove(e);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                return customersCache.TryRemove(employeeId, out e);
            }
        }
        return null;
    }

    public Task<IEnumerable<Employee>> GetAllAsync()
    {
        return Task.Run<IEnumerable<Employee>>(() => customersCache.Values);
    }

    public Task<Employee?> GetAsync(int employeeId)
    {
        return Task.Run(() =>
        {
            customersCache.TryGetValue(employeeId, out Employee? e);
            return e;
        });
    }

    public async Task<Employee> UpdateAsync(int employeeId, Employee employee)
    {
        db.Employees.Update(employee);
        int affected = await db.SaveChangesAsync();
        if (affected == 1)
        {
            return UpdateCache(employeeId, employee);
        }
        return null;
    }
}