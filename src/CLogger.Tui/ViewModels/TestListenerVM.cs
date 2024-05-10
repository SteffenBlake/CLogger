using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using CLogger.Common.Enums;
using CLogger.Common.Messages;
using CLogger.Common.Model;
using CLogger.Tui.Models;
using CLogger.Tui.ViewModels;
using Microsoft.Extensions.Logging;

namespace src.CLogger.Tui.ViewModels;

public class TestListenerVM(
    ILogger<TestListenerVM> logger,
    ModelState modelState,
    CliOptions cliOptions
) : IViewModel
{
    private ILogger<TestListenerVM> Logger { get; } = logger;
    private ModelState ModelState { get; } = modelState;
    private CliOptions CliOptions { get; } = cliOptions;

    private readonly JsonSerializerOptions _serializeOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    public async Task BindAsync(CancellationToken cancellationToken)
    {
        await ListenToAdaptersAsync(cancellationToken);
    }


    private async Task ListenToAdaptersAsync(
        CancellationToken cancellationToken
    )
    {
        Logger.LogInformation(
            "Starting up listening on {ip}:{port}",
            CliOptions.Domain, ModelState.MetaInfo.Port.Value
        );

        var ip = IPAddress.Parse(CliOptions.Domain);
        using var server = new TcpListener(ip, CliOptions.Port);
        server.Start();
       
        var actualPort = ((IPEndPoint)server.LocalEndpoint).Port;
        await ModelState.MetaInfo.Port.WriteAsync(actualPort, cancellationToken);
        await ModelState.MetaInfo.State.WriteAsync(AppState.Idle, cancellationToken);

        List<Task> pendingClients = [];
        var nextClientTask = 
            server.AcceptTcpClientAsync(cancellationToken).AsTask();
        while (!cancellationToken.IsCancellationRequested)
        {
            var allTasks = pendingClients.Append(nextClientTask);
            var next = await Task.WhenAny(allTasks);
            if (next == nextClientTask)
            {
                pendingClients.Add(
                    HandleClientAsync(nextClientTask.Result, cancellationToken)
                );
                nextClientTask = server.AcceptTcpClientAsync(cancellationToken).AsTask();
            }
            else
            {
                pendingClients.Remove(next);
            }
        }
    }   

    private async Task HandleClientAsync(
        TcpClient client, CancellationToken cancellationToken
    )
    {
        var guid = Guid.NewGuid();
        Logger.LogInformation(
            "New client connected, listening to incoming Messages, {guid}", guid
        );

        var reader = new StreamReader(client.GetStream()) ;

        while (
            !cancellationToken.IsCancellationRequested
        )
        {
            var next = await reader.ReadLineAsync(cancellationToken) 
                ?? throw new NullReferenceException("Unexpected null response from client");

            var msg = JsonSerializer.Deserialize<MessageBase>(next, _serializeOptions)!;

            var isFinal = await msg.InvokeAsync(ModelState, cancellationToken);
            if (isFinal)
            {
                break;
            }
        }

        Logger.LogInformation("Client finished, no more incoming Messages, {guid}", guid);
    }
}
