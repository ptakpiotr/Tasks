using AspNetCore.Scalar;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultSignInScheme = AppConstants.AuthenticationSchemeName;
    opts.DefaultAuthenticateScheme = AppConstants.AuthenticationSchemeName;
    opts.DefaultChallengeScheme = AppConstants.AuthenticationSchemeName;
    opts.DefaultScheme = AppConstants.AuthenticationSchemeName;
    opts.RequireAuthenticatedSignIn = false;
})
    .AddCookie(AppConstants.AuthenticationSchemeName, opts =>
    {
        opts.Cookie.Name = "app-cookie";
    });

builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("admin", pb =>
    {
        pb.RequireAuthenticatedUser()
            .RequireClaim("admin", "true");
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<TasksApi.Options.TokenOptions>()
    .Bind(builder.Configuration.GetSection("TokenOptions"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddMassTransit(x =>
{
    // Register consumers
    x.AddConsumer<DeleteUserConsumer>();

    // Configure the in-memory transport
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddDbContext<IdentityDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnString"));
});

builder.Services.AddDbContext<DataDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DataConnString"));
});

builder.Services.AddDataProtection();
builder.Services.AddScoped<IPasswordHasher<AppUser>>((_) => new PasswordHasher<AppUser>());
builder.Services.AddHttpContextAccessor();
builder.Services.AddOutputCache();

builder.Services.AddFluentValidation(opts =>
{
    opts.RegisterValidatorsFromAssemblyContaining<Program>();
});
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IAppIdentityService, AppIdentityService>();
builder.Services.AddScoped<ITasksService, TasksService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();

app.UseScalar(options =>
{
    options.RoutePrefix = "scalar";
    options.UseTheme(Theme.DeepSpace);
});

app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
