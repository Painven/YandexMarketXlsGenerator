using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для ПРАКТИКА")]
    public class PraktikaYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Практика";
        public string Host { get; } = "https://etk-komplekt.ru";
        public string TranslatedManufacturer { get; } = "Практика";

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",       
            ["V"] = "+",
            ["W"] = "Активно",
            ["X"] = "Работает везде",
            ["Y"] = "Инструмент Практика||Сверла||Коронки||Наборы Инструмента Практика",
            ["AA"] = "https://etk-komplekt.ru/praktika||https://etk-komplekt.ru/elektroinstrument/aksessuary-prinadlezhnosti-i-osnastka-dlya-elektroinstrumenta/sverla/sverla-praktika/||https://etk-komplekt.ru/elektroinstrument/aksessuary-prinadlezhnosti-i-osnastka-dlya-elektroinstrumenta/koronki/koronki-almaznye/koronki-almaznye-praktika/||https://etk-komplekt.ru/nabory-instrumentov/nabory-instrumentov-praktika/"
        };

        public List<string> InvalidWords => throw new NotImplementedException();

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 2));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(PraktikaYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(2);
        }
    }

    internal class PraktikaYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public PraktikaYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
        }

        protected override void FillDictionary(int lineNumber)
        {

            resultDictionary["A"] = parentSection.ParentTemplate.ColumnStaticValues["A"];
            resultDictionary["B"] = parentSection.ParentTemplate.ColumnStaticValues["B"];
            resultDictionary["C"] = parentSection.ParentTemplate.ColumnStaticValues["C"];
            resultDictionary["D"] = string.Empty;
            resultDictionary["E"] = GetGroupName();
            resultDictionary["F"] = parentSection.GroupIndex.ToString();
            resultDictionary["G"] = parentSection.ParentTemplate.ColumnStaticValues["G"];
            resultDictionary["H"] = string.Empty;
            resultDictionary["I"] = GetPhrase(lineNumber);
            resultDictionary["J"] = string.Empty;
            resultDictionary["K"] = GetTitle1();
            resultDictionary["L"] = GetTitle2();
            resultDictionary["M"] = GetTitle3();
            resultDictionary["N"] = string.Empty;
            resultDictionary["O"] = string.Empty;
            resultDictionary["P"] = string.Empty;
            resultDictionary["Q"] = FullUrlPath;
            resultDictionary["R"] = GetViewedUrl();
            resultDictionary["S"] = parentSection.ParentTemplate.ColumnStaticValues["S"];
            resultDictionary["T"] = string.Empty;
            resultDictionary["U"] = string.Empty;
            resultDictionary["V"] = parentSection.ParentTemplate.ColumnStaticValues["V"];
            resultDictionary["W"] = parentSection.ParentTemplate.ColumnStaticValues["W"];
            resultDictionary["X"] = parentSection.ParentTemplate.ColumnStaticValues["X"];
            resultDictionary["Y"] = parentSection.ParentTemplate.ColumnStaticValues["Y"];
            resultDictionary["Z"] = string.Empty;
            resultDictionary["AA"] = parentSection.ParentTemplate.ColumnStaticValues["AA"];
            resultDictionary["AB"] = string.Empty;
            resultDictionary["AC"] = string.Empty;
            resultDictionary["AD"] = string.Empty;
            resultDictionary["AE"] = string.Empty;
            resultDictionary["AF"] = string.Empty;
            resultDictionary["AG"] = string.Empty;
            resultDictionary["AH"] = string.Empty;
            resultDictionary["AI"] = string.Empty;
            resultDictionary["AJ"] = string.Empty;
            resultDictionary["AK"] = string.Empty;
            resultDictionary["AL"] = string.Empty;
            resultDictionary["AM"] = string.Empty;
            resultDictionary["AN"] = string.Empty;
            resultDictionary["AO"] = string.Empty;
            resultDictionary["AP"] = string.Empty;
            resultDictionary["AQ"] = string.Empty;
            resultDictionary["AR"] = string.Empty;
            resultDictionary["AS"] = string.Empty;
            resultDictionary["AT"] = string.Empty;
            resultDictionary["AU"] = parentSection.ProductLine.Price.ToString("F0");
        }

        protected override string GetGroupName()
        {
            return $"{Product.ProductTypeFull} {parentSection.ProductLine.Model} {Manufacturer}";
        }

        protected override string GetTitle1()
        {
            var title = $"{Product.ProductTypeFull} {Manufacturer.ToUpper()} {Product.Model}".Trim();
            title = Regex.Replace(title, " +", " ");

            if (title.Length >= TITLE1_MAX_LENGTH)
            {
               
            }

            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{Product.ProductTypeShort} {Manufacturer.ToUpper()} {Product.Model}".Trim();
            title = Regex.Replace(title, " +", " ");

            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                
            }


            return title;
        }

        protected override string GetTitle3()
        {
            //Бур SDS-MAX проломный ПРАКТИКА 80 х 1000 мм. Арт. 243-813, в наличии
            var title = $"{Product.Name} Арт. {Product.Sku}".Trim();

            title = Regex.Replace(title, " +", " ");
            title = title
                .Replace(" х ", "х")
                .Replace(" x ", "x")
                .Replace(" мм", "мм")
                .Replace( "зубов", "з.")
                .Replace( "зуба", "з.")
                .Replace( "зуб", "з.")
                .Replace( " серия", string.Empty)
                .Replace(" (1шт.)", string.Empty)
                .Replace(" (1шт)", string.Empty)
                .Replace(".\"", "\"");
            title = Regex.Replace(title, ",? (коробка|блистер|туба)", string.Empty);
            title = Regex.Replace(title, " +", " ");

            if (title.Length < (TITLE3_MAX_LENGTH - ", в наличии".Length))
            {
                title = $"{title}, в наличии";
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            //243-813 -практик
            //Практика 243-813


            if (lineNumber == 1)
            {
                keyPhrase = $"{Product.Sku}";
            }
            else if (lineNumber == 2)
            {
                keyPhrase = $"{Manufacturer} {Product.Sku}";
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToLower();
        }

        protected override string GetViewedUrl()
        {
            return $"{Manufacturer}";
        }
    }
}
