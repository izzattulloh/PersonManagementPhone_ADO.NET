using ContactManagementSystem_ADO.NET.Models;
using Microsoft.Data.SqlClient;

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
        
        using var cmd =  new SqlCommand(insertPersonSql, connection);
        cmd.Parameters.AddWithValue("@FullName", person.FullName);

        int newId = (int)cmd.ExecuteScalar();
        person.Id = newId;

        string insertPhoneSql = @"
            INSERT INTO Phone(PhoneNumber, PersonId)
            VALUES(@PhoneNumber, @PersonId)
        ";

        foreach (var phone in phones)
        {
            using var cmd2 = new SqlCommand(insertPhoneSql, connection);
            cmd2.Parameters.AddWithValue("@PhoneNumber", phone);
            cmd2.Parameters.AddWithValue("@PersonId", person.Id);
            
            cmd2.ExecuteNonQuery();
        }
    }

    public void Update(Person person)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        string updatePersonSql = @"
            UPDATE Person
            SET FullName = @FullName
            WHERE Id = @Id
        ";
        
        using var cmd = new SqlCommand(updatePersonSql, connection);
        cmd.Parameters.AddWithValue("@FullName", person.FullName);
        cmd.Parameters.AddWithValue("@Id", person.Id);
        
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        string deletePhonesSql = @"
            DELETE FROM Phone WHERE PersonId = @Id
        ";
        using var cmdDeletePhones = new SqlCommand(deletePhonesSql, connection);
        cmdDeletePhones.Parameters.AddWithValue("@Id", id);
        cmdDeletePhones.ExecuteNonQuery();
        
        string deletePersonSql = @"
            DELETE FROM Person
            WHERE Id = @Id
        ";
        
        using var cmd = new SqlCommand(deletePersonSql, connection);
        cmd.Parameters.AddWithValue("@Id", id);
        
        cmd.ExecuteNonQuery();
    }

    public Person? GetById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        string sql = @"
        SELECT Id, FullName
        FROM Person
        WHERE Id = @Id;
    ";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return null;

        return new Person
        {
            Id = reader.GetInt32(0),
            FullName = reader.GetString(1)
        };
    }


    public List<Person> GetAll()
    {
        var people = new Dictionary<int, Person>();

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

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

        using var cmd = new SqlCommand(sql, connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            int personId = reader.GetInt32(0);
            string fullName = reader.GetString(1);

            if (!people.TryGetValue(personId, out var person))
            {
                person = new Person
                {
                    Id = personId,
                    FullName = fullName
                };
                people.Add(personId, person);
            }

            if (!reader.IsDBNull(2))
            {
                var phone = new Phone
                {
                    Id = reader.GetInt32(2),
                    PhoneNumber = reader.GetString(3)
                };
                person.Phones.Add(phone);
            }
        }

        return people.Values.ToList();
    }
}