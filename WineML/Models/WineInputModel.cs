namespace WineML.Models
{
    public class WineInputModel
    {
        public float fixed_acidity { get; set; }
        public float volatile_acidity { get; set; }
        public float citric_acid { get; set; }
        public float residual_sugar { get; set; }
        public float chlorides { get; set; }
        public float free_sulfur_dioxide { get; set; }
        public float total_sulfur_dioxide { get; set; }
        public float density { get; set; }
        public float pH { get; set; }
        public float sulphates { get; set; }
        public float alcohol { get; set; }
        public string color { get; set; }
    }
}
