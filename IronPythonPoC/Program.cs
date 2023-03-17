using IronPython.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var pyScript = """
    obj = MutateModel(obj)
    """;

var aScript = """
    from System import *
    import PageNode

    class Fel(PageNode):
        def GetNextPage(self):
            answer = self.GetAnswer('x.3')
            if answer.Contains('Ja'):
                return self.GetPage('Contact')
            return self.GetPage('SummaryPage')

    fel = Fel()
    fel.GetNextPage()
    """;

app.MapPost("/mutate", (Person person) =>
{
    var scriptEngine = Python.CreateEngine();
    scriptEngine.Runtime.LoadAssembly(typeof(PageNode).Assembly);
    var scope = scriptEngine.CreateScope();
    //scope.SetVariable("obj", person);
    //scope.MutateModel = new Func<Person, Person>(p => new Person($"{p.name} the King", p.age * 2));

    scriptEngine.Execute(aScript, scope);
    //var p = scope.GetVariable<Person>("obj");
    return "";
}).WithDisplayName("Mutate model with python");

app.Run();

public class PageNode
{
    public string GetAnswer(string path)
    {
        Console.WriteLine($"GetAnswer: {path}");
        return "Ja";
    }

    public void GetPage(string pageName)
    {
        Console.WriteLine($"GetPage: {pageName}");
    }
}

public record Person(string name, int age);

