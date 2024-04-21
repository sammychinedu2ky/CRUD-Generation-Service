using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CRUD_Generator
{
    [Generator]
    public class MyGenerator : IIncrementalGenerator
    {
        public const string GenerateCRUDAttribute = """
            namespace CRUD_Generator
            {
                [AttributeUsage(AttributeTargets.Class)]
                public class GenerateCRUDAttribute : Attribute
                {
                    
                }
            }
            """;
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
          
            context.RegisterPostInitializationOutput(ctx =>
            {
            ctx.AddSource("GenerateCRUDAttribute.g.cs", GenerateCRUDAttribute);
            });
            if (Debugger.IsAttached) Debugger.Launch();
            var classesMarkedWithTheGeneratorAttribute = context.SyntaxProvider.ForAttributeWithMetadataName(
                "CRUD_Generator.GenerateCRUDAttribute",
                (node,_) => node is ClassDeclarationSyntax,
                (ctx,_) => (ClassDeclarationSyntax)ctx.TargetNode
                );
            context.RegisterSourceOutput(classesMarkedWithTheGeneratorAttribute.Combine(context.CompilationProvider), (ctx, classesAndCompilation) =>
            {
                var compilation = classesAndCompilation.Item2;
                var @class = classesAndCompilation.Item1;
                var model = compilation.GetSemanticModel(@class.SyntaxTree);
                var classSymbol = model.GetDeclaredSymbol(@class);
                var className = classSymbol!.Name;
                // check if class has a property named "Id" of type Guid
                var check = @class.Members.OfType<PropertyDeclarationSyntax>().Any(p => p.Identifier.Text == "Id" && model.GetTypeInfo(p.Type).Type?.Name == "Guid" && p.Modifiers.Any(m => m.Text == "public"));
                if (!check)
                {
                    var descriptor = new DiagnosticDescriptor("CRUD01", "GUID type with the property, Id not found", "GUID type with the property, Id not found", "CRUD", DiagnosticSeverity.Error, true);
                    var diagnostic = Diagnostic.Create(descriptor, @class.GetLocation());
                    ctx.ReportDiagnostic(diagnostic);
                    return;
                }
                // return a tuple of the properties that have a get and set accessor and their type
                var properties = @class.Members.OfType<PropertyDeclarationSyntax>()
                .Where(p => p.Modifiers.Any(m => m.Text == "public") &&  p.AccessorList?.Accessors
                .Any(a => a.Keyword.Text == "get") == true && p.AccessorList?.Accessors.Any(a => a.Keyword.Text == "set") == true)
                .Select(p => (p.Identifier.Text, model.GetTypeInfo(p.Type).Type?.Name));
                var sourceBuilder = new StringBuilder();
                sourceBuilder.Append($$"""
                namespace CRUD_Generator{

                    public class {{className}}Service {
                        public Guid Id { get; set;}
                        private static List<{{className}}> _data = new List<{{className}}>();
                        public {{className}}? Get{{className}}(Guid id) => _data.FirstOrDefault(d => d.Id == id);
                        public List<{{className}}> GetAll() => _data;
                        public void Add({{className}} {{className.ToLower()}}) => _data.Add({{className.ToLower()}});
                        public void Update({{className}} {{className.ToLower()}}) => _data[_data.FindIndex(d => d.Id == {{className.ToLower()}}.Id)] = {{className.ToLower()}};
                        public void Delete(Guid id) => _data.Remove(_data.FirstOrDefault(d => d.Id == id));   
                    }    
                }       
                """);
               ctx.AddSource($"{className}Service.g.cs", sourceBuilder.ToString());
                //context.RegisterPostInitializationOutput(ctx =>
                //{
                //    ctx.AddSource($"{className}Service.g.cs", sourceBuilder.ToString());
                //});
            }); 
        }
    }
}
