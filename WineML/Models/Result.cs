using Microsoft.ML.Data;

namespace WineML.Models
{
    public class WinePrediction
    {
        [ColumnName("PredictedLabel")]
        public int PredictedQuality { get; set; }

        public float Probability { get; set; }
        public float[] Score { get; set; }
    }
}