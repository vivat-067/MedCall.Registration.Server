using MedicalCallServer.Services;
using Microsoft.OpenApi;


var exePath = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
Directory.SetCurrentDirectory(exePath);

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<ITestDataService, TestDataService>();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Medical Assistance Call API",
        Description = "API для управления вызовами службы медицинской помощи"
    });
});

var app = builder.Build();

Console.WriteLine($" *Server: {app.Configuration["Urls"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS")}");
Console.WriteLine($" **WorkDir: {Environment.CurrentDirectory}");

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medical Assistance Call API v1");
    c.RoutePrefix = string.Empty; 
});
//}

//app.UseHttpsRedirection();
//app.UseAuthorization();

app.UseRouting();
app.MapControllers();

app.Run();
