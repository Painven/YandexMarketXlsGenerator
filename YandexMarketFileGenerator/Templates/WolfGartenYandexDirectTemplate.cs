using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Wolf Garten")]
    public class WolfGartenYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Wolf Garten";
        public string TranslatedManufacturer => throw new NotImplementedException();
        public List<string> InvalidWords => throw new NotImplementedException();    

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Санкт-Петербург и Ленинградская область",
            ["T"] = "",
            ["U"] = "",
            ["W"] = "+",
            ["X"] = "Активно",
            ["Y"] = "Работает везде",
            ["Z"] = "Wolf Garten||Газонокосилки||Мини трактора||Вертикуттеры Wolf Garten||Снегоуборщики Wolf Garten",
            ["AA"] = "Садовая техника||Для кустов и деревьев||Веерные, газонные, малые||для полива||Штыковые, зимние||ручные",
            ["AB"] = "https://etk-komplekt.ru/wolf-garten||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/nozhovki-pily-sadovye/sekatory-wolf-garten/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/grabli/grabli-wolf-garten/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/shlangi/shlangi-wolf-garten/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/lopaty/lopaty-wolf-garten/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/aeratory/aeratory-wolf-garten/",
            ["AK"] = "Доступные цены||официальный дилер||доставка"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int lines = (line.Sku == line.Model || !line.IsUniquePhrase) ? 4 : 5;

                sb.Append(CreateSection(line, startGroupSectionNumber++, lines));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(WolfGartenYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class WolfGartenYandexMarketSectionLine : YandexMarketSectionLineBase
    {

        public WolfGartenYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            return $"{Manufacturer.ToUpper()} {Product.Model}";
        }

        protected override string GetViewedUrl()
        {
            string url = $"{Manufacturer} {Product.Model}".ToUpper().ToViewedUrl();
            if(url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url;
        }

        protected override string GetTitle1()
        {
            //Wolf Garten SDL 2800 EVO

            var title = $"{Manufacturer} {Product.Model}".Trim();
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //Измельчитель SDL 2800 EVO
            var title = $"{Product.ProductTypeShort} {Product.Model}".Trim();
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            //Измельчитель садовый электрический Wolf Garten SDL 2800 EVO с доставкой по России!

            string articleAdditional = Product.Model == Product.Sku ? "с доставкой по России!" : $"(арт. {Product.Sku})";
            var title = $"{Product.ProductTypeFull} {Product.Model} {Manufacturer} {articleAdditional}";
            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            //SDL 2800 EVO
            //wolf garten 2800
            //Измельчитель Wolf Garten SDL 2800 EVO
            //Wolf Garten SDL 2800 EVO - измельчитель
            //Измельчитель SDL 2800
            //garten 2800 - wolf

            bool isUniueModel = Product.Model.Length >= 6;
            bool isSkuNotSame = Product.Model != Product.Sku;

            string phrase;

            if (lineNumber == 1)
            {
                phrase = (isUniueModel ? Product.Model : $"{Manufacturer} {Product.Model}");
            }
            else if (lineNumber == 2)
            {
                phrase = $"{Manufacturer} {Product.Model} -{Product.ProductTypeShort}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Product.ProductTypeShort} {Manufacturer} {Product.Model}";
            }
            else if (lineNumber == 4)
            {
                phrase =  $"{Manufacturer} {Product.Sku}";
            }
            else if (lineNumber == 5)
            {
                phrase = Product.Sku;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase.ToLower().Trim();
        }
    }
}
