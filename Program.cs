using BloemFinder.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS for Flutter app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutterApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:52811",
            "http://localhost:5000",
            "https://localhost:5001"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Register SupabaseService
builder.Services.AddHttpClient<SupabaseService>();
builder.Services.AddScoped<SupabaseService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFlutterApp");
app.UseAuthorization();
app.MapControllers();

app.Run();