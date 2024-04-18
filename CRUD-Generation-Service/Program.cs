using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static System.Console;
var syntaxTree =
    """
    [GenerateCRUD]
    internal class Person
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    internal class Testo{

    [GenerateCRUD]
    internal class Person
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    }

    // create an attribute called 'GenerateCRUD' that will be used to generate CRUD operations
    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateCRUD : Attribute
    {
    }
    """;
var tree = CSharpSyntaxTree.ParseText(syntaxTree);
CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
WriteLine($"The tree is a {root.Kind()} node.");
WriteLine($"The tree has {root.Members.Count} elements in it.");
WriteLine($"The tree has {root.Usings.Count} using statements. They are:");
foreach (UsingDirectiveSyntax element in root.Usings)
    WriteLine($"\t{element.Name}");
foreach(var member in root.Members)
{
    WriteLine($"The member is a {member.Kind()} node.");
}
var classesWithPerson = root.Members.OfType<ClassDeclarationSyntax>().Where(c => c.Identifier.Text == "Person");
WriteLine($"There are {classesWithPerson.Count()} classes with the name 'Person' in the tree.");

var classWithGenerateCRUDAndAGuidProperty = root.Members[0] as ClassDeclarationSyntax;

var compilation = CSharpCompilation.Create("TestingTheMic")
.AddSyntaxTrees(tree);

var model = compilation.GetSemanticModel(tree);
// check if the members contain a type of Guid with the name 'Id' and if the class has the attribute 'GenerateCRUD'
if (classWithGenerateCRUDAndAGuidProperty.Members.OfType<PropertyDeclarationSyntax>().Any(p => p.Type.ToString() == "Guid" && p.Identifier.Text == "Id") && classWithGenerateCRUDAndAGuidProperty.AttributeLists.Any(a => a.Attributes.Any(a => a.Name.ToString() == "GenerateCRUD")))
{
    WriteLine("The class has a property of type Guid with the name 'Id' and the attribute 'GenerateCRUD'.");
}
else
{
    WriteLine("The class does not have a property of type Guid with the name 'Id' and the attribute 'GenerateCRUD'.");
}
var classesWithNamePerson = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Where(c => c.Identifier.Text == "Person");  
foreach (var classWithNamePerson in classesWithNamePerson)
{
    WriteLine($"The class {classWithNamePerson.Identifier.Text} has the following properties:");
    foreach (var property in classWithNamePerson.Members.OfType<PropertyDeclarationSyntax>())
    {
        WriteLine($"\t{property.Type} {property.Identifier}");
    }
    // use the semantic model to get the namespace of the class
    var classSymbol = model.GetSymbolInfo(classWithNamePerson);
    // check if it contains the namespace 
    
    // check if members of 


}   
internal class Person
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}