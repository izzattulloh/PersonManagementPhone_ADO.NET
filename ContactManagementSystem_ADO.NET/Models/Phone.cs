namespace ContactManagementSystem_ADO.NET.Models;

public class Phone : BaseModel
{
    public string PhoneNumber { get; set; }
    public int PersonId { get; set; }
}