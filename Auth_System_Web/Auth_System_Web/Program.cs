using Auth_System_Web.Models;
using Auth_System_Web.Services;
using Firebase.Auth;
using FirebaseAdmin;
using Google.Api;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

//Singleton

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.GetApplicationDefault(),
    ProjectId = "userface-b47eb"
});

// Agregar Firestore a los servicios
builder.Services.AddSingleton(FirestoreDb.Create("userface-b47eb"));

// Configurar FirebaseAuthProvider usando la API web de Firebase para autenticación
builder.Services.AddSingleton(provider =>
    new FirebaseAuthProvider(new FirebaseConfig(builder.Configuration["settings:FirebaseWebApi"])));

if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile("application_default_credentials.json"),
        ProjectId = "userface-b47eb"
    });
}


builder.Services.AddSingleton<IUserModel, UserModel>();
builder.Services.AddSingleton<IScheduleModel, ScheduleModel>();
builder.Services.AddSingleton<IUtilitiesModel, UtilitiesModel>();   



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
