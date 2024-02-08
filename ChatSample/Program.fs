open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.SignalR

type ChatHub() =
    inherit Hub()

    member this.Send(name: string, message: string) =
        this.Clients.All.SendAsync("broadcastMessage", name, message)


[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    builder.Services.AddSignalR() |> ignore

    builder.Services.AddCors(fun options ->
        options.AddPolicy(
            "CorsPolicy",
            fun builder ->
                builder
                    .WithOrigins("http://localhost:5000")
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST")
                    .AllowCredentials()
                |> ignore
        ))
    |> ignore

    let app = builder.Build()

    app.UseCors("CorsPolicy") |> ignore
    app.UseFileServer() |> ignore
    app.UseRouting() |> ignore

    app.UseEndpoints(fun endpoints -> endpoints.MapHub<ChatHub>("/chat") |> ignore)
    |> ignore

    app.Run()

    0 // Exit code
