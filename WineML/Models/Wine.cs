using Microsoft.ML.Data;

// fixed_acidity, volatile_acidity, citric_acid, residual_sugar, chlorides, free_sulfur_dioxide, total_sulfur_dioxide, density, pH, sulphates, alcohol, quality

namespace WineML.Models
{
    public class WineMLData
    {
        [LoadColumn(0)] public float fixed_acidity { get; set; }
        [LoadColumn(1)] public float volatile_acidity { get; set; }
        [LoadColumn(2)] public float citric_acid { get; set; }
        [LoadColumn(3)] public float residual_sugar { get; set; }
        [LoadColumn(4)] public float chlorides { get; set; }
        [LoadColumn(5)] public float free_sulfur_dioxide { get; set; }
        [LoadColumn(6)] public float total_sulfur_dioxide { get; set; }
        [LoadColumn(7)] public float density { get; set; }
        [LoadColumn(8)] public float pH { get; set; }
        [LoadColumn(9)] public float sulphates { get; set; }
        [LoadColumn(10)] public float alcohol { get; set; }
        [LoadColumn(11)] public int quality { get; set; } // target. [0; 3] - 0, [4;6] - 1, [7;8] - 2, [9;10] - 3
        [LoadColumn(12)] public float white_wine { get; set; }
    }
}