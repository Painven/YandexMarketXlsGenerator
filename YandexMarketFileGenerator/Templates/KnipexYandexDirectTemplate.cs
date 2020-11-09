using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Knipex")]
    public class KnipexYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Knipex";
        public string TranslatedManufacturer { get; } = "Книпекс";
        public List<string> InvalidWords => throw new NotImplementedException();

        public static readonly IReadOnlyList<string> VALID_KNIPEX_KEYS = new List<string>()
        {

        }.Distinct().OrderBy(i => i).ToList();
    

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["T"] = "",
            ["U"] = "633.50",
            ["V"] = "354.30",
            ["W"] = "+",
            ["X"] = "Активно",
            ["Y"] = "Работает везде",
            ["Z"] = "Книпекс||Клещи||Ключи||Бокорезы||Ножи||Кусачки||Стрипперы Knipex||Плоскогубцы Knipex",
            ["AA"] = "Весь инструмент||обжимные||Наборы, торцевые, инструментальные||||Для изоляции, подвижные, сменные ||||для снятия изоляции||Книпекс",
            ["AB"] = "https://etk-komplekt.ru/knipex||https://etk-komplekt.ru/ruchnoy-instrument/ploskogubcy-kleshchi-kusachki/kleshchi-perestavnye/perestavnye-kleshhi-knipex/||https://etk-komplekt.ru/nabory-instrumentov/nabory-instrumentov-knipex/nabory-klyuchej-knipex/||https://etk-komplekt.ru/ruchnoy-instrument/ploskogubcy-kleshchi-kusachki/bokorezy-i-kusachki/bokorezy-knipex/||https://etk-komplekt.ru/ruchnoy-instrument/zapasnye-chasti/zapasnye-chasti-knipex/nozhi-knipex/||https://etk-komplekt.ru/ruchnoy-instrument/ploskogubcy-kleshchi-kusachki/kusachki/kusachki-knipex/||https://etk-komplekt.ru/ruchnoy-instrument/ploskogubcy-kleshchi-kusachki/stripper/strippery-knipex/||https://etk-komplekt.ru/ruchnoy-instrument/ploskogubcy-kleshchi-kusachki/ploskogubcy/ploskogubczy-knipex/",
            ["AK"] = "доставка||официальный дилер||Доступные цены"
        };


        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                var linesCount = !string.IsNullOrEmpty(line.Model) ? 5 : 3;
                sb.Append(CreateSection(line, startGroupSectionNumber++, linesCount));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(KnipexYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }
   
    }

    internal class KnipexYandexMarketSectionLine : YandexMarketSectionLineBase
    {
 
        public KnipexYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            resultDictionary["F"] = parentSection.GroupIndex.ToString();
            resultDictionary["I"] = GetPhrase(lineNumber);
            resultDictionary["K"] = GetTitle1();
            resultDictionary["L"] = GetTitle2();
            resultDictionary["M"] = GetTitle3();
            resultDictionary["Q"] = FullUrlPath;
            resultDictionary["R"] = GetViewedUrl();
            resultDictionary["AV"] = Product.Price == decimal.Zero ? "" : Product.Price.ToString("F0");
        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer.ToUpper()} {Product.Sku}";
        }

        protected override string GetViewedUrl()
        {
            string url = $"{Manufacturer} {Product.Sku}".ToUpper().ReplaceAll(new[] { " ", ".", "/", "_" }, newSubString: "-");
            if(url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                url = url.Replace("-KN-", "-");
                if (url.Length >= VIEWED_URL_MAX_LENGTH)
                {
                    throw new FormatException("Превышена допустимая длина: " + url);
                }
            }

            return url;
        }

        protected override string GetTitle1()
        {
            var title = ("KNIPEX " + Product.ProductTypeFull + " " + (Product.Model ?? string.Empty)).Trim();
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                title = ("KNIPEX " + Product.ProductTypeShort + " " + (Product.Model ?? string.Empty)).Trim();
                if (title.Length >= TITLE1_MAX_LENGTH)
                {
                    title = "KNIPEX " + Product.ProductTypeShort;
                    if (title.Length >= TITLE1_MAX_LENGTH)
                    {
                        throw new FormatException("Превышена допустимая длина: " + title);
                    }
                }
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //KN-8702250 KNIPEX Cobra
            var title = $"{Product.Sku} {Manufacturer.ToUpper()} {Product.Model ?? string.Empty}".Trim();
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                title = $"{Product.Sku} {Manufacturer.ToUpper()}";
                if (title.Length >= TITLE2_MAX_LENGTH)
                {
                    throw new FormatException("Превышена допустимая длина: " + title);
                }
            }
            return title;
        }

        protected override string GetTitle3()
        {
            //KNIPEX Cobra клещи черненые 250 mm (KN-8702250) с доставкой по России!
            var title = $"{Manufacturer.ToUpper()} {Product.ProductTypeFull} ({Product.Sku}) с доставкой по России!";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string phrase;

            //KNIPEX KN-8702250
            //KN-8702250 -knipex
            //Cobra 250 mm
            //KNIPEX Cobra 250
            //книпекс 250
            //KNIPEX 250 -cobra


            if (lineNumber == 1)
            {
                phrase = $"{Manufacturer.ToUpper()} {Product.Sku}";
            }
            else if (lineNumber == 2)
            {
                phrase = $"{Product.Sku} -{Manufacturer.ToLower()}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{TranslatedManufacturer.ToLower()} {Product.Sku}";
            }
            else if (lineNumber == 4)
            {
                if(string.IsNullOrWhiteSpace(Product.Model))
                {
                    throw new FormatException();
                }

                phrase = Product.Model;
            }
            else if (lineNumber == 5)
            {
                phrase = $"{Manufacturer.ToUpper()} {Product.Model}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase;
        }
    }
}
