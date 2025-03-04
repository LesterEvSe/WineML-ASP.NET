using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using WineML.Models;
using CsvHelper.Configuration.Attributes;

namespace WineML
{
    public class Wine
    {
        [Key]
        [Ignore]  // When import from CSV file
        public Guid Id { get; set; } = Guid.NewGuid();

        public required float fixed_acidity { get; set; }
        public required float volatile_acidity { get; set; }
        public required float citric_acid { get; set; }
        public required float residual_sugar { get; set; }
        public required float chlorides { get; set; }
        public required float free_sulfur_dioxide { get; set; }
        public required float total_sulfur_dioxide { get; set; }
        public required float density { get; set; }
        public required float pH { get; set; }
        public required float sulphates { get; set; }
        public required float alcohol { get; set; }
        public required int quality { get; set; } // target. [0; 3] - 0, [4;6] - 1, [7;8] - 2, [9;10] - 3
        public required float white_wine { get; set; }
    }

    // Singleton DB
    public class DB : DbContext
    {
        public static DB Instance = new();
        public DbSet<Wine> Wines { get; set; }

        private DB()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=ViruseWar;Trusted_Connection=True;Encrypt=False");
        }

        // LINQ Requests
        public void AddWine(Wine wine)
        {
            Wines.Add(wine);
            SaveChanges();
        }

        public static void LoadFromCsv(string csvPath, bool init=false)
        {
            using var db = DB.Instance;
            if (init && db.Wines.Count() > 6000)
            {
                Console.WriteLine($"The table has already been initialized. The current number of instances: {db.Wines.Count()}.");
                return;
            }

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });

            var wines = csv.GetRecords<Wine>().ToList();
            db.Wines.AddRange(wines);
            db.SaveChanges();

            Console.WriteLine($"Successfully saved {db.Wines.Count()} records to database.");
        }

        public IDataView LoadDataFromDB(MLContext mlContext)
        {
            using var db = new DB();

            List<WineMLData> wines = db.Wines.Select(w => new WineMLData
            {
                fixed_acidity = w.fixed_acidity,
                volatile_acidity = w.volatile_acidity,
                citric_acid = w.citric_acid,
                residual_sugar = w.residual_sugar,
                chlorides = w.chlorides,
                free_sulfur_dioxide = w.free_sulfur_dioxide,
                total_sulfur_dioxide = w.total_sulfur_dioxide,
                density = w.density,
                pH = w.pH,
                sulphates = w.sulphates,
                alcohol = w.alcohol,
                quality = w.quality,
                white_wine = w.white_wine
            }).ToList();

            return mlContext.Data.LoadFromEnumerable(wines);
        }

        // Debug method
        public static void PrintAllWines()
        {
            using var db = new DB();
            var wines = db.Wines.ToList();

            if (wines.Count == 0)
            {
                Console.WriteLine("No wines found in the database.");
                return;
            }

            Console.WriteLine("Wine Database Records:");
            Console.WriteLine("------------------------------------------------------------");
            foreach (var wine in wines)
            {
                Console.WriteLine($"Fixed Acidity: {wine.fixed_acidity}, Volatile Acidity: {wine.volatile_acidity}, " +
                                  $"Citric Acid: {wine.citric_acid}, Residual Sugar: {wine.residual_sugar}, " +
                                  $"Chlorides: {wine.chlorides}, Free SO₂: {wine.free_sulfur_dioxide}, " +
                                  $"Total SO₂: {wine.total_sulfur_dioxide}, Density: {wine.density}, " +
                                  $"pH: {wine.pH}, Sulphates: {wine.sulphates}, Alcohol: {wine.alcohol}, " +
                                  $"Quality: {wine.quality}, White Wine: {wine.white_wine}");
            }
            Console.WriteLine("------------------------------------------------------------");
        }
    }
}
