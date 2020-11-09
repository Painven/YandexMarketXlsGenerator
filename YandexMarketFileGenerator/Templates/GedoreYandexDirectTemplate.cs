using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Gedore")]
    public class GedoreYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Gedore";
        public string TranslatedManufacturer => "Гедоре";
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
            ["Z"] = "GEDORE наборы инструмента||Ключи||головки||данамометрические ключи||съемники||наборы головок||отвертки GEDORE",
            ["AA"] = "GEDORE наборы ручного и автомобильного инструмента||инструментальные ключи, рожковые, динамометрические, гаечные||головки отверточные, ударные, торцевые, VDE, удлиненные||данамометрические ключи, электронные, VDE-ключи||съемники универсальные, промышленные, гидравлические, наборы||наборы торцевых головок с битами, наборы на рейке||отвертки динамометрические, индикаторные, ударные",
            ["AB"] = "https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabory-instrumentov-gedore/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/instrumentalnye-klyuchi/klyuchi-gedore/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/golovki/golovki-gedore/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/instrumentalnye-klyuchi/dinamometricheskij-klyuch/dinamometricheskie-klyuchi-gedore/||https://etk-komplekt.ru/instrumenty/instrument-dlya-avtoservisa/avtomobilnyj-instrument-gedore/semniki-gedore/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabor-bit/nabory-golovok-gedore/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/otvertka/otvertki-gedore/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||Немецкое качество"
        };
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = line.IsUniquePhrase ? 6 : 4;
                
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(GedoreYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }
       
    }

    internal class GedoreYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public GedoreYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection) { }

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

            return url.ToUpper();
        }

        protected override string GetTitle1()
        {
            var title = $"{Manufacturer} {Product.Sku} {Product.ProductTypeShort}";
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

            var title = $"{Manufacturer} {modelOrSku}";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                title = $"{Manufacturer} {Product.Sku}";
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            string modelOrSku = !string.IsNullOrEmpty(Product.Model) ? $"{Product.Sku} {Product.Model}" : Product.Sku;

            string title = $"{Manufacturer} {modelOrSku} {Product.ProductTypeFull} с доставкой по России!";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Manufacturer} {Product.Sku} {Product.ProductTypeFull} в наличии!";
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
                phrase = $"{Manufacturer} {Product.Sku}";
            }
            if (lineNumber == 2)
            {
                phrase = $"{TranslatedManufacturer} {Product.Sku}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Manufacturer} GE-{Product.Sku}";
            }
            else if (lineNumber == 4 && Product.IsUniquePhrase)
            {
                phrase = $"{Manufacturer} {Product.Model} {Product.Sku}";
            }
            else if (lineNumber == 5 && Product.IsUniquePhrase)
            {
                phrase = $"{Product.Model} {Product.Sku}";
            }
            else if (lineNumber == 6 || lineNumber == 4)
            {
                phrase = Product.Sku;
            }

            return phrase.ToLower().Trim();
        }
    
    }
}
