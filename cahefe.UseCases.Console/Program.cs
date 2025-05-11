// See https://aka.ms/new-console-template for more information
using cahefe.UseCases;
using cahefe.UseCases.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
//  Incluindo serviços
//  ... logging
services.AddLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Trace);
    builder.AddDebug();
});

//  P1) Identifica o tipo de dados que será compartilhado entre os casos de uso...
services.AddScoped<Bag<RequestInfo, ResponseInfo>>();
//  P2) Registra o serviço de caso de uso dinâmico...
services.AddScoped<Pipeline<RequestInfo, ResponseInfo>>();
//  P4) Registra os casos de uso (lidam com dados compartilhados, quando necessário)...
services.AddTransient<UseCasePreparation>();
services.AddTransient<UseCasePhase1>();
services.AddTransient<UseCasePhase21>();
services.AddTransient<UseCasePhase22>();
services.AddTransient<UseCasePhase23>();
services.AddTransient<UseCasePhase31>();
services.AddTransient<UseCasePhase32>();
services.AddTransient<ResponseUseCase>();

using var serviceProvider = services.BuildServiceProvider();
//  P5) Representação dos dados recebidos por uma requisição...
var userInfo = new RequestInfo { Name = "Init", Value = 0 };

//  P6) Execução do caso de uso dinâmico...
var pileline = serviceProvider.GetRequiredService<Pipeline<RequestInfo, ResponseInfo>>();

pileline
    .AppendPhase("Preparation")
    .AppendStep<RequestInfo, ResponseInfo, UseCasePreparation>(name: "Preparation 1.1", retries: 1)
    .AppendPhase("Phase 1")
    .AppendStep<RequestInfo, ResponseInfo, UseCasePhase1>(name: "Phase 1.1")
    .AppendPhase("Phase 2")
    .AppendStep<RequestInfo, ResponseInfo, UseCasePhase21>(name: "Phase 2.1")
    .AppendStep<RequestInfo, ResponseInfo, UseCasePhase22>(name: "Phase 2.2")
    .AppendStep<RequestInfo, ResponseInfo, UseCasePhase23>()
    .AppendPhase("Phase 3")
    .AppendStep<RequestInfo, ResponseInfo, UseCasePhase31>(name: "Phase 3.1")
    .AppendStep<RequestInfo, ResponseInfo, UseCasePhase32>(name: "Phase 3.2")
    .AppendPhase("Empty Phase 4")
    .AppendPhase("Response")
    .AppendStep<RequestInfo, ResponseInfo, ResponseUseCase>(name: "Response 1.1");

var x = await pileline.Execute(userInfo);
Console.WriteLine("Finished: Success? {0} - Erros: {1} - Info: {2} / {3}", x.Success, x.Fails.Length, x.Response?.ResponseNameValue, x.Response?.ResponseValue);
pileline.Stats();



