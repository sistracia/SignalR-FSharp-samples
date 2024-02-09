namespace MoveShape.Hubs

open Microsoft.AspNetCore.SignalR

type ShapeHub() =
    inherit Hub()

    member this.MoveShape(x: int, y: int) =
        this.Clients.Others.SendAsync("shapeMoved", x, y)
