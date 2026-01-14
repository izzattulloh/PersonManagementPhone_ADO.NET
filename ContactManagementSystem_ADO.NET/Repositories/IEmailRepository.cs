using ContactManagementSystem_ADO.NET.Models;

namespace ContactManagementSystem_ADO.NET.Repositories;

public interface IEmailRepository
{
    public void Add(Phone phone);
    public void Update(Phone phone);
    public void Delete(Phone phone);
    public Phone GetById(int id);
    public List<Phone> GetAll();
}