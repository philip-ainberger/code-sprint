using CodeSprint.Api.Extensions;
using CodeSprint.Api.Grpc;
using CodeSprint.Api.Interceptors;
using CodeSprint.Api.Middlewares;
using CodeSprint.Api.Services;
using CodeSprint.Api.Validators;
using FluentValidation;
using Google.Protobuf;

var builder = WebApplication.CreateBuilder(args);

// ==== services start
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCustomOptions(builder.Configuration);
builder.Services.AddCustomCors();
builder.Services.AddCustomJwtAuthentication();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionHandlingInterceptor>();
    options.Interceptors.Add<ValidationInterceptor>();
});

builder.Services
    .AddMongoDbAccess()
    .AddCustomMongoDbCollections();

builder.Services.AddScoped<IGitHubOAuthService, GitHubOAuthService>();
builder.Services.AddScoped<ISessionProviderService, SessionProviderService>();
builder.Services.AddValidatorsFromAssemblyContaining<BaseRequestValidator<IMessage>>();

builder.Services.AddHttpClient();

// ==== services end


// ==== build start
var app = builder.Build();

app.UseCors(app.Environment.EnvironmentName);

app.UseHttpsRedirection();

app.UseMiddleware<OriginRestrictionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseGrpcWeb();
app.UseHttpsRedirection();
app.MapControllers();

app.MapGrpcService<CodingService>().EnableGrpcWeb();
app.MapGrpcService<TaggingService>().EnableGrpcWeb();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

// ==== build end

public partial class Program
{
    protected Program() { }
}