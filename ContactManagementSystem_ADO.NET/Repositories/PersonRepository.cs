using ContactManagementSystem_ADO.NET.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace ContactManagementSystem_ADO.NET.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly string _connectionString;
    
    public PersonRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public void Add(Person person, List<string> phones)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        string insertPersonSql = @"
            INSERT INTO Person(FullName)
            OUTPUT INSERTED.Id
            VALUES(@FullName)
        ";
        int newId = connection.ExecuteScalar<int>(insertPersonSql, new { FullName = person.FullName });
        person.Id = newId;

        string insertPhoneSql = @"
            INSERT INTO Phone(PhoneNumber, PersonId)
            VALUES(@PhoneNumber, @PersonId)
        ";

        foreach (var phone in phones)
        {
            connection.Execute(insertPhoneSql, new { PhoneNumber = phone, PersonId = person.Id });
        }
    }

    public void Update(Person person)
    {
        using var connection = new SqlConnection(_connectionString);

        string updatePersonSql = @"
            UPDATE Person
            SET FullName = @FullName
            WHERE Id = @Id
        ";
        
        connection.Execute(updatePersonSql, new { person.FullName, person.Id });
    }

    public void Delete(int id)
    {
        using var connection = new SqlConnection(_connectionString);

        string deletePhonesSql = @"
            DELETE FROM Phone WHERE PersonId = @Id
        ";
        connection.Execute(deletePhonesSql, new { Id = id });
        
        string deletePersonSql = @"
            DELETE FROM Person
            WHERE Id = @Id
        ";
        
        connection.Execute(deletePersonSql, new { Id = id });
    }

    public Person? GetById(int id)
    {
        using var connection = new SqlConnection(_connectionString);

        string sql = @"
        SELECT Id, FullName
        FROM Person
        WHERE Id = @Id;
    ";

        return connection.QuerySingleOrDefault<Person>(sql, new { Id = id });
    }


    public List<Person> GetAll()
    {
        using var connection = new SqlConnection(_connectionString);

        var people = new Dictionary<int, Person>();

        string sql = @"
        SELECT
            p.Id,
            p.FullName,
            ph.Id,
            ph.PhoneNumber
        FROM Person p
        LEFT JOIN Phone ph ON ph.PersonId = p.Id
        ORDER BY p.Id;
    ";

        connection.Query<Person, Phone, Person>(
            sql,
            (person, phone) =>
            {
                if (!people.TryGetValue(person.Id, out var existingPerson))
                {
                    existingPerson = person;
                    existingPerson.Phones = new List<Phone>();
                    people.Add(existingPerson.Id, existingPerson);
                }

                if (phone != null)
                {
                    existingPerson.Phones.Add(phone);
                }

                return existingPerson;
            },
            splitOn: "Id"
        );

        return people.Values.ToList();
    }

}