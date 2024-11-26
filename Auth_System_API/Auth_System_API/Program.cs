using Auth_System_API.Models;
using Auth_System_API.Services;
using Firebase.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var config = builder.Configuration;
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

//singleton

builder.Services.AddSingleton<IUtilities, Utilities>();

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.GetApplicationDefault(),
    ProjectId = "userface-b47eb"
});

builder.Services.AddSingleton(FirestoreDb.Create("userface-b47eb"));


builder.Services.AddSingleton<FirebaseAuthProvider>(provider =>
    new FirebaseAuthProvider(new FirebaseConfig(builder.Configuration["settings:FirebaseWebApi"])));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
