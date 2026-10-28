using AuthLab;
using BasicApp.Authentication;
using BasicApp.Endpoints;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<UserService>();
builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", _ => { });
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapBasicEndpoints();
app.Run();
