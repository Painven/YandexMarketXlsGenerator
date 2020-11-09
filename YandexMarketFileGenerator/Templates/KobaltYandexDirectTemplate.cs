using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для КОБАЛЬТ")]
    public class KobaltYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Кобальт";
        public string Host { get; } = "https://etk-komplekt.ru";
        public string TranslatedManufacturer { get; } = "Кобальт";

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
            ["Y"] = "Инструмент Кобальт||Наборы инструментов||Молотки||Головки Кобальт",
            ["AA"] = "https://etk-komplekt.ru/cobalt||https://etk-komplekt.ru/nabory-instrumentov/nabory-instrumentov-kobalt/||https://etk-komplekt.ru/ruchnoy-instrument/molotki-kiyanki-kuvaldy/molotky/molotki-kobalt/||https://etk-komplekt.ru/ruchnoy-instrument/golovki/golovki-kobalt/"
        };

        public List<string> InvalidWords => throw new NotImplementedException();


        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 3));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(KobaltYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class KobaltYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public KobaltYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            //Набор инструментов Кобальт 010106-94 
            return $"{Product.ProductTypeFull} {Manufacturer} {Product.Model}";
        }

        protected override string GetTitle1()
        {
            //Набор Кобальт 010106 94
            var modelString = Product.Model.Replace("-", " ");
            var title = $"{Product.ProductTypeShort} {Manufacturer} {modelString}".Trim();
            title = Regex.Replace(title, " +", " ");

            if (title.Length >= TITLE1_MAX_LENGTH)
            {
               
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //Кобальт 010106 94

            var modelString = Product.Model.Replace("-", " ");
            var title = $"{Manufacturer} {modelString}".Trim();
            title = Regex.Replace(title, " +", " ");

            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                
            }


            return title;
        }

        protected override string GetTitle3()
        {
            //Набор инструментов Кобальт 010106-94 в наличии!

            var title = $"{Product.ProductTypeFull} {Manufacturer} {Product.Model} в наличии!".Trim();
            title = Regex.Replace(title, " +", " ");

            if (title.Length > TITLE3_MAX_LENGTH)
            {
                
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            //набор инструментов кобальт 010106 94
            //кобальт 010106 94
            //010106 94 -кобальт

            var replacedModelString = Product.Model.Replace("-", " ");

            if (lineNumber == 1)
            {
                keyPhrase = $"{Product.ProductTypeShort} {Manufacturer} {replacedModelString}";
            }
            else if (lineNumber == 2)
            {
                keyPhrase = $"{Manufacturer} {replacedModelString}";
            }
            else if (lineNumber == 3)
            {
                keyPhrase = $"{replacedModelString} -{Manufacturer}";
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
