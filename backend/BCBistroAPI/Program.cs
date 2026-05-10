var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Services.AddControllers();
builder.Services.AddScoped<ConexaoDapper>();

//Controllers
builder.Services.AddScoped<VendaController>();
builder.Services.AddScoped<ProdutoController>();
builder.Services.AddScoped<CatalogoController>();

//Services
builder.Services.AddScoped<VendaService>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<CatalogoService>();

//Repositories
builder.Services.AddScoped<VendaRepository>();
builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<CatalogoRepository>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
