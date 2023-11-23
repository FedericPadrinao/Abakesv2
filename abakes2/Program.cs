using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();

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
/*FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
provider.Mappings[".glb"] = "model/gltf+binary";
provider.Mappings[".gltf"] = "model/gltf+json";
/*app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "models")),
    RequestPath = "models",
    ContentTypeProvider = provider
});*/
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();

app.Run();
