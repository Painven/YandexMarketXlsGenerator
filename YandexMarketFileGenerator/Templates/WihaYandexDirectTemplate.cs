using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Wiha")]
    public class WihaYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Wiha";
        public string TranslatedManufacturer { get; } = "Виха";

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["V"] = "+",
            ["X"] = "Работает везде",
            ["Y"] = "Отвертки Wiha||Пассатижи Wiha||Наборы ключей Wiha||Держатели Wiha",
            ["AA"] = "https://etk-komplekt.ru/ruchnoy-instrument/otvertka/otvertki-wiha/||https://etk-komplekt.ru/ruchnoy-instrument/ploskogubcy-kleshchi-kusachki/passatizhi/passatizhi-wiha/||https://etk-komplekt.ru/ruchnoy-instrument/instrumentalnye-klyuchi/klyuchi-wiha/nabory-klyuchey-wiha/||https://etk-komplekt.ru/ruchnoy-instrument/tiski-i-derzhateli/derzhateli-wiha/",
            ["AJ"] = "wiha каталог||набор инструментов wiha||wiha германия||набор ключей wiha||wiha набор бит",
            ["AK"] = "\"-поверка -сертификат -скачать\""
        };

        public List<string> InvalidWords => throw new NotImplementedException();

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            var source = productsInfo.ToList();

            foreach (var line in source)
            {
                bool isEmpty = string.IsNullOrEmpty(line.Model.Trim());

                var uniqueList = source.Where(p => p.Model != null).GroupBy(p => p.Model.Trim()).ToDictionary(i => i.Key, j => j.Count());

                int uniqueCount = isEmpty ? 0 : uniqueList[line.Model.Trim()];
                
                int linesCount = (!isEmpty && uniqueCount == 1) ? 3 : 2;

                var section = CreateSection(line, startGroupSectionNumber++, linesCount);

                sb.Append(section);               
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount = 3)
        {
            productInfo.Name = productInfo.Name.Replace($" Wiha {productInfo.Sku.Replace("WI-", string.Empty)}", string.Empty).Trim();

            var data = new YandexMarketSection(this, typeof(WihaYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class WihaYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public WihaYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            return $"{Manufacturer}-{Product.Sku}".ToUpper();
        }

        protected override string GetGroupName()
        {
            string groupName = $"{Product.Sku} {Product.ProductTypeShort} {Manufacturer}";

            return groupName;
        }

        protected override string GetTitle1()
        {
            var title = $"{parentSection.ProductLine.Sku} {parentSection.ProductLine.ProductTypeShort} {parentSection.Manufacturer}";

            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{parentSection.ProductLine.Sku} {parentSection.Manufacturer}";

            return title;
        }

        protected override string GetTitle3()
        {
            Product.Model = string.IsNullOrWhiteSpace(Product.Model) ? null : (" " + Product.Model);

            string title = null;

            decimal roundedPrice = Product.Price;

            if (Product.Price != decimal.Zero)
            {
                title = $"{parentSection.ProductLine.Sku} {Product.ProductTypeShort} {parentSection.Manufacturer}{Product.Model ?? string.Empty}. Цена {roundedPrice} руб, в наличии, доставка по России!";

                if (title.Length > TITLE3_MAX_LENGTH)
                {
                    title = title.Replace(", в наличии, доставка по России!", string.Empty);
                }
            }
            else
            {
                title = $"{parentSection.ProductLine.Sku} {Product.ProductTypeShort} {parentSection.Manufacturer}{Product.Model ?? string.Empty}. В наличии, отправка по России!";

                if (title.Length > TITLE3_MAX_LENGTH)
                {
                    title = title.Replace(". В наличии, отправка по России!", string.Empty);
                }
            }

            title = Regex.Replace(title, " +", " ").Trim();

            if(title.Length > TITLE3_MAX_LENGTH)
            {
                title = title.Replace($" {parentSection.Manufacturer} ", " ");

                if (title.Length > TITLE3_MAX_LENGTH)
                {
                    throw new ArgumentOutOfRangeException($"Длина строки должна быть не более {TITLE3_MAX_LENGTH}");
                }
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string result = null;

            if (lineNumber == 1)
            {
                result = Product.Sku.RemoveInvalidCharsInYandexKeyPhrase();
            }
            else if (lineNumber == 2)
            {
                string modelString = (parentSection.Count() == 2) ? Product.Sku.Replace("WI ", string.Empty) : Product.Model;

                result = $"{Product.ProductTypeShort} {Manufacturer} {modelString}".RemoveInvalidCharsInYandexKeyPhrase();
            }
            else if (lineNumber == 3)
            {
                result = $"{Manufacturer} {Product.Model}".RemoveInvalidCharsInYandexKeyPhrase();
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
                File.AppendAllLines("errors_wiha.txt", new string[] { $"[{DateTime.Now.ToShortTimeString()}] {parentSection.ProductLine.Sku}" });
            }

            return result;
        }
    }
}
