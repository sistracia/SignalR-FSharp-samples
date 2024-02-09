namespace StockTickR.Hubs

open StockTickR
open Microsoft.AspNetCore.SignalR

type StockTickerHub(stockTicker: StockTicker) =
    inherit Hub()
    let _stockTicker = stockTicker

    member this.GetAllStocks() = _stockTicker.GetAllStocks()

    member this.StreamStocks() =
        Extentions.ObservableExtensions.AsChannelReader(_stockTicker.StreamStocks(), 10)


    member this.GetMarketState() = _stockTicker.MarketState.ToString()

    member this.OpenMarket() =
        async {
            _stockTicker.OpenMarket()
            |> Async.RunSynchronously
            |> this.BroadcastMarketStateChange
            |> ignore
        }

    member this.CloseMarket() =
        async { _stockTicker.CloseMarket() |> ignore }


    member this.Reset() =
        async {
            (_stockTicker.Reset()) |> Async.RunSynchronously |> ignore
            this.BroadcastMarketReset() |> Async.RunSynchronously |> ignore
        }

    member private this.BroadcastMarketStateChange(marketState: MarketState) =
        async {
            match marketState with
            | MarketState.Open -> this.Clients.All.SendAsync("marketOpened")
            | MarketState.Closed -> this.Clients.All.SendAsync("marketClosed")
            | _ -> this.Clients.Caller.SendAsync("unknown")
            |> Async.AwaitTask
            |> ignore
        }

    member private this.BroadcastMarketReset() =
        async { this.Clients.All.SendAsync("marketReset") |> Async.AwaitTask |> ignore }
