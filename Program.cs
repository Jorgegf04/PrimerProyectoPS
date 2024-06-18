using BlazorSimuladorJGF.Models;
using BlazorSimuladorJGF.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Registrar servicios personalizados
builder.Services.AddScoped<ICampaignInterface, CampaignService>();
builder.Services.AddScoped<CampaignService>(); // Añadir esta línea
builder.Services.AddScoped<IEmailInterface, EmailService>();
builder.Services.AddScoped<IPhishingInterface, PhishingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<EmailTemplateService>();
builder.Services.AddScoped<ReportingService>();
builder.Services.AddScoped<PdfReportGenerator>();
builder.Services.AddScoped<ExcelReportGenerator>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();