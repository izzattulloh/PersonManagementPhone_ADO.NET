using ContactManagementSystem_ADO.NET.Models;

namespace ContactManagementSystem_ADO.NET.Repositories;

public interface IPersonRepository
{
    public void Add(Person person, List<string> phones);
    public void Update(Person person);
    public void Delete(int id);
    public Person GetById(int id);
    public List<Person> GetAll();
}