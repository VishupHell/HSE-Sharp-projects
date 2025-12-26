using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddCors();


builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();


app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

app.UseCors(builder => builder
    .WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod());


await app.UseOcelot();

app.Run();