namespace ChatSample.Hubs

open Microsoft.AspNetCore.SignalR

type ChatHub() =
    inherit Hub()

    member this.Send(name: string, message: string) =
        task {
            this.Clients.All.SendAsync("broadcastMessage", name, message)
            |> Async.AwaitTask
            |> ignore
        }
