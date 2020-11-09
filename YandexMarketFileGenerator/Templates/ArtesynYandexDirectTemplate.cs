using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Artesyn")]
    public class ArtesynYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://artesyn.ru";
        public string Manufacturer { get; } = "Artesyn";
        public string TranslatedManufacturer { get; } = "Артесун";
        public List<string> InvalidWords => throw new NotImplementedException();

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["T"] = "230136513907",
            ["U"] = "30",
            ["W"] = "+",
            ["X"] = "Активно",
            ["Y"] = "Работает везде",
            ["Z"] = "AC-DC Источники питания||Преобразователи||Высоковольтные||Медицинские||Конфигурируемые||на DIN-рейку||Маломощные||Железнодорожные",
            ["AA"] = "Внешние, открытые, стоечные, на DIN-рейку, адаптеры||Высоковольтные, железнодорожные, медицинские, промышленные||Изолированные, 2в1, железодорожные||медицинские, 2в1||серии uMP, iHP, iMP, iVS, MP||однофазные, трехфазные||открытого типа, промышленные||высоковольтные",
            ["AB"] = "https://artesyn.su/ac-dc-power-supplies/||https://artesyn.su/dc-dc-converters/||https://artesyn.su/dc-dc-converters/high-voltage-dc-dc-modules/||https://artesyn.su/dc-dc-converters/medical-dc-dc/||https://artesyn.su/ac-dc-power-supplies/configurables/||https://artesyn.su/ac-dc-power-supplies/din-rail/||https://artesyn.su/ac-dc-power-supplies/open-frame-low-power-ac-dc-power-supplies/||https://artesyn.su/dc-dc-converters/railway-dc-dc/",
            ["AK"] = "официальный дилер||источники питания||блоки питания"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 3));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(ArtesynYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }
    }

    internal class ArtesynYandexMarketSectionLine : YandexMarketSectionLineBase
    {
 
        public ArtesynYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            return $"{Manufacturer} {Product.Model}";
        }

        protected override string GetViewedUrl()
        {
            string url = Product.Model.ReplaceAll(new[] { " ", ".", "/", "_" },  newSubString: "-");
            if(url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url;
        }

        protected override string GetTitle1()
        {           
            var title = $"{Product.ProductTypeShort} {Manufacturer} {Product.Model}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {            
            var title = $"{Manufacturer} {Product.Model}";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            var title = $"{Product.ProductTypeFull} {Manufacturer} {Product.Model} с доставкой";
            if(title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException();
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string phrase;

            //DRC-100A
            //Artesyn DRC-100A

            if (lineNumber == 1)
            {
                phrase = $"{Product.Model}";
            }
            else if (lineNumber == 2)
            {
                phrase = $"{Manufacturer} {Product.Model}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Model}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase;
        }
    }
}
