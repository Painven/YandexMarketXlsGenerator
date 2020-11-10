using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Hakko")]
    public class HakkoYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://hakko.su";
        public string Manufacturer { get; } = "Hakko";
        public string TranslatedManufacturer { get; } = "Хакко";

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["V"] = "+",
            ["X"] = "Работает везде",
            ["W"] = "Работает везде",
            ["Y"] = "Hakko Паяльное оборудование||Паяльные станции||Паяльники||Hakko",
            ["AA"] = "https://hakko.su/||https://hakko.su/payalnye-stanczii/||https://hakko.su/payalniki/||https://hakko.su/raskhodnye-materialy/",
            ["AJ"] = "Hakko каталог||набор инструментов Hakko||Hakko германия||набор ключей Hakko||Hakko набор бит",
            ["AK"] = "ремонт оборудования||Жала паяльные||Расходные материалы"
        };

        public List<string> InvalidWords => throw new NotImplementedException();


        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            var source = productsInfo.ToList();

            foreach (var line in source)
            {
                var section = CreateSection(line, startGroupSectionNumber++);

                sb.Append(section);               
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount = 1)
        {
            var data = new YandexMarketSection(this, typeof(HakkoYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class HakkoYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public HakkoYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection) { }

        protected override void FillDictionary(int lineNumber)
        {
            var cells = GetCellsRange();

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
            resultDictionary["F"] = $"{parentSection.GroupIndex}";
            resultDictionary["I"] = GetPhrase(lineNumber);
            resultDictionary["K"] = GetTitle1();
            resultDictionary["L"] = GetTitle2();
            resultDictionary["M"] = GetTitle3();
            resultDictionary["Q"] = FullUrlPath;
            resultDictionary["R"] = GetViewedUrl();
            resultDictionary["AU"] = GetProductPrice();

        }

        protected override string GetViewedUrl()
        {
            return $"{Manufacturer}-{Product.Model}".Replace(" ", "-").Trim(' ', '-').Replace("--", "-");
        }

        protected override string GetGroupName()
        {
            string groupName = $"{Manufacturer} {Product.Model}".ToLower().Trim();

            return groupName;
        }

        protected override string GetTitle1()
        {
            var title = $"{Manufacturer} {Product.Model} {Product.ProductTypeShort}";

            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{Manufacturer} {Product.Model}";

            return title;
        }

        protected override string GetTitle3()
        {
            var title = $"{Manufacturer} {Product.Model} {Product.ProductTypeFull} c доставкой РФ!";

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string result = null;

            var replacedModel = Regex.Replace(Product.Model, " {2,}", " ").Trim();

            if (lineNumber == 1)
            {
                result = $"{Manufacturer} {replacedModel}".ToLower().ToKeyPhrase();
            }
            else if (lineNumber == 2)
            {
                result = $"{replacedModel} -{Manufacturer}".ToLower().ToKeyPhrase();
            }
            else
            {
                throw new FormatException();
            }

            result = Regex.Replace(result, " +", " ")
                .Replace("-", " ")
                .Trim();

            if (result.Split().Length > 7)
            {
                //throw new FormatException("Длина ключа должна содержать не более 7 слов");
                File.AppendAllLines("errors_Hakko.txt", new string[] { $"[{DateTime.Now.ToShortTimeString()}] {parentSection.ProductLine.Sku}" });
            }

            return result;
        }
    }
}
