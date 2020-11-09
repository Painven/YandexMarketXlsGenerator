using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Testo")]
    public class TestoYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Testo";
        public string TranslatedManufacturer { get; } = "Тэсто";
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
            ["Y"] = "Testo Тепловизоры ||Приборы Testo||Газоанализаторы||Пирометры Testo",
            ["Z"] = "делают невидимое для глаз ИК (тепловое) излучение видимым||Весь спектр оборудования от официального дилера||Предназначены для анализа дымовых газов||Для измерения температуры тел бесконтактным способом",
            ["AA"] = "https://etk-komplekt.ru/izmeritelnye-pribory/teplovizory/teplovizory-testo/||https://etk-komplekt.ru/testo||https://etk-komplekt.ru/izmeritelnye-pribory/gazoanalizatory/gazoanalizatory-testo/||https://etk-komplekt.ru/izmeritelnye-pribory/pirometry/pirometry-testo/",
            ["AJ"] = "официальный дилер||доставка||Поверка||Приборы Testo"
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
            var data = new YandexMarketSection(this, typeof(TestoYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }
    }

    internal class TestoYandexMarketSectionLine : YandexMarketSectionLineBase
    {
 
        public TestoYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            return Product.Model.ToLower();
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
            //Термогигрометр Testo 608-H1
            var title = $"{Product.ProductTypeShort} {Product.Model}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //Testo 608-H1
            var title = $"{Product.Model}";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            var title = $"{Product.ProductTypeFull} {Product.Model} от официального дилера с доставкой по России";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string phrase;

            //термогигрометр testo 608 h1
            //testo 608 h1 -термогигрометр


            if (lineNumber == 1)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Model}";
            }
            else if (lineNumber == 2)
            {

                phrase = $"{Product.Model} -{Product.ProductTypeShort}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase.ToLower();
        }
    }
}
