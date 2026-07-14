using UMT.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load .env FIRST
DotEnv.Load();

// ✅ Add Controllers (IMPORTANT for your API)
builder.Services.AddControllers();

// ✅ Register your services
builder.Services.AddScoped<MySqlConnectionFactory>();
builder.Services.AddScoped<RawSessionsService>();

// ✅ CORS (required for React dashboard)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ✅ Swagger (optional but useful)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Middleware pipeline
app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ (For your admin logic)
app.UseAuthorization();

// ✅ Map controllers (CRITICAL)
app.MapControllers();

app.Run();
