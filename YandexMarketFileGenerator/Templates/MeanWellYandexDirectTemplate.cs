using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для MeanWell")]
    public class MeanWellYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://meanwellpower.ru";
        public string Manufacturer { get; } = "Mean Well";
        public string TranslatedManufacturer { get; } = "Мин Велл";
        public List<string> InvalidWords => throw new NotImplementedException();

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["T"] = "30",
            ["V"] = "+",
            ["W"] = "Активно",
            ["X"] = "Работает везде",
            ["Y"] = "AC/DC блоки питания||DC/AC инверторы||DC/DC преобразователи||LED-драйвер",
            ["Z"] = "Mean Well блоки питания||Mean Well инверторы||Mean Well преобразователи||LED-драйверы Mean Well для светодиодной техники",
            ["AA"] = "https://meanwellpower.ru/acdc-setevye-bloki-pitaniya/||https://meanwellpower.ru/dcac-invertory/||https://meanwellpower.ru/dcdc-preobrazovateli/||https://meanwellpower.ru/led-drajvery-dlya-svetodiodnoj-tekhniki/",
            ["AJ"] = "официальный дилер||источники питания||блоки питания||Россия"
        };

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
            var data = new YandexMarketSection(this, typeof(MeanWellYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }
    }

    internal class MeanWellYandexMarketSectionLine : YandexMarketSectionLineBase
    {
 
        public MeanWellYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
        }

        protected override string GetGroupName()
        {
            if(!string.IsNullOrWhiteSpace(Product.Model))
            {
                return Product.Model;
            }
            else if (!string.IsNullOrWhiteSpace(Product.Sku))
            {
                return Product.Sku;
            }
            else
            {
                throw new FormatException();
            }
        }

        protected override string GetViewedUrl()
        {
            string url = Product.Model.ReplaceAll(new[] { " ", ".", "/", "_" },  newSubString: "-");
            if(url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url;
        }

        protected override string GetTitle1()
        {
            
            var title = $"{Product.ProductTypeShort} {Manufacturer} {Product.Model}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {

            

            var title = $"{Manufacturer} {Product.Model}";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            //Источник питания AC/DC Mean Well DRC-100A от официального дилера с доставкой!

            var title = $"{Product.ProductTypeFull} {Manufacturer} {Product.Model} от официального дилера с доставкой!";
            if(title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title.Replace(" с доставкой!", string.Empty);

                if (title.Length >= TITLE3_MAX_LENGTH)
                {
                    throw new FormatException("Превышена допустимая длина: " + title);
                }
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string phrase;

            //DRC-100A
            //Mean Well DRC-100A

            if (lineNumber == 1)
            {
                phrase = $"{Product.Model}";
            }
            else if (lineNumber == 2)
            {

                phrase = $"{Manufacturer} {Product.Model}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase;
        }
    }
}
