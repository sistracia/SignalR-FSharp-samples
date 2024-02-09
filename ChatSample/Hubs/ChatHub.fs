namespace ChatSample.Hubs

open Microsoft.AspNetCore.SignalR

type ChatHub() =
    inherit Hub()

    member this.Send(name: string, message: string) =
        this.Clients.All.SendAsync("broadcastMessage", name, message)
