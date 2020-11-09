using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для HELFER")]
    public class HelferYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Helfer";
        public string Host { get; } = "https://etk-komplekt.ru";
        public string TranslatedManufacturer { get; } = "Хелфер";

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",       
            ["V"] = "+",
            ["X"] = "Работает везде",
            ["Y"] = "Helfer||Наборы инструмента||Инструментальные ключи||Головки",
            ["AA"] = "https://etk-komplekt.ru/helfer||https://etk-komplekt.ru/nabory-instrumentov/nabory-instrumentov-helfer/||https://etk-komplekt.ru/ruchnoy-instrument/instrumentalnye-klyuchi/instrumentalnye-klyuchi-helfer/||https://etk-komplekt.ru/ruchnoy-instrument/instrumentalnye-klyuchi/golovki/golovki-helfer/"
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
            var data = new YandexMarketSection(this, typeof(HelferYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(3);
        }
    }

    internal class HelferYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public HelferYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            resultDictionary["W"] = string.Empty;
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
            return $"helfer {parentSection.ProductLine.Model}";
        }

        protected override string GetTitle1()
        {
            //Kraft KT 700300. Набор инструментов
            var title = $"Helfer {parentSection.ProductLine.Model}. {parentSection.ProductLine.ProductTypeFull}".Trim();

            return title;
        }

        protected override string GetTitle2()
        {
            //Kraft KT 700300. 108 предметов
            var title = $"Helfer {parentSection.ProductLine.Model}. {parentSection.ProductLine.ProductTypeShort}".Trim();

            return title;
        }

        protected override string GetTitle3()
        {
            //Kraft KT 700300 набор инструментов из 108 предметов
            var replacedName = Product.Name
                .Replace("HELFER", string.Empty)
                .Replace("Helfer", string.Empty);

            var newName = Regex.Replace(replacedName, " +", " ").Trim();

            var title = $"Helfer {parentSection.ProductLine.Model} {newName}".Trim();

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            //kraft кт 700300
            //кт 700300 -kraft
            //набор инструментов kraft кт 700300


            if (lineNumber == 1)
            {
                keyPhrase = $"helfer {parentSection.ProductLine.Model}";
            }
            else if (lineNumber == 2)
            {
                keyPhrase = $"{parentSection.ProductLine.Model} -helfer";
            }
            else if (lineNumber == 3)
            {
                keyPhrase = $"{parentSection.ProductLine.ProductTypeShort} helfer {parentSection.ProductLine.Model}";
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToLower();
        }

        protected override string GetViewedUrl()
        {
            return $"Helfer-{parentSection.ProductLine.Model}";
        }
    }
}
