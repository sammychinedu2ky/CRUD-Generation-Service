using CRUD_Generator;

var p = "Sam";
[GenerateCRUD]
public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
