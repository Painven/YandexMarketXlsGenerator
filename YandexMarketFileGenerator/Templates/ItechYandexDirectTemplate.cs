using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для ITECH")]
    public class ItechYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://itechate.su";
        public string Manufacturer { get; } = "ITECH";
        public string TranslatedManufacturer => "Айтэк";
        public List<string> InvalidWords => throw new NotImplementedException();

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["T"] = "182659790278",
            ["U"] = "",
            ["W"] = "+",
            ["X"] = "Активно",
            ["Y"] = "Работает везде",
            ["Z"] = "Itech||Измерители мощности||Источники питания||ИП||Электронные нагрузки Itech",
            ["AA"] = "Вся продукция||||переменного тока||постоянного тока||",
            ["AB"] = "https://itechate.su/||https://itechate.su/izmeriteli-moshhnosti/||https://itechate.su/istochniki-pitaniya-peremennogo-toka/||https://itechate.su/istochniki-pitaniya-postoyannogo-toka/||https://itechate.su/elektronnye-nagruzki/",
            ["AK"] = "официальный дилер||в наличии||Выставочный зал в СПб||Доставка по России||источники питания"
        };
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = 3;
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(ItechYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class ItechYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public ItechYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
            Product.Manufacturer = Manufacturer;
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
            return Product.Model;
        }

        protected override string GetViewedUrl()
        {
            string url = $"{Product.Model} {Product.Manufacturer}".ReplaceAll(new[] { " ", ".", "/", "_" }, newSubString: "-");

            if (url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                url = Product.Model.ReplaceAll(new[] { " ", ".", "/", "_" }, newSubString: "-");
            }

            if (url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url.ToUpper();
        }

        protected override string GetTitle1()
        {
            //IT9121E ITECH Измеритель мощности

            var title = $"{Product.Model} {Product.Manufacturer} {Product.ProductTypeShort}";

            return title;
        }

        protected override string GetTitle2()
        {
            //IT9121E ITECH

            var title = $"{Product.Model} {Product.Manufacturer}";

            return title;
        }

        protected override string GetTitle3()
        {
            //IT9121E ITECH Измеритель мощности от официального дилера с доставкой по России

            var title = $"{Product.Model} {Product.Manufacturer} {Product.ProductTypeFull} от официального дилера с доставкой по России";
            if(title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Product.Model} {Product.Manufacturer} {Product.ProductTypeFull} от официального дилера";

                if (title.Length >= TITLE3_MAX_LENGTH)
                {
                    title = $"{Product.Model} {Product.Manufacturer} {Product.ProductTypeFull} в наличии";
                }
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        { 
            string phrase = null;

            //itech IT9121E
            //IT9121E Измеритель мощности
            //IT9121E -itech

            if (lineNumber == 1)
            {
                phrase = $"itech {Product.Model}";
            }
            else if (lineNumber == 2)
            {
                phrase = $"{Product.Model} {Product.ProductTypeShort}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Product.Model} -itech";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase.ToLower().Trim();
        }
    }
}
