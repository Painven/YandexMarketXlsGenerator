using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Weller")]
    public class WellerYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://weller-wd.ru";
        public string Manufacturer { get; } = "Weller";
        public string TranslatedManufacturer { get; } = "Веллер";
        public List<string> InvalidWords { get; } = new List<string>()
        {
            "activator",
            "anaheim",
            "california",
            "cargo",
            "chemical",
            "ellen",
            "gladys",
            "joe",
            "judith",
            "kate",
            "linden",
            "lord",
            "mar",
            "mask",
            "michael",
            "paul",
            "rondell",
            "sam",
            "set",
            "spot",
            "tip",
            "wecp",
            "велосипед",
            "выжигатель",
            "газ",
            "запчасть",
            "котел",
            "отзыв",
            "пеларгония",
            "перевод",
            "сковорода",
            "сотейник",
            "час"
        };

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["Z"] = "Паяльные станции||Паяльники||Аксесуары||Дымоудаление и фильтрация",
            ["AB"] = "https://weller-wd.ru/payalnye-stancii/||https://weller-wd.ru/payalniki/||https://weller-wd.ru/vse-dlya-pajki/||https://weller-wd.ru/dymoudalenie-i-filtraciya/",
            ["T"] = "Россия",
            ["Y"] = "Работает везде",
            ["W"] = "+",
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["G"] = "-",
            ["C"] = "-",
            ["AJ"] = ""
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo.OrderBy(p => p.Model.Length))
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 3));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(WellerYandexMarketSectionLine), productInfo, groupIndex);
            bool cancelLast = productInfo.Model.Split().Where(word => !string.IsNullOrEmpty(word)).Count() == 1;

            return data.BuildSection(3);
        }
    }

    internal class WellerYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public WellerYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
        }

        protected override void FillDictionary(int lineNumber)
        {
            resultDictionary["A"] = parentSection.ParentTemplate.ColumnStaticValues["A"];
            resultDictionary["B"] = parentSection.ParentTemplate.ColumnStaticValues["B"];
            resultDictionary["C"] = parentSection.ParentTemplate.ColumnStaticValues["C"];
            resultDictionary["D"] = string.Empty;
            resultDictionary["E"] = GetGroupName();
            resultDictionary["F"] = $"{parentSection.GroupIndex}";
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
            resultDictionary["S"] = parentSection.ParentTemplate.ColumnStaticValues["T"];
            resultDictionary["T"] = string.Empty;
            resultDictionary["U"] = string.Empty;
            resultDictionary["V"] = string.Empty;
            resultDictionary["W"] = parentSection.ParentTemplate.ColumnStaticValues["W"];
            resultDictionary["X"] = string.Empty;
            resultDictionary["Y"] = parentSection.ParentTemplate.ColumnStaticValues["Y"];
            resultDictionary["Z"] = parentSection.ParentTemplate.ColumnStaticValues["Z"];
            resultDictionary["AA"] = string.Empty;
            resultDictionary["AB"] = parentSection.ParentTemplate.ColumnStaticValues["AB"];
            resultDictionary["AC"] = string.Empty;
            resultDictionary["AD"] = string.Empty;
            resultDictionary["AE"] = string.Empty;
            resultDictionary["AF"] = string.Empty;
            resultDictionary["AG"] = string.Empty;
            resultDictionary["AH"] = string.Empty;
            resultDictionary["AI"] = string.Empty;
            resultDictionary["AJ"] = parentSection.ParentTemplate.ColumnStaticValues["AJ"];
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
        }

        protected override string GetViewedUrl()
        {
            return base.GetViewedUrl().Replace(Manufacturer + "-", string.Empty).Trim();
        }

        protected override string GetTitle1()
        {
            var title = $"{parentSection.Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeShort}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                title = $"{ModelWithoutManufacturerName} {Product.ProductTypeShort}";
            }

            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{parentSection.Manufacturer} {ModelWithoutManufacturerName}";

            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                title = ModelWithoutManufacturerName;
            }
            return title;
        }

        protected override string GetTitle3()
        {
            string title = null;

            decimal realPriceInRoubles = Math.Ceiling(Product.Price / 10) * 10;

            if (realPriceInRoubles != decimal.Zero)
            {
                title = $"{parentSection.Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}. Цена {realPriceInRoubles} руб, в наличии, отправка по России!";
                if (title.Length >= TITLE3_MAX_LENGTH)
                {
                    title = $"{parentSection.Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}. Цена {realPriceInRoubles} руб, отправка по России!";

                    if (title.Length >= TITLE3_MAX_LENGTH)
                    {
                        title = $"{parentSection.Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}. Цена {realPriceInRoubles} руб,";
                    }
                }
            }
            else
            {
                title = $"{parentSection.Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}. В наличии, отправка по России!";
                if (title.Length >= TITLE3_MAX_LENGTH)
                {
                    title = $"{parentSection.Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}. Отправка по России!";

                    if (title.Length >= TITLE3_MAX_LENGTH)
                    {
                        title = $"{parentSection.Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}. В наличии";
                    }
                }
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string result = null;

            if (lineNumber == 1)
            {
                throw new NotImplementedException();
            }
            else if (lineNumber == 2)
            {
                result = $"{Manufacturer} {ModelWithoutManufacturerName}"
                    .ToKeyPhrase();
            }
            else if (lineNumber == 3)
            {
                result = $"{ModelWithoutManufacturerName} {Manufacturer.ToLower()}"
                    .ToKeyPhrase();
            }
            else if (lineNumber == 4)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new FormatException();
            }

            return result;
        }
    }
}
