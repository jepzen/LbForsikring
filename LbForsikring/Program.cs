using LbForsikring;
using LbForsikring.Integrations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddHttpClient<IDstService, DstService>();
builder.Services.AddHttpClient<ICvrService, CvrService>(); 


builder.Services.AddEndpoints();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();
