// Backend de VibeScreen. Escucha solo en loopback y exige token de sesión.
var token = Environment.GetEnvironmentVariable("VIBESCREEN_TOKEN");
if (string.IsNullOrWhiteSpace(token))
{
    Console.Error.WriteLine("VIBESCREEN_TOKEN no definido. El backend debe lanzarlo la aplicación de escritorio.");
    return 1;
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins("http://localhost:5173")   // solo Vite en desarrollo
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();
app.UseCors();

// Todas las rutas /api requieren el token (comparación en tiempo constante).
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Request.Method != "OPTIONS")
    {
        var sent = ctx.Request.Headers["X-Vibe-Token"].ToString();
        var ok = System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
            System.Text.Encoding.UTF8.GetBytes(sent),
            System.Text.Encoding.UTF8.GetBytes(token));
        if (!ok) { ctx.Response.StatusCode = StatusCodes.Status401Unauthorized; return; }
    }
    await next();
});

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.1.0",
    runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription
}));

app.Run();
return 0;
