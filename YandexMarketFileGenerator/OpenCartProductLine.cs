using System;
using System.Collections.Generic;
using System.Linq;

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
        

        public static List<OpenCartProductLine> ParseList(string rawData)
        {
            var readedProducts = rawData
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                .Skip(1)
                .Select(line => OpenCartProductLine.Parse(line))
                .ToList();

            return readedProducts;
        }

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
                product.Model = data[4].Trim();
                product.Sku = data[5].Trim();
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

    }
}
