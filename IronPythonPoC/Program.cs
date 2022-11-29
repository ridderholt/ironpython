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
    def MutateModel(person):
        person.name = person.name + ' the King'
        person.age = person.age*2
        return person

    MutateModel(obj)
    """;

app.MapPost("/mutate", (Person person) =>
{
    var scriptEngine = Python.CreateEngine();
    var scope = scriptEngine.CreateScope();
    scope.SetVariable("obj", person);
    scriptEngine.Execute(pyScript, scope);
    var p = scope.GetVariable<Person>("obj");
    return p;
}).WithDisplayName("Mutate model with python");

app.Run();

public record Person(string name, int age);

