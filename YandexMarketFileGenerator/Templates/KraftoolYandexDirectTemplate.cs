using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Kraftool")]
    public class KraftoolYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Kraftool";
        public string TranslatedManufacturer => "Крафтул";
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
            ["Z"] = "Kraftool инструмент||Ключи||Биты||Заклепочники||Отвертки||Ножницы||Головки||Пистолеты Kraftool",
            ["AA"] = "Весь инструмент||Гаечные, разводные, трубные, наборы||Профессиональные биты в наборе и поштучно||Пневматические, механические||Наборы и штучно: динамометрические, реверсивные||По металлу||Торцевые, в наборе и поштучно||Для монтажной пены, пена Kraftool",
            ["AB"] = "https://etk-komplekt.ru/kraftool||https://etk-komplekt.ru/ruchnoy-instrument/instrumentalnye-klyuchi/klyuchi-kraftool/||https://etk-komplekt.ru/ruchnoy-instrument/bity/bity-kraftool/||https://etk-komplekt.ru/ruchnoy-instrument/steplery-i-zaklepochniki/zaklepochniki-kraftool/||https://etk-komplekt.ru/ruchnoy-instrument/otvertka/otvertki-kraftool/||https://etk-komplekt.ru/ruchnoy-instrument/nozhnicy/nozhnicy-kraftool/||https://etk-komplekt.ru/ruchnoy-instrument/golovki/golovki-kraftool/||https://etk-komplekt.ru/ruchnoy-instrument/pistolety-stroitelnye/pistolety-dlya-peny-kraftool/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||Немецкое качество"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 4));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(KraftoolYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class KraftoolYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public KraftoolYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            return $"{Manufacturer} {Product.Model}";
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

            return url;
        }

        protected override string GetTitle1()
        {
            //Kraftool 22065 Клещи переставные

            var title = $"{Manufacturer} {Product.Model} {Product.ProductTypeShort}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //Крафтул 22065 Клещи 

            var title = $"{TranslatedManufacturer} {Product.Model} {Product.ProductTypeShort}";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            //Kraftool RE 430130 кернер автоматический с доставкой по России
            //Kraftool RE 430130 Цельнометаллическая струбцина c доставкой по России!

            var title = $"{Manufacturer} {Product.Model} {Product.ProductTypeFull} с доставкой по России!";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Manufacturer} {Product.Model} {Product.ProductTypeFull} в наличии!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            //крафтул 22065
            //Kraftool 22065 Клещи -переставной
            //Kraftool 22065 -клещ
            //Kraftool 22065 Клещи переставные



            string phrase;

            if (lineNumber == 1)
            {
                phrase = $"{TranslatedManufacturer} {Product.Model}";
            }
            else if (lineNumber == 2)
            {
                phrase = $"{Manufacturer} {Product.Model}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Manufacturer} {Product.Model} {Product.ProductTypeShort}";
            }
            else if (lineNumber == 4)
            {
                phrase = $"{Product.Model} {Product.ProductTypeShort} -{Manufacturer}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase.ToLower().Trim();
        }
    }
}
