namespace StockTickR.Hubs

open StockTickR
open Microsoft.AspNetCore.SignalR
open Extensions

type StockTickerHub(stockTicker: StockTicker) =
    inherit Hub()
    let _stockTicker = stockTicker

    member this.GetAllStocks() = _stockTicker.GetAllStocks()

    member this.StreamStocks() =
        _stockTicker.StreamStocks().AsChannelReader(10)

    member this.GetMarketState() = _stockTicker.MarketState.ToString()

    member this.OpenMarket() =
        async {
            let! marketState = _stockTicker.OpenMarket()
            do! this.BroadcastMarketStateChange marketState
        }

    member this.CloseMarket() =
        async {
            let! marketState = _stockTicker.CloseMarket()
            do! this.BroadcastMarketStateChange marketState
        }

    member this.Reset() =
        async {
            do! (_stockTicker.Reset())
            do! this.BroadcastMarketReset()
        }

    member private this.BroadcastMarketStateChange(marketState: MarketState) =
        async {
            do!
                (match marketState with
                 | MarketState.Open -> this.Clients.All.SendAsync("marketOpened")
                 | MarketState.Closed -> this.Clients.All.SendAsync("marketClosed")
                 | _ -> this.Clients.Caller.SendAsync("unknown")
                 |> Async.AwaitTask)
        }

    member private this.BroadcastMarketReset() =
        async { do! (this.Clients.All.SendAsync("marketReset") |> Async.AwaitTask) }
