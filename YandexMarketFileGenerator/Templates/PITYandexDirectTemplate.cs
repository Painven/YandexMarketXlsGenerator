using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для PIT")]
    public class PITYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "PIT";
        public string TranslatedManufacturer => "Пит";
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
            ["Z"] = "PIT шуруповерты||триммеры||сварочные аппараты||пилы||перфораторы||полуавтоматы||бензопилы||дрели PIT",
            ["AA"] = "шуруповерты аккумуляторные, сетевые, дрели-шуруповерты||триммеры бензиновые, электрические и аккумуляторные||сварочные аппараты, полуавтоматы, инверторы||пилы сабельные, цепные, настольные, отрезные, торцовочные||сетевые перфораторы, для бетона, сверления дерева и металла||сварочные полуавтоматы||пилы цепные бензиновые серии Стандарт, Мастер||электродрели, ударные, дрели-миксеры, дрели-шуруповерты",
            ["AB"] = "https://etk-komplekt.ru/instrumenty/elektroinstrument/shurupoverty/shurupoverty-pit/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/trimmery/trimmery-pit/||https://etk-komplekt.ru/stroitelstvo/svarochnoe-oborudovanie/svarochnye-apparaty/svarochnye-apparaty-pit/||https://etk-komplekt.ru/instrumenty/elektroinstrument/elektropily/pily-pit/||https://etk-komplekt.ru/instrumenty/elektroinstrument/perforatory-i-otboynye-molotki/perforatory/perforatory-pit/||https://etk-komplekt.ru/stroitelstvo/svarochnoe-oborudovanie/svarochnye-poluavtomaty-pit/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/czepnye-pily/cepnye-pily-pit/pily-cepnye-benzinovye-pit/||https://etk-komplekt.ru/instrumenty/elektroinstrument/dreli/dreli-pit/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||в наличии"
        };
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 6));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(PITYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class PITYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public PITYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection) { }

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
            var title = $"{Manufacturer} {Product.Model} {Product.ProductTypeShort}";

            return title;
        }

        protected override string GetTitle2()
        {
            string title =  $"{Manufacturer} {Product.Model}";

            return title;
        }

        protected override string GetTitle3()
        {
            string title = $"{Manufacturer} {Product.Model} {Product.ProductTypeFull} с доставкой по России!";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Manufacturer} {Product.Model} {Product.ProductTypeFull} в наличии";
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
                phrase = $"{Manufacturer} {Product.Model}";
            }
            else if(lineNumber == 2)
            {
                phrase = $"P.I.T. {Product.Model}";
            }
            else if(lineNumber == 3)
            {
                phrase = $"{TranslatedManufacturer} {Product.Model}";
            }
            else if (lineNumber == 4)
            {
                phrase = $"{Manufacturer} {Product.ProductTypeShort} {Product.Model}";
            }
            else if (lineNumber == 5)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Model}";
            }
            else if (lineNumber == 6)
            {
                phrase = $"{Product.Model}";
            }
            else
            {
                throw new NotImplementedException();
            }

            return phrase.ToLower().Trim();
        }
    }
}
