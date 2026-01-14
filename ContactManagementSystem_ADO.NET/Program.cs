using ContactManagementSystem_ADO.NET.Models;
using ContactManagementSystem_ADO.NET.Repositories;

string connectionString = 
    "Server=localhost;Database=ContactsDb;Trusted_Connection=True;TrustServerCertificate=True;";
    
var personRepository = new PersonRepository(connectionString);

while (true)
{
    Console.WriteLine(@"
        Choose one option:
        1. Add a person
        2. Delete a person
        3. Update a person
        4. Get a person
        5. Get All People List
        6. Exit program
    ");
    
    int choice = Convert.ToInt32(Console.ReadLine());

    switch (choice)
    {
        case 1:
            Console.WriteLine("Enter Fullname of the person: ");
            string fullName = Console.ReadLine();
            Person personToAdd = new Person
            {
                FullName = fullName
            };
            List<string> phonesOfPerson = new List<string>();
            while (true)
            {
                Console.WriteLine(@"
                    Choose one option:
                    1. Add a new phone number
                    2. Save All
                ");
                
                int option = Convert.ToInt32(Console.ReadLine());
                if (option == 1)
                {
                    string newPhoneNumber = Console.ReadLine();
                    phonesOfPerson.Add(newPhoneNumber);
                }
                else if (option == 2)
                    break;
            }
            personRepository.Add(personToAdd, phonesOfPerson);
            break;
        case 2:
            Console.Write("Enter Person ID to delete: ");
            int idToDelete = Convert.ToInt32(Console.ReadLine());
            personRepository.Delete(idToDelete);
            break;
        case 3:
            Console.WriteLine("Enter ID of the Person to Update: ");
            int idToUpdate = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter New Fullname of the person: ");
            string newFullName = Console.ReadLine();

            Person personToUpdate = new Person
            {
                Id = idToUpdate,
                FullName = newFullName
            };
            personRepository.Update(personToUpdate);
            break;
        case 4:
            Console.WriteLine("Enter ID of the person: ");
            int id = Convert.ToInt32(Console.ReadLine());
            var getPersonById = personRepository.GetById(id);
            Console.WriteLine(getPersonById.FullName);
            break;
        case 5:
            List<Person> people = personRepository.GetAll();
            foreach (var itemPerson in people)
            {
                Console.WriteLine(itemPerson.Id + " " + itemPerson.FullName);
                Console.WriteLine("----Phone Number----");
                foreach (var phone in itemPerson.Phones)
                {
                    Console.WriteLine(phone.Id + " " + phone.PhoneNumber);
                }

                Console.WriteLine("----------------");
                Console.WriteLine();
            }
            break;            
        case 6:
            return;
        default:
            Console.WriteLine("Invalid choice, pls try again");
            break;
    }
}    