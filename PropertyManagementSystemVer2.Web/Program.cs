using PropertyManagementSystemVer2.BLL;
using PropertyManagementSystemVer2.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Dependency injection (phải đăng ký trước Build())
builder.Services.AddInfrastructure(builder.Configuration); // DAL Layer
builder.Services.AddBusinessLogic(); // BLL Layer

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
