namespace ContactManagementSystem_ADO.NET.Models;

public class Person : BaseModel
{
    public string FullName { get; set; }

    public List<Phone> Phones { get; set; } = new List<Phone>();
}