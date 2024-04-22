using CodeSprint.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ==== services start
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCustomCors();
builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddCustomOptions(builder.Configuration);

builder.Services.AddMongoClient();
// ==== services end


// ==== build start
var app = builder.Build();

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