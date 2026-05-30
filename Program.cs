//--------------------------------------------------------------------
//            МедВызов: регистрация заявок вызова СМП
// -------------------------------------------------------------------
// Пример/тестовое демо-приложение медицинской информационной системы
// Генерация тестовых данных и API для доступа клиента.
// Сервер ASP.NET С# 
// Vit Vatkov   vivat-067@mail.ru


using MedicalCallServer.Helpers;
using MedicalCallServer.Services;
using Microsoft.OpenApi;
using System.Text;


Console.OutputEncoding = Encoding.UTF8;

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

Console.WriteLine($" МедВызов: регистрация заявок вызова СМП. Версия: {AppVersionInfo.GetMajorVersion()} ({AppVersionInfo.GetBuildVersion()})");
Console.WriteLine("--------------------------------------------------");
Console.WriteLine(" Демо-версия ПРОТОТИПА МЕДИЦИНСКОЙ ИС для hh.ru");
Console.WriteLine();
Console.WriteLine($" *Server:   {app.Configuration["Urls"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS")}");
Console.WriteLine($" **WorkDir:  {Environment.CurrentDirectory}");
Console.WriteLine($" **Platform: {AppVersionInfo.GetPlatformInfo()}");
Console.WriteLine();

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
