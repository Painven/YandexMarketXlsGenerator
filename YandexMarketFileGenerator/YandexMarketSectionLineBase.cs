using System;
using System.Collections.Generic;
using System.Linq;

namespace YandexMarketFileGenerator
{
    public abstract class YandexMarketSectionLineBase
    {
        public const int TITLE1_MAX_LENGTH = 35;
        public const int TITLE2_MAX_LENGTH = 30;
        public const int TITLE3_MAX_LENGTH = 80;
        public const int VIEWED_URL_MAX_LENGTH = 20;
        public const int KEY_PHRASE_MAX_WORDS = 7;
        
        protected string FullUrlPath => $"{parentSection.Host}/{Product.URL}/?utm_source=yandex";
        protected YandexMarketSection parentSection { get; }
        protected Dictionary<string, string> resultDictionary { get; }
        protected OpenCartProductLine Product
        {
            get
            {
                return parentSection.ProductLine;
            }
        }
        
        
        protected string ModelWithoutManufacturerName
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(Product.Model))
                {
                    return Product.Model.Replace(parentSection.Manufacturer, string.Empty).Trim();
                }

                return string.Empty;               
            }
        }            
        protected string Manufacturer => parentSection.Manufacturer;
        protected string Sku => Product.Sku;
        protected string Model => Product.Model;
        protected string ProductTypeShort => Product.ProductTypeShort;
        protected string ProductTypeFull => Product.ProductTypeFull;

        protected string TranslatedManufacturer => parentSection.TranslatedManufacturer;

        public YandexMarketSectionLineBase(YandexMarketSection parentSection)
        {
            this.parentSection = parentSection;
            resultDictionary = new Dictionary<string, string>();
        }

        protected virtual string GetProductPrice()
        {
            return Product.Price != decimal.Zero ? Product.Price.ToString("F0") : string.Empty;
        }

        protected virtual void FillDictionary(int lineNumber)
        {
            var range1 = Enumerable.Range(0, 26)
                .Select(i => (char)((int)'A' + i))
                .Select(c => c.ToString())
                .ToList();

            var range2 = Enumerable.Range(0, 21)
                .Select(i => (char)((int)'A' + i))
                .Select(c => string.Concat("A", c.ToString()))
                .ToList();

            var cells = range1.Concat(range2).ToList();

            foreach (var cellLetter in cells)
            {
                if (parentSection.ParentTemplate.ColumnStaticValues.ContainsKey(cellLetter))
                {
                    resultDictionary[cellLetter] = parentSection.ParentTemplate.ColumnStaticValues[cellLetter];
                }
                else
                {
                    resultDictionary[cellLetter] = string.Empty;
                }
            }

            resultDictionary["E"] = GetGroupName();
            resultDictionary["F"] = parentSection.GroupIndex.ToString();
            resultDictionary["I"] = GetPhrase(lineNumber);
            resultDictionary["K"] = GetTitle1();
            resultDictionary["L"] = GetTitle2();
            resultDictionary["M"] = GetTitle3();
            resultDictionary["Q"] = FullUrlPath;
            resultDictionary["R"] = GetViewedUrl();
            resultDictionary["AV"] = Product.Price == decimal.Zero ? "" : Product.Price.ToString("F0");
        }
        protected virtual string GetGroupName()
        {
            return ($"{parentSection.ProductLine.Model} {parentSection.ProductLine.ProductTypeShort}")
                .Replace("/", " ")
                .Replace("\\", " ")
                .Trim(' ', '\\', '/');
        
        }
        protected virtual string GetViewedUrl()
        {
            Func<string, string> getValidString = (s) =>
                 s.Replace(" ", "-")
                 .Replace("/", "-")
                 .Replace("\\", "-")
                 .Replace(".", "-")
                 .Replace("–", "")
                 .Replace("(", string.Empty)
                 .Replace(")", string.Empty)
                 .Replace("--", "-")
                 .Trim();

            var url = getValidString($"{parentSection.Manufacturer}-{ModelWithoutManufacturerName}");
            if (url.Length <= 20)
            {
                return url;
            }
            else
            {
                url = getValidString(ModelWithoutManufacturerName);

                if (url.Length > 20)
                {
                    url = getValidString(ModelWithoutManufacturerName)
                        .Replace("-", string.Empty)
                        .Trim();
                }

                return url;
            }
        
        }
        
        protected abstract string GetTitle1();
        protected abstract string GetTitle2();
        protected abstract string GetTitle3();
        protected abstract string GetPhrase(int lineNumber);

        public virtual string BuildLine(int lineNumber)
        {
            FillDictionary(lineNumber);
            return string.Join("\t", resultDictionary.Values);
        }

        protected virtual List<string> GetCellsRange()
        {
            var range1 = Enumerable.Range(0, 26)
                .Select(i => (char)((int)'A' + i))
                .Select(c => c.ToString())
                .ToList();

            var range2 = Enumerable.Range(0, 21)
                .Select(i => (char)((int)'A' + i))
                .Select(c => string.Concat("A", c.ToString()))
                .ToList();

            var cells = range1.Concat(range2).ToList();
            return cells;
        }
    }

}
