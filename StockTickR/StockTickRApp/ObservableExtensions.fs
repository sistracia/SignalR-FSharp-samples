module Extensions

open System
open System.Threading.Channels
open System.Runtime.CompilerServices

[<Extension>]
type ObservableExtensions =
    [<Extension>]
    static member inline AsChannelReader<'T> (observable: IObservable<'T>, ?maxBufferSize0: int) =
        // This sample shows adapting an observable to a ChannelReader without
        // back pressure, if the connection is slower than the producer, memory will
        // start to increase.

        // If the channel is bounded, TryWrite will return false and effectively
        // drop items.

        // The other alternative is to use a bounded channel, and when the limit is reached
        // block on WaitToWriteAsync. This will block a thread pool thread and isn't recommended and isn't shown here.
        let channel =
            match maxBufferSize0 with
            | Some maxBufferSize1 -> Channel.CreateBounded<'T> maxBufferSize1
            | None -> Channel.CreateUnbounded<'T>()


        let onNext value = channel.Writer.TryWrite value |> ignore

        let onError ex = printfn "Error occurred:"

        let onCompleted () = printfn "Sequence completed"

        let disposable = observable.Subscribe(onNext, onError, onCompleted)

        let dispose _ = disposable.Dispose()

        // Complete the subscription on the reader completing
        channel.Reader.Completion.ContinueWith dispose |> ignore

        channel.Reader
