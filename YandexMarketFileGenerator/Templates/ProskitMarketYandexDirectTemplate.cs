using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Pro'sKit (proskit-market.ru)")]
    public class ProskitMarketYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Pro'sKit";
        public string Host { get; } = "http://proskit-market.ru";
        public string TranslatedManufacturer { get; } = "Проскит";

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["V"] = "+",
            ["X"] = "Работает везде",
        };

        public List<string> InvalidWords => throw new NotImplementedException();

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 2));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(ProskitMarketYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(2);
        }
    }

    internal class ProskitMarketYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public ProskitMarketYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
            Product.Name = Product.Name
                .Replace("Proskit", string.Empty)
                .Replace("Pro'sKit", string.Empty)
                .Replace("Pros'Kit", string.Empty)
                .Replace("ProsKit", string.Empty)
                .Replace(Product.Model, string.Empty)
                .RemoveExtraSpaceAndTrim();
        }

        protected override void FillDictionary(int lineNumber)
        {
            resultDictionary["A"] = parentSection.ParentTemplate.ColumnStaticValues["A"];
            resultDictionary["B"] = parentSection.ParentTemplate.ColumnStaticValues["B"];
            resultDictionary["C"] = parentSection.ParentTemplate.ColumnStaticValues["C"];
            resultDictionary["D"] = string.Empty;
            resultDictionary["E"] = GetGroupName();
            resultDictionary["F"] = parentSection.GroupIndex.ToString();
            resultDictionary["G"] = parentSection.ParentTemplate.ColumnStaticValues["G"];
            resultDictionary["H"] = string.Empty;
            resultDictionary["I"] = GetPhrase(lineNumber);
            resultDictionary["J"] = string.Empty;
            resultDictionary["K"] = GetTitle1();
            resultDictionary["L"] = GetTitle2();
            resultDictionary["M"] = GetTitle3();
            resultDictionary["N"] = string.Empty;
            resultDictionary["O"] = string.Empty;
            resultDictionary["P"] = string.Empty;
            resultDictionary["Q"] = FullUrlPath;
            resultDictionary["R"] = GetViewedUrl();
            resultDictionary["S"] = parentSection.ParentTemplate.ColumnStaticValues["S"];
            resultDictionary["T"] = string.Empty;
            resultDictionary["U"] = string.Empty;
            resultDictionary["V"] = parentSection.ParentTemplate.ColumnStaticValues["V"];
            resultDictionary["W"] = string.Empty;
            resultDictionary["X"] = parentSection.ParentTemplate.ColumnStaticValues["X"];
            resultDictionary["Y"] = string.Empty;
            resultDictionary["Z"] = string.Empty;
            resultDictionary["AA"] = string.Empty;
            resultDictionary["AB"] = string.Empty;
            resultDictionary["AC"] = string.Empty;
            resultDictionary["AD"] = string.Empty;
            resultDictionary["AE"] = string.Empty;
            resultDictionary["AF"] = string.Empty;
            resultDictionary["AG"] = string.Empty;
            resultDictionary["AH"] = string.Empty;
            resultDictionary["AI"] = string.Empty;
            resultDictionary["AJ"] = string.Empty;
            resultDictionary["AK"] = string.Empty;
            resultDictionary["AL"] = string.Empty;
            resultDictionary["AM"] = string.Empty;
            resultDictionary["AN"] = string.Empty;
            resultDictionary["AO"] = string.Empty;
            resultDictionary["AP"] = string.Empty;
            resultDictionary["AQ"] = string.Empty;
            resultDictionary["AR"] = string.Empty;
            resultDictionary["AS"] = string.Empty;
            resultDictionary["AT"] = string.Empty;
            resultDictionary["AU"] = Product.Price.ToString("F0");
        }

        protected override string GetGroupName()
        {
            return $"pro skit {parentSection.ProductLine.Model}".ToLower().Trim();
        }

        protected override string GetTitle1()
        {
            //Кримпер Pro skit 376tr

            var title = $"{Product.ProductTypeFull} Pro skit {Product.Model.ToLower()}".RemoveExtraSpaceAndTrim();
            return title;
        }

        protected override string GetTitle2()
        {
            //Pro skit 376tr
            var title = $"Pro skit {parentSection.ProductLine.Model.ToLower()}".RemoveExtraSpaceAndTrim();

            return title;
        }

        protected override string GetTitle3()
        {
            //Кримпер Pro skit 376tr с доставкой по России.
            var title = ($"{Product.ProductTypeFull} Pro skit {Product.Model.ToLower()} c доставкой по России.").RemoveExtraSpaceAndTrim();

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            //pro skit 376tr
            //376tr


            if (lineNumber == 1)
            {
                keyPhrase = $"pro skit {Product.Model.ToLower()}".RemoveExtraSpaceAndTrim();
            }
            else if (lineNumber == 2)
            {
                keyPhrase = Product.Model.ToLower().RemoveExtraSpaceAndTrim();
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToLower();
        }

        protected override string GetViewedUrl()
        {
            //proskit-cp-376tr
            var replacedModel = Product.Model.ToLower()
                .Replace("/", "-")
                .Replace(".", "-")
                .RemoveExtraSpaceAndTrim();

            var viewedUrl = $"proskit-{replacedModel}";

            if(viewedUrl.Length > VIEWED_URL_MAX_LENGTH)
            {
                viewedUrl = viewedUrl.Replace("proskit-", string.Empty);
            }

            return viewedUrl;
        }
    }
}
