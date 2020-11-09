using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Stayer")]
    public class StayerYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Stayer";
        public string TranslatedManufacturer => "Стайер";
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
            ["Z"] = "Stayer наборы инструмента||Отвертки||Очки||Ключи||Кисти||Клеевые пистолеты Stayer",
            ["AA"] = "Универсальные наборы и наборы для ремонта Stayer||Отвертки Stayer. Ударные, сменные, диэлектрические||Защитные очки для сварки, защиты от частиц и с вентиляцией||Инструментальные ключи||Малярные кисти, плоские и круглые||Электрические клеевые пистолеты и стержни",
            ["AB"] = "https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabory-instrumentov-stayer/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/otvertka/otvertki-stayer||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/sredstva-individualnoy-zashchity/sredstva-individualnoj-zashhity-stayer/zashhitnye-ochki-stayer/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/instrumentalnye-klyuchi/klyuchi-stayer/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/malyarnyj-instrument/kisti/kisti-stayer/||https://etk-komplekt.ru/instrumenty/elektroinstrument/kleevye-pistolety/kleevye-pistolety-stayer/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||Немецкое качество"
        };
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = line.IsUniquePhrase ? 4 : 2; 
                if(!Regex.IsMatch(line.Sku, @"^(\d+)$"))
                {
                    count++;
                }
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(StayerYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }
       
    }

    internal class StayerYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public StayerYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
            Product.Manufacturer = "Stayer";
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

            return url.ToUpper();
        }

        protected override string GetTitle1()
        {
            //Stayer Sku ShortName 

            var title = $"Stayer {Product.Sku} {Product.ProductTypeShort}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //ЗУБР ЗПТ-305-1800 ПЛ

            string modelOrSku = !string.IsNullOrEmpty(Product.Model) ? $"{Product.Model} {Product.Sku}" : Product.Sku;
            var title = $"{Product.Manufacturer} {modelOrSku}";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                title = $"{Product.Manufacturer} {Product.Sku}";
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            //ЗУБР ЗПТ-305-1800 ПЛ пила торцовочная с доставкой по России!
            string modelOrSku = !string.IsNullOrEmpty(Product.Model) ? Product.Model : Product.Sku;

            var title = string.Empty;
            if(modelOrSku == Product.Model)
            {
                title = $"{Product.Manufacturer} {Product.Model} (арт. {Product.Sku}) {Product.ProductTypeFull} с доставкой по России!";
            }
            else
            {
                title = $"{Product.Manufacturer} {modelOrSku} {Product.ProductTypeFull} с доставкой по России!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Product.Manufacturer} {Product.Model} {Product.ProductTypeFull} в наличии!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            //Stayer артикул
            //Стайер артикул
            //Stayer модель артикул
            //модель артикул

            string phrase = string.Empty;

            if (lineNumber == 1)
            {
                phrase = $"{Product.Manufacturer} {Product.Sku}";
            }
            else if (lineNumber == 2)
            {
                phrase = $"{TranslatedManufacturer} {Product.Sku}";
            }
            else if (lineNumber == 3 && Product.IsUniquePhrase)
            {
                phrase = $"{Product.Manufacturer} {Product.Model} {Product.Sku}";
            }
            else if (lineNumber == 4 && Product.IsUniquePhrase)
            {
                phrase = $"{Product.Model} {Product.Sku}";
            }
            else if((lineNumber == 5 || lineNumber == 3) && !Regex.IsMatch(Product.Sku, @"^(\d+)$"))
            {
                phrase = Product.Sku;
            }

            return phrase.ToLower().Trim();
        }
    
    }
}
