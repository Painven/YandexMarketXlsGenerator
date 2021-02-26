using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для FIT")]
    public class FITYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "FIT";
        public string TranslatedManufacturer => "Фит";
        public List<string> InvalidWords => throw new NotImplementedException();

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>();
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = !string.IsNullOrWhiteSpace(line.Model) ? 6 : 5;

                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(FITYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class FITYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public FITYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection) { }

        protected override string GetGroupName()
        {
            return $"{Manufacturer.ToUpper()} {Product.Sku}";
        }

        protected override string GetViewedUrl()
        {
            string url = $"{Manufacturer} {Product.Sku}".ToViewedUrl();

            if (url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                url = Product.Model.ToViewedUrl();
            }

            if (url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url.ToUpper();
        }

        protected override string GetTitle1()
        {
            var title = $"{Manufacturer} {Product.Sku} {Product.ProductTypeShort}";

            return title;
        }

        protected override string GetTitle2()
        {
            string title =  $"{Manufacturer} {Product.Sku}";

            return title;
        }

        protected override string GetTitle3()
        {
            string title = null;

            if(!string.IsNullOrWhiteSpace(Product.Model))
            {
                title = $"{Manufacturer} {Product.ProductTypeFull} {Product.Model} {Product.Sku} с доставкой по России!";
            }
            else
            {
                title = $"{Manufacturer} {Product.Sku} {Product.ProductTypeFull} с доставкой по России!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Manufacturer} {Product.Sku} {Product.ProductTypeFull} в наличии";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string phrase = string.Empty;

            if (lineNumber == 1)
            {
                phrase = $"{Manufacturer} {Product.Sku}";
            }
            else if(lineNumber == 2)
            {
                phrase = $"{Manufacturer}-{Product.Sku}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Sku}";
            }
            else if (lineNumber == 4)
            {
                phrase = $"{Product.ProductTypeShort} {Manufacturer} {Product.Sku}";
            }
            else if(lineNumber == 5)
            {
                phrase = $"{TranslatedManufacturer} {Product.Sku}";
            }
            else if(lineNumber == 6)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Model} {Product.Sku}";
            }
            else
            {
                throw new NotImplementedException();
            }

            return phrase.ToLower().Trim();
        }
    }
}
