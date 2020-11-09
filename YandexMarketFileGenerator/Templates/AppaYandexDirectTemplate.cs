using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для APPA")]
    public class AppaYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "APPA";
        public string Host { get; } = "https://etk-komplekt.ru";
        public string TranslatedManufacturer { get; } = "АППА";

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["Z"] = "Мультиметры APPA||Клещи APPA||Измерители APPA||Индикаторы APPA",
            ["AB"] = "https://etk-komplekt.ru/izmeritelnye-pribory/multimetry/multimetry-cifrovye-appa/||https://etk-komplekt.ru/izmeritelnye-pribory/kleshhi-elektroizmeritelnye-i-preobrazovateli-toka/kleshchi-elektroizmeritelnye-appa/||https://etk-komplekt.ru/izmeritelnye-pribory/izmeriteli-rlc/izmeriteli-rlc-appa/||https://etk-komplekt.ru/izmeritelnye-pribory/indikatory-napryazheniya/indikatory-napryazheniya-appa/",
            ["T"] = "Россия",
            ["Y"] = "Работает везде",
            ["W"] = "+",
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["G"] = "-",
            ["C"] = "-",
            ["AJ"] = "-инструкция -руководство -эксплуатации"
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
            var data = new YandexMarketSection(this, typeof(AppaYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(3);
        }
    }

    internal class AppaYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public AppaYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
        }

        protected override void FillDictionary(int lineNumber)
        {
            resultDictionary["A"] = parentSection.ParentTemplate.ColumnStaticValues["A"];
            resultDictionary["B"] = parentSection.ParentTemplate.ColumnStaticValues["B"];
            resultDictionary["C"] = parentSection.ParentTemplate.ColumnStaticValues["C"];
            resultDictionary["D"] = string.Empty;
            resultDictionary["E"] = GetGroupName();
            resultDictionary["F"] = $"{parentSection.GroupIndex}";
            resultDictionary["G"] = parentSection.ParentTemplate.ColumnStaticValues["G"];
            resultDictionary["H"] = string.Empty;
            resultDictionary["I"] = GetPhrase(lineNumber);
            resultDictionary["J"] = string.Empty;
            resultDictionary["K"] = string.Empty;
            resultDictionary["L"] = GetTitle1();
            resultDictionary["M"] = GetTitle2();
            resultDictionary["N"] = GetTitle3();
            resultDictionary["O"] = string.Empty;
            resultDictionary["P"] = string.Empty;
            resultDictionary["Q"] = string.Empty;
            resultDictionary["R"] = FullUrlPath;
            resultDictionary["S"] = GetViewedUrl();
            resultDictionary["T"] = parentSection.ParentTemplate.ColumnStaticValues["T"];
            resultDictionary["U"] = string.Empty;
            resultDictionary["V"] = string.Empty;
            resultDictionary["W"] = parentSection.ParentTemplate.ColumnStaticValues["W"];
            resultDictionary["X"] = string.Empty;
            resultDictionary["Y"] = parentSection.ParentTemplate.ColumnStaticValues["Y"];
            resultDictionary["Z"] = parentSection.ParentTemplate.ColumnStaticValues["Z"];
            resultDictionary["AA"] = string.Empty;
            resultDictionary["AB"] = parentSection.ParentTemplate.ColumnStaticValues["AB"];
            resultDictionary["AC"] = string.Empty;
            resultDictionary["AD"] = string.Empty;
            resultDictionary["AE"] = string.Empty;
            resultDictionary["AF"] = string.Empty;
            resultDictionary["AG"] = string.Empty;
            resultDictionary["AH"] = string.Empty;
            resultDictionary["AI"] = string.Empty;
            resultDictionary["AJ"] = parentSection.ParentTemplate.ColumnStaticValues["AJ"];
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
        }

        protected override string GetGroupName()
        {
            return ReplaceManufacturerStart(base.GetGroupName());
        }

        private string ReplaceManufacturerStart(string s)
        {
            return s.ToUpper().Replace("APPA ", "APPA-");
        }

        protected override string GetTitle1()
        {
            var title = $"{ReplaceManufacturerStart(Product.Model)} {Product.ProductTypeShort}";

            return title;
        }

        protected override string GetTitle2()
        {
            return GetTitle1();
        }

        protected override string GetTitle3()
        {
            var title = $"{ReplaceManufacturerStart(Product.Model)} {Product.ProductTypeFull}. Официальный дилер APPA, доставка по России!";

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            string clearModel = Regex.Replace(Product.Model.Replace(Manufacturer, string.Empty), " +", " ").Trim();

            if (lineNumber == 1)
            {
                //appa 30r клещи токоизмерительные
                keyPhrase = $"{Manufacturer} {clearModel} {Product.ProductTypeFull}";
            }
            else if (lineNumber == 2)
            {
                //appa 30r клещи 
                keyPhrase = $"{Manufacturer} {clearModel} {Product.ProductTypeShort}";
            }
            else if (lineNumber == 3)
            {
                //appa 30r 
                keyPhrase = $"{Manufacturer} {clearModel}";
            }
            else if (lineNumber == 4)
            {
                //30r
                keyPhrase = $"{clearModel}";
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToLower();
        }
    }
}
