open StockTickR
open StockTickR.Hubs
open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    builder.Services.AddSignalR().AddMessagePackProtocol() |> ignore
    builder.Services.AddSingleton<StockTicker>() |> ignore

    let app = builder.Build()
    app.UseFileServer() |> ignore
    app.UseRouting() |> ignore

    app.UseEndpoints(fun endpoints -> endpoints.MapHub<StockTickerHub>("/stocks") |> ignore)
    |> ignore

    app.Run()

    0 // Exit code
