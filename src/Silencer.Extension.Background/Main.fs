module Silencer.Extension.Background.Main

open System.Threading.Tasks
open Bolero
open Bolero.Html
open Microsoft.JSInterop
open Silencer.Extension.Shared.Predictor
open Silencer.Extension.Shared.Types

[<Struct>]
type Result =
    { toxic: bool
      confidence: float32 }

type MyApp() =
  inherit Component()

  do printfn "MyApp component created"

  override this.OnInitialized() = printfn "On Initialized"

  override this.OnInitializedAsync() =
    printfn "On Initialized Async"
    predictToxicity "start up the works" |> ignore
    Task.CompletedTask

  override this.Render() =
    printfn "Loaded program"
    empty

  [<JSInvokable>]
  static member NotifyFromBackgroundJs (message: string): Task<Result> =
    printfn $"I was called from the background js with the message %s{message}. Long live interop!"
    let prediction = predictToxicity message
    let normalizedConfidence = if prediction.Prediction then prediction.Probability else 1F - prediction.Probability

    Task.FromResult({ toxic = prediction.Prediction; confidence = normalizedConfidence})
