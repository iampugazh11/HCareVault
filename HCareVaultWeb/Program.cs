using HCareVaultWeb.Components;
using HCareVault.Database;
using HCareVault.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddDbContextFactory<HCareDBContext>(o => o.UseSqlite("Data Source=medicines.db"));
builder.Services.AddSingleton<FileStorage>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<HCareDBContext>>().CreateDbContext();
    SchemaUpgrader.Upgrade(db);
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

// Uploaded medicine photos, prescriptions and scan reports.
var contentTypes = new FileExtensionContentTypeProvider();
app.MapGet("/files/{name}", (string name, FileStorage storage) =>
{
    var path = storage.GetPath(name);
    if (path is null) return Results.NotFound();
    return Results.File(path, contentTypes.TryGetContentType(path, out var type) ? type : "application/octet-stream");
});
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
