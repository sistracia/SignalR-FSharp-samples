namespace MoveShape.Hubs

open Microsoft.AspNetCore.SignalR

type ShapeHub() =
    inherit Hub()

    member this.MoveShape(x: int, y: int) =
        task { this.Clients.Others.SendAsync("shapeMoved", x, y) |> Async.AwaitTask |> ignore }
