using Domain.Interfaces.Generics;
using Domain.Interfaces.ICategoria;
using Domain.Interfaces.IDespesa;
using Domain.Interfaces.InterfaceServicos;
using Domain.Interfaces.ISistemaFinanceiro;
using Domain.Interfaces.IUsuarioSistemaFinanceiro;
using Domain.Servicos;
using Entities.Entidades;
using Infra.Configuracao;
using Infra.Repositorio;
using Infra.Repositorio.Generics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Token;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Criando a autenticação direto pelo Swagger, basta digitar "Bearer códigoToken"
builder.Services.AddSwaggerGen(c =>

{

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API_DDD_2023", Version = "v1" });



    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme

    {

        Description =

            "JWT Authorization Header - utilizado com Bearer Authentication.\r\n\r\n" +

            "Digite 'Bearer' [espaço] e então seu token no campo abaixo.\r\n\r\n" +

            "Exemplo (informar sem as aspas): 'Bearer 12345abcdef'",

        Name = "Authorization",

        In = ParameterLocation.Header,

        Type = SecuritySchemeType.ApiKey,

        Scheme = "Bearer",

        BearerFormat = "JWT",

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

                        new string[] {}

                    }

                });

});


// Config com o BD
builder.Services.AddDbContext<ContextBase>(options =>
               options.UseSqlServer(
                   builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ContextBase>();

// INTERFACE E REPOSITORIO
builder.Services.AddSingleton(typeof(InterfaceGenerics<>), typeof(RepositoryGenerics<>));
builder.Services.AddSingleton<InterfaceCategoria, RepositorioCategoria>();
builder.Services.AddSingleton<InterfaceDespesa, RepositorioDespesa>();
builder.Services.AddSingleton<InterfaceSistemaFinanceiro, RepositorioSistemaFinanceiro>();
builder.Services.AddSingleton<InterfaceUsuarioSistemaFinanceiro, RepositorioUsuarioSistemaFinanceiro>();


// SERVIÇO DOMINIO
builder.Services.AddSingleton<ICategoriaServico, CategoriaServico>();
builder.Services.AddSingleton<IDespesaServico, DespesaServico>();
builder.Services.AddSingleton<ISistemaFinanceiroServico, SistemaFinanceiroServico>();
builder.Services.AddSingleton<IUsuarioSistemaFinanceiroServico, UsuarioSistemaFinanceiroServico>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(option =>
             {
                 option.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = false,
                     ValidateAudience = false,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,

                     ValidIssuer = "Teste.Securiry.Bearer",
                     ValidAudience = "Teste.Securiry.Bearer",
                     IssuerSigningKey = JwtSecurityKey.Create("Secret_Key-12345678")
                 };

                 option.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                         return Task.CompletedTask;
                     },
                     OnTokenValidated = context =>
                     {
                         Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                         return Task.CompletedTask;
                     }
                 };
             });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var devClient = "http://localhost:4200";

app.UseCors(x =>
x.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader()
.WithOrigins(devClient));

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
