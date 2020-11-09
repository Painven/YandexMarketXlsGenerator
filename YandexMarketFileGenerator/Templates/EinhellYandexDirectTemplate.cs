using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Einhell")]
    public class EinhellYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Einhell";
        public string TranslatedManufacturer => "Эйнхель";
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
            ["Z"] = "EINHELL газонокосилки||перфораторы||пилы||пылесосы||станки||торцовочные пилы||триммеры||шуруповерты EINHELL",
            ["AA"] = "газонокосилки электрические, бензиновые и механические||перфораторы сетевые, аккумуляторные, отбойные молотки||перфораторы сетевые, аккумуляторные, отбойные||пылесосы садовые, строительные, пылесосы-воздуходувы||шлифовальные, заточные, сверлильные, рейсмусовые токарные||пилы торцовочные с площадкой и без||триммеры электрические, бензиновые и аккумуляторные||сетевые, аккумуляторные, шуруповерты-отвертки",
            ["AB"] = "https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/gazonokosilki/gazonokosilki-einhell/||https://etk-komplekt.ru/instrumenty/elektroinstrument/perforatory-i-otboynye-molotki/perforatory/perforatory-einhell/||https://etk-komplekt.ru/instrumenty/elektroinstrument/perforatory-i-otboynye-molotki/perforatory/perforatory-einhell/||https://etk-komplekt.ru/instrumenty/elektroinstrument/pylesosy/pylesosy-einhell/||https://etk-komplekt.ru/instrumenty/elektroinstrument/stanki/stanki-einhell/||https://etk-komplekt.ru/instrumenty/elektroinstrument/elektropily/pily-torcevye/pily-torczovochnye-einhell/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/trimmery/trimmery-einhell/||https://etk-komplekt.ru/instrumenty/elektroinstrument/shurupoverty/shurupoverty-einhell/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||Немецкое качество"
        };
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = line.IsUniquePhrase ? 8 : 4;

                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(EinhellYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class EinhellYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public EinhellYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
            Product.Manufacturer = "Einhell";
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
            string url = $"{Manufacturer} {Product.Sku}".ToViewedUrl();

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
            var title = $"{Product.Manufacturer} {Product.Sku} {Product.ProductTypeShort}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            string title = null;

            if(Product.Model != Product.Sku)
            {
                title = $"{Product.Manufacturer} {Product.Model} {Product.Sku}";
            }
            else
            {
                title = $"{Product.Manufacturer} {Product.Sku}";
            }

            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            string title = null;

            if(Product.IsUniquePhrase)
            {
                title = $"{Product.Manufacturer} {Product.Model} (арт. {Product.Sku}) {Product.ProductTypeFull} с доставкой по России!";
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
            else if (lineNumber == 2)
            {
                phrase = $"{TranslatedManufacturer} {Product.Sku}";              
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Sku}";
            }
            else if (lineNumber == 4)
            {
                phrase = Product.Sku;
            }
            else if (lineNumber == 5)
            {
                phrase = Product.Model;
            }
            else if (lineNumber == 6)
            {
                phrase = $"{Product.Manufacturer} {Product.Model}";
            }
            else if (lineNumber == 7)
            {
                phrase = $"{TranslatedManufacturer} {Product.Model}";
            }
            else if (lineNumber == 8)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Model}";
            }
            else
            {
                throw new NotImplementedException();
            }

            return phrase.ToLower().Trim();
        }
    }
}
