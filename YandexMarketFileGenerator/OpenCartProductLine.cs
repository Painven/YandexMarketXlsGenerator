using System;

namespace YandexMarketFileGenerator
{
    public class OpenCartProductLine
    {
        public string Name { get; set; }
        public string ProductTypeFull { get; set; }
        public string ProductTypeShort { get; set; }
        public string Model { get; set; }
        public string Sku { get; set; }
        public string URL { get; set; }
        public decimal Price { get; set; }
        public bool IsUniquePhrase { get; set; }
        public string CustomField { get; set; }
        

        public static OpenCartProductLine Parse(string line)
        {
            var product = new OpenCartProductLine();
            
            try
            {
                var data = line.Split(new string[] { "\t" }, StringSplitOptions.None);

                product.Name = data[0].Replace("  ", " ").Trim();
                product.ProductTypeFull = data[1].Replace("  ", " ").Trim();
                product.ProductTypeShort = data[2].Replace("  ", " ").Trim();
                product.IsUniquePhrase = bool.Parse(data[3]);
                product.Model = ParseModel(data[4]);
                product.Sku = ParseModel(data[5]);
                product.Price = !string.IsNullOrWhiteSpace(data[6]) ? decimal.Parse(data[6]) : decimal.Zero;
                product.URL = data[7].Replace("  ", " ").Trim();
                product.CustomField = data[8].Trim();


                
            }
            catch(Exception ex)
            {
                throw;   
            }

            return product;
        }

        private static string ParseModel(string v)
        {
            return v
                .Replace("  ", " ")
                .Replace("+", "plus")
                .Trim();
        }
    }
}
