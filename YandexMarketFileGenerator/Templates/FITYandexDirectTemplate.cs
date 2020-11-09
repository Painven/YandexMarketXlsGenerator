using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для FIT")]
    public class FITYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "FIT";
        public string TranslatedManufacturer => "Фит";
        public List<string> InvalidWords => throw new NotImplementedException();

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["T"] = "23605641314",
            ["U"] = "",
            ["W"] = "+",
            ["X"] = "Активно",
            ["Y"] = "Работает везде",
            ["Z"] = "FIT триммеры||дрели||шуруповерт||ящики||набор инструментов||пилы циркулярные||шлифмашины||отвертки FIT",
            ["AA"] = "Триммеры бензиновые и электрические||дрели сетевые, аккумуляторные, дрели-шуруповерты, ударные||шуруповерты сетевые, аккумуляторные, дрели-шуруповерты||ящики и органайзеры для инструмента, пластиковые и алюм.||наборы инструмента, VDE наборы, наборы ключей и головок||электрпилы отрезные (циркулярные)||машины шлифовальные, полировальные, вибрационные||отвертки переставные, для точных работ, наборы отверток",
            ["AB"] = "https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/trimmery/trimmery-fit/||https://etk-komplekt.ru/instrumenty/elektroinstrument/dreli/dreli-fit/||https://etk-komplekt.ru/instrumenty/elektroinstrument/shurupoverty/shurupoverty-fit/||https://etk-komplekt.ru/instrumenty/hranenie-instumenta/yashhiki-dlya-instrumenta/yashchiki-dlya-instrumenta-fit/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabory-instrumentov-fit/||https://etk-komplekt.ru/instrumenty/elektroinstrument/elektropily/elektropily-otreznye/elektropily-otreznye-fit/||https://etk-komplekt.ru/instrumenty/elektroinstrument/shlifmashiny/shlifmashiny-fit/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/otvertka/otvertki-fit/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||в наличии"
        };
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = !string.IsNullOrWhiteSpace(line.Model) ? 6 : 5;

                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(FITYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class FITYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public FITYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
            Product.Manufacturer = "FIT";
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
            string url = $"{Manufacturer} {Product.Sku}".ReplaceAll(new[] { " ", ".", "/", "_" }, newSubString: "-");

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
            var title = $"{Product.Manufacturer} {Product.Sku} {Product.ProductTypeShort}";

            return title;
        }

        protected override string GetTitle2()
        {
            string title =  $"{Product.Manufacturer} {Product.Sku}";

            return title;
        }

        protected override string GetTitle3()
        {
            string title = null;

            if(!string.IsNullOrWhiteSpace(Product.Model))
            {
                title = $"{Product.Manufacturer} {Product.ProductTypeFull} {Product.Model} {Product.Sku} с доставкой по России!";
            }
            else
            {
                title = $"{Product.Manufacturer} {Product.Sku} {Product.ProductTypeFull} с доставкой по России!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Product.Manufacturer} {Product.Sku} {Product.ProductTypeFull} в наличии";
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
                phrase = $"{Product.Manufacturer} {Product.Sku}";
            }
            else if(lineNumber == 2)
            {
                phrase = $"{Product.Manufacturer}-{Product.Sku}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Sku}";
            }
            else if (lineNumber == 4)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Manufacturer} {Product.Sku}";
            }
            else if(lineNumber == 5)
            {
                phrase = $"{TranslatedManufacturer} {Product.Sku}";
            }
            else if(lineNumber == 6)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Model} {Product.Sku}";
            }
            else
            {
                throw new NotImplementedException();
            }

            return phrase.ToLower().Trim();
        }
    }
}
