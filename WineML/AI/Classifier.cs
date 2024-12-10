using WineML.Models;
using Microsoft.ML;
using Microsoft.ML.Data;

/*
7.4  0.34  0.42
1.1,  0.033  17
171  0.9917  3.12
0.53  11.3  white -> quality 1 +

6.8   0.26   0.42
1.7   0.049  41
122  0.993    3.47
0.48  10.5   white ->  quality 2  +

7.4   0.18   0.31
1.4   0.058  38
167  0.9931   3.16
0.53  10  white -> quality 2  -

9.1   0.27   0.45
10.6  0.035  28
124  0.997  3.2
0.46  10.4   white -> quality 3 -

6.6  0.36  0.29
1.6  0.021  24
85  0.98965  3.41
0.61  12.4  white -> quality 3 +

8  0.59  0.16
1.8  0.065  3
16  0.9962  3.42
0.92  10.5  red -> quality 2 +

7.4  1.185  0
4.25  0.097  5
14  0.9966  3.63
0.54  10.7  red -> quality 0 +
*/

namespace WineML.AI;
class Classifier
{
    private static ITransformer? model;
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
