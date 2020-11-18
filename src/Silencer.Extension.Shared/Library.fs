module Silencer.Extension.Shared.Predictor

open Silencer.Extension.Shared.Types
open Microsoft.ML

let mlContext = MLContext(seed = 1)
let assembly = (typeof<Types.SentimentData>).Assembly
let names = assembly.GetManifestResourceNames()

let modelResourceName =
  names
  |> Array.find (fun s -> s.Contains("toxicity_classifier.zip"))

printfn $"Resource name found: %s{modelResourceName}"

let modelStream =
  assembly.GetManifestResourceStream(modelResourceName)

let mutable dataViewSchema: DataViewSchema = null

let model =
  mlContext.Model.Load(modelStream, &dataViewSchema)

let predictionEngine =
  mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model)

let predictToxicity (text: string) =
  let data = SentimentData.ForText text
  predictionEngine.Predict data
