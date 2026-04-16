using Application.Activities.Queries;
using Microsoft.EntityFrameworkCore;
using Application.Core;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
builder.Services.AddMediatR(x =>
{
    x.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>();
    x.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODA1MjQxNjAwIiwiaWF0IjoiMTc3Mzc2ODI2MCIsImFjY291bnRfaWQiOiIwMTljZmNkMzQyNWM3NDcwYWZmNTBhZTg4NWQwYjIzYyIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2t5ZDdlZWN5a3l5MDE3MzRheTJrMXlrIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.CmzmeOI5Ue5gcGUuLDgGyL3Ilb8puDi72Ng93omjPMO6sZ6HNvgQ-wF2yz7n4s-decVcyCcLVB1GEX_wQoYKaKqx60eEeUJALel8lPaiwgf7470FnTy7O8744IlwPftawNSZylOKThDpQNMyQ_yI0qIM-DopERkAAzV_QEAM5yvQs0cft1BVN8HNBVzxcOR_Ay_65xIjCoEGlXtsyywwA_AHB5V8Hi809HoKJ96oIFDvwX2rCjVOBaryHJ-We8N9Pa0PYhdk4I-r85J_9d7_z_t1ErOPh8fv0GA5I3Zob71zRQ_lvJaTwFPEC79pjHFIiQuvTXkxxl1AvmIPv43wxA";
});

builder.Services.AddAutoMapper(cfg => {}, typeof(MappingProfiles).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(x =>  x.
AllowAnyHeader().
AllowAnyMethod().
WithOrigins(
    "http://localhost:3000",
    "https://localhost:3000"));

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DbInitializer.SeedData(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration:");
}

app.Run();
