using CRUD_Generator;

var Id = Guid.NewGuid();


[GenerateCRUD]
public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}