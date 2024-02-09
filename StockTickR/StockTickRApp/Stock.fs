namespace StockTickR

open System

type Stock() =
    let mutable _price: decimal = 0m

    let mutable symbol: string = ""

    let mutable dayOpen: decimal = 0m

    let mutable dayLow: decimal = 0m

    let mutable dayHigh: decimal = 0m

    let mutable lastChange: decimal = 0m

    member this.Symbol
        with get () = symbol
        and set (value: string) = symbol <- value

    member this.DayOpen
        with get () = dayOpen
        and private set (value: decimal) = dayOpen <- value

    member this.DayLow
        with get () = dayLow
        and private set (value: decimal) = dayLow <- value

    member this.DayHigh
        with get () = dayHigh
        and private set (value: decimal) = dayHigh <- value

    member this.LastChange
        with get () = lastChange
        and private set (value: decimal) = lastChange <- value

    member this.Change = this.Price - this.DayOpen

    member this.PercentChange = double (Math.Round(this.Change / this.Price, 4))

    member this.Price
        with get () = _price
        and set (value: decimal) =
            if _price.Equals(value) then
                ()

            this.LastChange <- value - _price
            _price <- value

            if this.DayOpen.Equals 0m then
                this.DayOpen <- _price

            if _price.CompareTo this.DayLow < 0 || this.DayLow.Equals 0m then
                this.DayLow <- _price

            if _price.CompareTo this.DayHigh > 0 then
                this.DayHigh <- _price
