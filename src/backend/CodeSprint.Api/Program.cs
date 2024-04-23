using CodeSprint.Api.Extensions;
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

builder.Services.AddCustomCors();
builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddCustomOptions(builder.Configuration);

builder.Services
    .AddMongoDbAccess()
    .AddCustomMongoDbCollections();

builder.Services.AddScoped<ISessionProviderService, SessionProviderService>();
builder.Services.AddValidatorsFromAssemblyContaining<BaseRequestValidator<IMessage>>();

// ==== services end


// ==== build start
var app = builder.Build();

app.UseCors("AllowAngularDev");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseGrpcWeb();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
// ==== build end