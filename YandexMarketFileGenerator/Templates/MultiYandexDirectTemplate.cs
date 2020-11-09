using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Multi")]
    public class MultiYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Multi";
        public string TranslatedManufacturer => "Мульти";
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
            ["Z"] = "Multi||Датчики||Измерители сопротивления||Индикаторы напряжения||Преобразователи тока||Тестеры||Клещи Multi",
            ["AA"] = "Полный каталог||||||||||||",
            ["AB"] = "https://etk-komplekt.ru/multi||https://etk-komplekt.ru/izmeritelnye-pribory/datchiki/datchiki-multi/||https://etk-komplekt.ru/izmeritelnye-pribory/izmeriteli-soprotivleniya/izmeriteli-soprotivleniya-multi/||https://etk-komplekt.ru/izmeritelnye-pribory/indikatory/indikatory-napryazheniya/indikatory-napryazheniya-multi/||https://etk-komplekt.ru/izmeritelnye-pribory/izmeritelnye-preobrazovateli-peremennogo-toka/preobrazovateli-toka-multi/||https://etk-komplekt.ru/izmeritelnye-pribory/testery/testery-multi/||https://etk-komplekt.ru/izmeritelnye-pribory/kleshchi-elektroizmeritelnye/kleshhi-elektroizmeritelnye-multi/",
            ["AK"] = "официальный дилер||доставка||Гарантия производителя"
        };
     
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
            var data = new YandexMarketSection(this, typeof(MultiYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class MultiYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        private string GetModelWithManufacturer => (Product.Model.Contains("MULTI") ? $"{Product.Model}" : $"MULTI {Product.Model}").Trim();
        private string GetModelWithoutManufacturer => Product.Model.Replace("MULTI ", string.Empty).Trim();

        public MultiYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection) { }

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
            return GetModelWithManufacturer;
        }

        protected override string GetViewedUrl()
        {
            string url = GetModelWithManufacturer.ToViewedUrl();

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
            var title = GetModelWithManufacturer.Replace("MULTI ", "Multi ");

            return title;
        }

        protected override string GetTitle2()
        {
            string title =  $"{Product.ProductTypeShort} {Product.Model}";

            return title;
        }

        protected override string GetTitle3()
        {
            string title = $"{GetModelWithManufacturer.Replace("MULTI ", "Multi ")} {Product.ProductTypeFull} с доставкой по России";

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string phrase = string.Empty;
            //Multi FCM-100
            //FCM-100 -multi
            //---autotargeting


            if (lineNumber == 1)
            {
                phrase = GetModelWithManufacturer.Replace("MULTI ", "Multi");
            }
            else if(lineNumber == 2)
            {
                phrase = $"{GetModelWithoutManufacturer} -multi";
            }
            else if(lineNumber == 3)
            {
                phrase = "'---autotargeting";
            }
            else
            {
                throw new NotImplementedException();
            }

            return phrase.ToLower().Trim();
        }
    }
}
