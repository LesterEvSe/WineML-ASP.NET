using Microsoft.ML.Data;

namespace WineML.Models
{
    public class WinePrediction
    {
        [ColumnName("PredictedLabel")]
        public int PredictedQuality { get; set; }

        public float Probability { get; set; }
        public float[]? Score { get; set; }
    }
}

/*
7.4  0.34  0.42
1.1,  0.033  17
171  0.9917  3.12
0.53  11.3  white -> quality 1 +

6.8   0.26   0.42
1.7   0.049  41
122  0.993    3.47
0.48  10.5   white ->  quality 2  +

8  0.59  0.16
1.8  0.065  3
16  0.9962  3.42
0.92  10.5  red -> quality 2 +

7.4  1.185  0
4.25  0.097  5
14  0.9966  3.63
0.54  10.7  red -> quality 0 +

6.6  0.36  0.29
1.6  0.021  24
85  0.98965  3.41
0.61  12.4  white -> quality 3 +
*/
