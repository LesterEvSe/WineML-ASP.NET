using WineML.Models;
using Microsoft.ML;

namespace WineML.AI;
class Classifier
{
    private static ITransformer? model;

    public Classifier()
    {
        DB.LoadFromCsv("C:/Users/evzel/source/repos/WineML/WineML/AI/winequality-init.csv", true);

        if (model == null) {
            ReInitModel();
        }
    }

    public int Predict(WineMLData elem)
    {
        var context = new MLContext();
        var predictionEngine = context.Model.CreatePredictionEngine<WineMLData, WinePrediction>(model);
        var prediction = predictionEngine.Predict(elem);

        Console.WriteLine($"Predicted quality: {prediction.PredictedQuality}");

        foreach (float score in prediction.Score)
        {
            Console.WriteLine($"{score:F5}");
        }

        Console.WriteLine();
        return prediction.PredictedQuality;
    }

    public static void ReInitModel()
    {
        var context = new MLContext();
        var data = DB.LoadDataFromDB(context);

        // Concatenate features and map label to key
        // Best 86%
        var pipeline = context.Transforms.Conversion.MapValueToKey("Label", "quality")
            .Append(context.Transforms.Concatenate("Features", "fixed_acidity", "volatile_acidity", "citric_acid",
                                            "residual_sugar", "chlorides", "free_sulfur_dioxide", "total_sulfur_dioxide",
                                            "density", "pH", "sulphates", "alcohol", "white_wine"))
            .Append(context.Transforms.NormalizeMeanVariance("Features")) // Normalization
            .Append(context.MulticlassClassification.Trainers.LightGbm(labelColumnName: "Label", featureColumnName: "Features"))
            .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));


        // Split data into training and testing sets
        var splitData = context.Data.TrainTestSplit(data, testFraction: 0.2, seed: 42);

        // Train the model
        model = pipeline.Fit(splitData.TrainSet);

        // Evaluate the model
        var predictions = model.Transform(splitData.TestSet);
        var metrics = context.MulticlassClassification.Evaluate(predictions, labelColumnName: "Label");

        // Output metrics
        Console.WriteLine($"Log Loss: {metrics.LogLoss:F5}");
        Console.WriteLine($"Accuracy: {metrics.MicroAccuracy:F5}"); // average metric for all classes
    }
}
