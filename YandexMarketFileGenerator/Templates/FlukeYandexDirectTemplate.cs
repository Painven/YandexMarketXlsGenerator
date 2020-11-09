using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Fluke")]
    public class FlukeYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Fluke";
        public string TranslatedManufacturer => "Флюк";
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
            ["Z"] = "FLUKE мультиметры||серия networks||Тестеры||Клещи||Тепловизоры||Калибраторы||Пирометры||Осциллографы FLUKE",
            ["AA"] = "цифровые мультиметры, мультиметры-осциллографы, прецизионные||для телефона, тестеры, локаторы, портативные устройства||электрические, бесконтактные, для воздуха, аккумуляторные||электроизмерительные, преобразователи тока, выносные||строительные, портативные, для пищевой промышленности||калибраторы электрических величин, давления, температуры||инфракрасные, переносные||осциллографы-мультиметры, USB-осциллографы, компактные",
            ["AB"] = "https://fluke-co.ru/multimetry/||https://fluke-co.ru/fluke-networks/||https://fluke-co.ru/testery/||https://fluke-co.ru/kleshhi-elektroizmeritelnye-i-preobrazovateli-toka/||https://fluke-co.ru/teplovizory/||https://fluke-co.ru/kalibratory/||https://fluke-co.ru/pirometry/||https://fluke-co.ru/oscillografy/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||в наличии"
        };
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, line.IsUniquePhrase ? 8 : 7));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(FlukeYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class FlukeYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public FlukeYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
            Product.Manufacturer = "Fluke";
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
            var title = $"{Product.Manufacturer} {Product.Model} {Product.ProductTypeShort}";

            return title;
        }

        protected override string GetTitle2()
        {
            string title =  $"{Product.Manufacturer} {Product.Model}";

            return title;
        }

        protected override string GetTitle3()
        {
            string title = $"{Product.Manufacturer} {Product.Model} (арт. {Product.Sku}) {Product.ProductTypeFull} с доставкой по России!";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Product.Manufacturer} {Product.Model} {Product.ProductTypeFull} в наличии";
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
                phrase = $"{Product.Manufacturer} {Product.Model}";
            }
            else if(lineNumber == 2)
            {
                phrase = $"{TranslatedManufacturer} {Product.Model}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Product.Manufacturer} {Product.Sku}";
            }
            else if (lineNumber == 4)
            {
                phrase = $"{TranslatedManufacturer} {Product.Sku}";
            }
            else if (lineNumber == 5)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Model}";
            }
            else if (lineNumber == 6)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Sku}";
            }
            else if (lineNumber == 7)
            {
                phrase = Product.Sku;
            }
            else if (lineNumber == 8)
            {
                phrase = Product.Model;
            }
            else
            {
                throw new NotImplementedException();
            }

            return phrase.ToLower().Trim();
        }
    }
}
