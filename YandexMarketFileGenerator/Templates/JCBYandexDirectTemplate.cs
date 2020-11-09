using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для JCB")]
    public class JCBYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "JCB";
        public string TranslatedManufacturer => "Джисиби";
        public List<string> InvalidWords => throw new NotImplementedException();

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["T"] = "",
            ["U"] = "",
            ["W"] = "+",
            ["X"] = "Активно",
            ["Y"] = "Работает везде",
            ["Z"] = "JCB наборы ручного инструмента||отвертки||уровни||сумки JCB",
            ["AA"] = "JCB наборы ручного инструмента для ремонтных работ||хромомолибденовая сталь, двухкомпонентные, для точных работ||коробчатые, алюминевые||сумки для инструмента",
            ["AB"] = "https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabory-instrumentov-jcb/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/otvertka/otvertki-jcb/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/urovni/urovni-jcb/||https://etk-komplekt.ru/instrumenty/hranenie-instumenta/sumki-i-ryukzaki-dlya-detaley/sumki-jcb/",
            ["AK"] = "Доступные цены||официальный дилер||Гарантия качества||доставка"
        };
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = 5;
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(JCBYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class JCBYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public JCBYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
            Product.Manufacturer = "JCB";
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
            return $"{Manufacturer.ToUpper()} {Product.Model}";
        }

        protected override string GetViewedUrl()
        {
            string url = $"{Manufacturer} {Product.Model}".ReplaceAll(new[] { " ", ".", "/", "_" }, newSubString: "-");

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
            var title = $"JCB {Product.Model} {Product.ProductTypeShort}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"JCB {Product.Model}";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            var title = $"JCB {Product.Model} {Product.ProductTypeFull} с доставкой по России!";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"JCB {Product.Model} {Product.ProductTypeFull} в наличии!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {

            string phrase = string.Empty;

            if (lineNumber == 1)
            {
                phrase = $"{Product.Manufacturer} {Product.Model} {Product.ProductTypeShort}";
            }
            else if (lineNumber == 2)
            {
                phrase = $"{TranslatedManufacturer} {Product.Model} {Product.ProductTypeShort}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Product.Manufacturer} {Product.Model}";
            }
            else if (lineNumber == 4)
            {
                phrase = $"{TranslatedManufacturer} {Product.Model}";
            }
            else if (lineNumber == 5)
            {
                phrase = $"{Product.Model}";
            }
            else if (lineNumber == 6)
            {
                phrase = $"{Product.Model} -{TranslatedManufacturer}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase.ToLower().Trim();
        }
    }
}
