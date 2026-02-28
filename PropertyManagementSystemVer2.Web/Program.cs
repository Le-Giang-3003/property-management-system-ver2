using PropertyManagementSystemVer2.BLL;
using PropertyManagementSystemVer2.BLL.Settings;
using PropertyManagementSystemVer2.DAL;
using PropertyManagementSystemVer2.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Dependency injection (phải đăng ký trước Build())
builder.Services.AddInfrastructure(builder.Configuration); // DAL Layer
builder.Services.AddBusinessLogic(builder.Configuration); // BLL Layer

// Cloudinary Configuration
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

// Background Service: Auto tạo hóa đơn hàng tháng
builder.Services.AddHostedService<MonthlyPaymentGeneratorService>();

// Configure Authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Error";
        options.Cookie.Name = "PropertyMS.Auth";
    });

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
