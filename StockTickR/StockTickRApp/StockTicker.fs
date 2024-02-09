namespace StockTickR

open System
open System.Collections.Concurrent
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Threading

type MarketState =
    | Closed = 0
    | Open = 1
    | Unknown = 2

type StockTicker() as self =

    let _marketStateLock: SemaphoreSlim = new SemaphoreSlim(1, 1)

    let _updateStockPricesLock: SemaphoreSlim = new SemaphoreSlim(1, 1)

    let _stocks: ConcurrentDictionary<string, Stock> =
        new ConcurrentDictionary<string, Stock>()

    let _subject: Subject<Stock> = new Subject<Stock>()

    // Stock can go up or down by a percentage of this factor on each change
    let _rangePercent: double = 0.002

    let _updateInterval: TimeSpan = TimeSpan.FromMilliseconds(250)
    let _updateOrNotRandom: Random = new Random()

    let mutable _timer: Timer option = None

    [<VolatileField>]
    let mutable _updatingStockPrices: bool = false

    [<VolatileField>]
    let mutable _marketState: MarketState = MarketState.Closed

    do self.LoadDefaultStocks() |> ignore

    member this.MarketState
        with get () = _marketState
        and private set (value: MarketState) = _marketState <- value

    member this.GetAllStocks() = _stocks.Values

    member this.StreamStocks() = _subject.AsObservable()

    member this.OpenMarket() =
        async {
            _marketStateLock.WaitAsync() |> Async.AwaitTask |> ignore

            try
                if this.MarketState <> MarketState.Open then
                    let timerCallback = fun _ -> this.UpdateStockPrices |> ignore
                    _timer <- Some(new Timer(timerCallback, null, _updateInterval, _updateInterval))

                    this.MarketState <- MarketState.Open
            finally
                this.MarketState <- MarketState.Unknown
                _marketStateLock.Release() |> ignore

            return this.MarketState
        }

    member this.CloseMarket() =
        async {
            _marketStateLock.WaitAsync() |> Async.AwaitTask |> ignore

            try
                if this.MarketState = MarketState.Open then
                    match _timer with
                    | Some time -> time.Dispose()
                    | _ -> ()

                    this.MarketState <- MarketState.Closed
            finally
                this.MarketState <- MarketState.Unknown
                _marketStateLock.Release() |> ignore

            return this.MarketState
        }

    member this.Reset() =
        async {
            _marketStateLock.WaitAsync() |> Async.AwaitTask |> ignore

            try
                if this.MarketState <> MarketState.Closed then
                    raise (InvalidOperationException("Market must be closed before it can be reset."))

                this.LoadDefaultStocks() |> ignore
            finally
                _marketStateLock.Release() |> ignore
        }

    member this.LoadDefaultStocks() =
        async {
            _stocks.Clear()

            let stocks =
                [ new Stock(Symbol = "MSFT", Price = 107.56m)
                  new Stock(Symbol = "AAPL", Price = 215.49m)
                  new Stock(Symbol = "GOOG", Price = 1221.16m) ]

            stocks |> List.iter (fun stock -> _stocks.TryAdd(stock.Symbol, stock) |> ignore)
        }

    member this.UpdateStockPrices() =
        async {
            // This function must be re-entrant as it's running as a timer interval handler
            _updateStockPricesLock.WaitAsync() |> Async.AwaitTask |> ignore

            try
                if not _updatingStockPrices then
                    _updatingStockPrices <- true

                _stocks.Values
                |> Seq.iter (fun stock ->
                    this.TryUpdateStockPrice(stock)
                    _subject.OnNext(stock))

                _updatingStockPrices <- false
            finally
                _updateStockPricesLock.Release() |> ignore
        }

    member this.TryUpdateStockPrice(stock: Stock) =
        // Randomly choose whether to udpate this stock or not
        let r = _updateOrNotRandom.NextDouble()

        if r > 0.1 then
            ()

        let random = new Random(int (Math.Floor(stock.Price)))
        let percentChange = random.NextDouble() * _rangePercent
        let pos = random.NextDouble() > 0.51
        let change = Math.Round(stock.Price * decimal (percentChange), 2)
        let foo = if pos then change else -change

        stock.Price <- stock.Price + change
