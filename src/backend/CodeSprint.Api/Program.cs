using CodeSprint.Api.Extensions;
using CodeSprint.Api.Grpc;
using CodeSprint.Api.Interceptors;
using CodeSprint.Api.Validators;
using FluentValidation;
using Google.Api;
using Google.Protobuf;

var builder = WebApplication.CreateBuilder(args);

// ==== services start
builder.Configuration.ValidateRequiredFrameworkSettings();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

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

builder.Services.AddCustomServices();
builder.Services.AddValidatorsFromAssemblyContaining<BaseRequestValidator<IMessage>>();

builder.Services.AddHttpClient();

// ==== services end


// ==== build start
var app = builder.Build();

app.UseCors(app.Environment.EnvironmentName);

app.UseRouting();

app.UseCustomHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseGrpcWeb();
app.MapControllers();

app.MapGrpcService<CodingService>().RequireAuthorization().EnableGrpcWeb();
app.MapGrpcService<TaggingService>().RequireAuthorization().EnableGrpcWeb();

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