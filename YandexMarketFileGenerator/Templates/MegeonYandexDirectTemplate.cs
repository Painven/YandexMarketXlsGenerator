using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Megeon")]
    public class MegeonYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Megeon";
        public string TranslatedManufacturer { get; } = "Мегеон";

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["V"] = "-",
            ["W"] = "Активно",
            ["X"] = "Работает везде",
            ["Y"] = "Мегеон||Мегаомметры Мегеон||Пирометры Мегеон||Мультиметры Мегеон",
            ["AA"] = "https://etk-komplekt.ru/megeon||https://etk-komplekt.ru/izmeritelnye-pribory/izmeriteli-soprotivleniya/megaommetry/megaommetry-megeon/||https://etk-komplekt.ru/izmeritelnye-pribory/pirometry/pirometry-megeon/||https://etk-komplekt.ru/izmeritelnye-pribory/multimetry-cifrovye/multimetry-cifrovye-megeon/",
            ["AJ"] = "официальный дилер||измеритель мегеон||анемометр мегеон",
            ["AK"] = "-поверка -сертификат -скачать"
        };

        public List<string> InvalidWords => throw new NotImplementedException();

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            var source = productsInfo.ToList();

            foreach (var line in source)
            {
                var section = CreateSection(line, startGroupSectionNumber++, 1);

                sb.Append(section);               
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount = 3)
        {
            var data = new YandexMarketSection(this, typeof(MegeonYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class MegeonYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public MegeonYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
            
        }

        protected override void FillDictionary(int lineNumber)
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
            resultDictionary["F"] = $"{parentSection.GroupIndex}";
            resultDictionary["I"] = GetPhrase(lineNumber);
            resultDictionary["K"] = GetTitle1();
            resultDictionary["L"] = GetTitle2();
            resultDictionary["M"] = GetTitle3();
            resultDictionary["Q"] = FullUrlPath;
            resultDictionary["R"] = GetViewedUrl();

        }

        protected override string GetViewedUrl()
        {
            if (Regex.IsMatch(Product.Model, @"МЕГЕОН \d+"))
            {
                return Product.Model.Replace(" ", "-").ToUpper();
            }
            return string.Empty;
        }

        protected override string GetGroupName()
        {
            return Product.Model.ToUpper();
        }

        protected override string GetTitle1()
        {
            var title = $"{parentSection.ProductLine.Model.ToUpper()} {parentSection.ProductLine.ProductTypeShort}".Trim();

            return title;
        }

        protected override string GetTitle2()
        {
            var title = parentSection.ProductLine.Model.ToUpper().Trim();

            return title;
        }

        protected override string GetTitle3()
        {
            //var title = "МЕГЕОН 22150 мультиметр от официального дилера. Доставка по России";
            var title = $"{Product.Model.ToUpper()} {Product.ProductTypeShort.Trim()} от официального дилера. Доставка по России";

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string result = null;

            if (lineNumber == 1)
            {
                result = Product.Sku.ToKeyPhrase();
            }
            else if (lineNumber == 2)
            {
                string modelString = (parentSection.Count() == 2) ? Product.Sku.Replace("WI ", string.Empty) : Product.Model;

                result = $"{Product.ProductTypeShort} {Manufacturer} {modelString}".ToKeyPhrase();
            }
            else if (lineNumber == 3)
            {
                result = $"{Manufacturer} {Product.Model}".ToKeyPhrase();
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
                File.AppendAllLines("errors_Megeon.txt", new string[] { $"[{DateTime.Now.ToShortTimeString()}] {parentSection.ProductLine.Sku}" });
            }

            return result;
        }
    }
}
