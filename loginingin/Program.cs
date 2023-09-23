using loginingin.Models;
using System.Web.Http;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(
  options => options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader()
);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

var UsersList = new List<LoginDTO>
{
    new LoginDTO {Login = "Admin", Email = "admin@admin.admin", Password = "Admin"},
    new LoginDTO {Login = "Lasgra", Email = "lasgra12@gmail.com", Password = "Czaszunia"},
};

app.MapGet("/Users", () => {
    return UsersList;
});

app.MapGet("/Users/{Login}", (string Login) => {
    var ULogin = UsersList.Find(U => U.Login == Login);
    if (ULogin is null)
        return Results.NotFound();
    return Results.Ok(ULogin);
});

app.MapPost("/Users", ([FromBody] LoginDTO Login) =>
{
    var ULogin = UsersList.Find(U => U.Login == Login.Login);
    if (ULogin is null)
    {
        UsersList.Add(Login);
        return Results.Ok();
    }
    else
    {
        return Results.Conflict();
    }
});

app.MapPut("/Users/{Login}", (LoginDTO InputLogin, string SettingChange, string Change) => {
    var ULogin = UsersList.Find(U => U.Login == InputLogin.Login);
    if (InputLogin.Email == ULogin.Email && InputLogin.Password  == ULogin.Password)
    {
        if (SettingChange == "Password")
        {
            if (Change != ULogin.Password)
            {
                ULogin.Password = Change;
            }
            else
            {
                return Results.Problem();
            }
        }
        if (SettingChange == "Email")
        {
            if (Change != ULogin.Email)
            {
                ULogin.Email = Change;
            }
            else
            {
                return Results.Problem();
            }
        }
        return Results.Ok();
    }
    else
    {
        return Results.Conflict();
    }
});

app.MapDelete("/Users/{Login}", (string Login, string Password, string Email) => {
    var ULogin = UsersList.Find(U => U.Login == Login);
    var UPassword = UsersList.Find(U => U.Password == Password);
    var UEmail = UsersList.Find(U => U.Email == Email);
    if (ULogin is null || UPassword is null || UEmail is null)
        return Results.NotFound();
    UsersList.Remove(ULogin);
    return Results.Ok();

});


app.Run();