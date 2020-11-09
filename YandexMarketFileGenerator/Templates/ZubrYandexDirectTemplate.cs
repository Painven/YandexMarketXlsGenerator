using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Zubr")]
    public class ZubrYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "ЗУБР";
        public string TranslatedManufacturer => throw new NotImplementedException();
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
            ["Z"] = "ЗУБР Мастер инструменты||Пилы торцовочные||Сабельные пилы||Мотоблоки||Шуруповерты||Станки||Насосы||Перфораторы ЗУБР",
            ["AA"] = "Весь инструмент||ЗУБР ЗПТ||ЗУБ ЗПС и СПЛ||Бензиновые МТБ и МТУ усиленные||Сетевые и аккумуляторные ||Для обработки металла и дерева||Дренажные, погружные, фекальные, садовые||SDS",
            ["AB"] = "https://etk-komplekt.ru/zubr||https://etk-komplekt.ru/elektroinstrument/elektropily/pily-torcevye/pily-torcovochnye-zubr/||https://etk-komplekt.ru/elektroinstrument/elektropily/sabelnye-pily/sabelnye-pily-zubr/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/kultivatory/motobloki-zubr/||https://etk-komplekt.ru/elektroinstrument/shurupoverty/shurupoverty-zubr/||https://etk-komplekt.ru/elektroinstrument/stanki/stanki-zubr/||https://etk-komplekt.ru/nasosy-i-nasosnye-stancii/nasosy-zubr/||https://etk-komplekt.ru/elektroinstrument/perforatory-i-otboynye-molotki/perforatory/perforatory-zubr/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||Сделано в России||Гарантия производителя"
        };
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = line.Model.Split().Length >= 2 ? 5 : 3;
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(ZubrYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class ZubrYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public ZubrYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            string url = $"{Manufacturer} {Product.Model}".ToViewedUrl();

            if (url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                url = Product.Model.ToViewedUrl();
            }

            if (url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url.ToUpper();
        }

        protected override string GetTitle1()
        {
            //ЗУБР ЗПТ-305-1800 ПЛ пила 

            var title = $"ЗУБР {Product.Model} {Product.ProductTypeShort}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //ЗУБР ЗПТ-305-1800 ПЛ

            var title = $"ЗУБР {Product.Model}";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            //ЗУБР ЗПТ-305-1800 ПЛ пила торцовочная с доставкой по России!

            var title = $"ЗУБР {Product.Model} {Product.ProductTypeFull} с доставкой по России!";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"ЗУБР {Product.Model} {Product.ProductTypeFull} в наличии!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            //ЗУБР ЗПТ-305-1800 ПЛ пила торцовочная
            //ЗУБР ЗПТ-305-1800 ПЛ
            //     ЗПТ-305-1800 ПЛ -зубр
            //ЗУБР ЗПТ-305-1800 -пл
            //     ЗПТ-305-1800 -зубр -пл

            string phrase;
            string modelWithoutLastWord = null, lastWord = null;

            if (lineNumber > 3)
            {
                modelWithoutLastWord = Product.Model.Substring(0, Product.Model.LastIndexOf(" "));
                lastWord = Product.Model.Split().Last();
            }

            if (lineNumber == 1)
            {
                phrase = $"ЗУБР {Product.Model} {Product.ProductTypeShort}";
            }
            else if (lineNumber == 2)
            {
                phrase = $"ЗУБР {Product.Model} {Product.ProductTypeShort}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Product.Model} {Product.ProductTypeShort} -зубр";
            }
            else if (lineNumber == 4)
            {             
                phrase = $"ЗУБР {modelWithoutLastWord} -{lastWord}";
            }
            else if (lineNumber == 5)
            {
                phrase = $"{modelWithoutLastWord} -зубр -{lastWord}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase.ToLower().Trim();
        }
    }
}
