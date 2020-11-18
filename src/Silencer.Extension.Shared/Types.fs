module Silencer.Extension.Shared.Types

open Microsoft.ML.Data

[<CLIMutable>]
type SentimentData =
  { Text: string
    [<ColumnName("Label")>]
    Toxic: bool }

  static member ForText text = { Text = text; Toxic = false }

[<CLIMutable>]
type SentimentPrediction =
  { [<ColumnName("PredictedLabel")>]
    Prediction: bool
    Probability: float32
    Score: float32 }
