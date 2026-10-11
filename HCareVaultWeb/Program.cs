using HCareVaultWeb.Components;
using HCareVault.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddDbContextFactory<HCareDBContext>(o => o.UseSqlite("Data Source=medicines.db"));

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<IDbContextFactory<HCareDBContext>>()
         .CreateDbContext().Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
