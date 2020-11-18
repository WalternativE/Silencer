module Silencer.Extension.Frontend.Main

open Elmish
open Bolero
open Bolero.Html
open Bolero.Remoting.Client
open Microsoft.JSInterop

type Model =
  { LastMessage: string option
    TextToPredict: string }

let initModel =
  let model =
    { LastMessage = None
      TextToPredict = "" }

  model, Cmd.none

type Message =
  | SetTextToPredict of text: string
  | PredictToxicity
  | Error of exn

let update (js: IJSRuntime) message model =
  match message with
  | SetTextToPredict text ->
    { model with TextToPredict = text }, Cmd.none
  | PredictToxicity ->
      let cmd =
        Cmd.OfJS.attempt js "sendGreeting" [| model.TextToPredict |] Error

      model, cmd
  | Error exn ->
      printfn "%A" exn
      model, Cmd.none

let view model dispatch =
  div [] [
    div [] [
      textarea [ bind.input.string model.TextToPredict (SetTextToPredict >> dispatch) ] []
    ]
    div [] [
      button [ on.click
                 (fun _ ->
                   printfn "Clicked"
                   dispatch PredictToxicity) ] [
        text "Do a prediction!"
      ]
    ]
    match model.LastMessage with
    | None -> empty
    | Some message -> div [] [ text message ]
  ]

type MyApp() =
  inherit ProgramComponent<Model, Message>()

  override this.Program =
    Program.mkProgram (fun _ -> initModel) (update this.JSRuntime) view
