using WineML.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using WineML.Models;

namespace WineML.AI;
class Classifier
{
    private static ITransformer model;
    private static string dataPath = "C:/Users/evzel/source/repos/WineML/WineML/AI/winequality-transformed.csv";

    public Classifier()
    {
        if (model == null)
            InitModel();
    }

    public int Predict(Wine elem)
    {
        var context = new MLContext();
        var predictionEngine = context.Model.CreatePredictionEngine<Wine, WinePrediction>(model);
        var prediction = predictionEngine.Predict(elem);

        Console.WriteLine($"Predicted quality: {prediction.PredictedQuality}");

        foreach (float score in prediction.Score)
        {
            Console.WriteLine($"{score:F5}");
        }

        Console.WriteLine();
        return prediction.PredictedQuality;
    }

    private void InitModel()
    {
        var context = new MLContext();
        var data = context.Data.LoadFromTextFile<Wine>(dataPath, separatorChar: ',', hasHeader: true);

        // Concatenate features and map label to key
        // Best 86%
        var pipeline = context.Transforms.Conversion.MapValueToKey("Label", "quality")
            .Append(context.Transforms.Concatenate("Features", "fixed_acidity", "volatile_acidity", "citric_acid",
                                            "residual_sugar", "chlorides", "free_sulfur_dioxide", "total_sulfur_dioxide",
                                            "density", "pH", "sulphates", "alcohol", "wine_white"))
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
        // Console.WriteLine($"Macro Accuracy: {metrics.MacroAccuracy}"); // metrics for each class
    }
}
