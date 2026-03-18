using System.Net;
using MendSync.API.Middleware;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Data;
using MendSync.Infrastructure.HttpClients;
using MendSync.Infrastructure.Services;
using MendSync.Infrastructure.Token;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Oracle.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ──────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext());

// ── Controllers + Swagger ────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MendSync API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token from POST /api/auth/login"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

// ── Oracle DbContext ──────────────────────────────────────────────────────────
builder.Services.AddDbContext<MendSyncDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("Oracle")
        ?? throw new InvalidOperationException("Connection string 'Oracle' not configured")));

// ── Sync State (singleton) ────────────────────────────────────────────────────
builder.Services.AddSingleton<SyncState>();

// ── Token Store (singleton) ───────────────────────────────────────────────────
builder.Services.AddSingleton<TokenStore>();

// ── HttpClient com Polly retry ────────────────────────────────────────────────
builder.Services.AddHttpClient<MendApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Mend:BaseUrl"]
        ?? throw new InvalidOperationException("Mend:BaseUrl is not configured"));
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromMinutes(2);
})
.AddPolicyHandler(GetRetryPolicy());

// ── Services (Scoped) ─────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IFindingsService, FindingsService>();
builder.Services.AddScoped<IScansService, ScansService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IGroupsService, GroupsService>();
builder.Services.AddScoped<ILabelsService, LabelsService>();
builder.Services.AddScoped<ISyncService, SyncService>();

// ── Health Check ──────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddCheck("mend-api", () =>
    {
        // Basic check — full connectivity check is done via GET /health
        return HealthCheckResult.Healthy("MendSync is running");
    });

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TokenRefreshMiddleware>();
app.UseMiddleware<SyncMiddleware>();

app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent);
    };
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MendSync API v1"));
}

app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// ── Polly retry policy ────────────────────────────────────────────────────────
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
