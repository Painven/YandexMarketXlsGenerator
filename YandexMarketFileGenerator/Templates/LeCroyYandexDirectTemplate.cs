using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для LeCroy")]
    public class LeCroyYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "LeCroy";
        public string Host { get; } = "https://etk-komplekt.ru";
        public string TranslatedManufacturer { get; } = "LeCroy";

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
            ["Y"] = "LeCroy Осциллографы||Анализаторы протоколов||Цепей||Генераторы LeCroy",
            ["AA"] = "https://etk-komplekt.ru/izmeritelnye-pribory/oscillografy/cifrovye-oscillografy/oscillografy-lecroy?utm_source=yandex||https://etk-komplekt.ru/izmeritelnye-pribory/analizatory-protokolov/analizatory-protokolov-lecroy?utm_source=yandex||https://etk-komplekt.ru/izmeritelnye-pribory/analizatory-cepej/analizatory-cepey-izmeriteli-ksvn-i-kkpo?utm_source=yandex||https://etk-komplekt.ru/izmeritelnye-pribory/generatory/generatory-signalov-specialnoy-formy-lecroy?utm_source=yandex",
            ["AJ"] = "ремонт оборудования||аренда Anritsu||Рефлектометры Anritsu"
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
            var data = new YandexMarketSection(this, typeof(LeCroyYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class LeCroyYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public LeCroyYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            resultDictionary["Q"] = FullUrlPath + "?utm_source=yandex";
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
            resultDictionary["AU"] = parentSection.ProductLine.Price.ToString("F0");
        }

        protected override string GetGroupName()
        {
            //lecroy wavesurfer

            return $"{Manufacturer} {Product.Model}".Trim().ToLower();
        }

        protected override string GetTitle1()
        {
            //Lecroy WaveSurfer Осциллографы 

            var title = $"Lecroy {Product.Model} {Product.ProductTypeShort}".Trim();
            title = Regex.Replace(title, " +", " ");

            if (title.Length >= TITLE1_MAX_LENGTH)
            {
               
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //Lecroy WaveSurfer 

            var title = $"Lecroy {Product.Model}".Trim();
            title = Regex.Replace(title, " +", " ");

            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                
            }


            return title;
        }

        protected override string GetTitle3()
        {
            //Lecroy WaveSurfer Осциллографы c доставкой РФ!
            var title = $"Lecroy {Product.Model} {Product.ProductTypeShort} с доставкой РФ!".Trim();
            title = Regex.Replace(title, " +", " ");

            if (title.Length > TITLE3_MAX_LENGTH)
            {
                
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            //lecroy wavesurfer
            //wavesurfer -lecroy


            if (lineNumber == 1)
            {
                keyPhrase = $"{Manufacturer} {Product.Model}";
            }
            else if (lineNumber == 2)
            {
                keyPhrase = $"{Product.Model} teledyne lecroy";
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToLower();
        }

        protected override string GetViewedUrl()
        {
            string url = $"Lecroy-{Product.Model.Replace(" ", "-")}"; 
            if(url.Length >= VIEWED_URL_MAX_LENGTH)
            {

            }

            return url;
        }
    }
}
