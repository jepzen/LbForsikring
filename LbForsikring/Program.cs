using LbForsikring.Features;
using LbForsikring.Integrations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddScoped<ICvrService, CvrService>();
builder.Services.AddScoped<IDstService, DstService>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGetCompanyDetailsEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();

app.Run();
