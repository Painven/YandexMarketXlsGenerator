using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Center")]
    public class CenterYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Center";
        public string TranslatedManufacturer { get; } = "Центр";

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["T"] = "23605641314",
            ["W"] = "+",
            ["X"] = "Активно",
            ["Y"] = "Работает везде",
            ["Z"] = "CENTER Приборы||Измерители температуры||Люксометры||Шумомеры||Клещи||Измерители влажности||Газоанализаторы||Датчики CENTER",
            ["AA"] = "Весь ассортимент оборудования CENTER||Измеряют температуру, влажность||Измерение освещенности||Измерители шума||Измеряет ACA, DCV, ACV, Сопротивление, Частота||Измеряют влажность||Индикатор утечки горючих газов||Сменные датчики для разных приборов",
            ["AB"] = "https://etk-komplekt.ru/center||https://etk-komplekt.ru/izmeritelnye-pribory/termometry/izmeriteli-temperatury-center/||https://etk-komplekt.ru/izmeritelnye-pribory/testery/lyuksmetry/lyuksmetry-center/||https://etk-komplekt.ru/izmeritelnye-pribory/shumomery/shumomery-center/||https://etk-komplekt.ru/izmeritelnye-pribory/kleshchi-elektroizmeritelnye/kleshhi-elektroizmeritelnye-center/||https://etk-komplekt.ru/izmeritelnye-pribory/izmeriteli-vlazhnosti/izmeriteli-vlazhnosti-center/||https://etk-komplekt.ru/izmeritelnye-pribory/gazoanalizatory/gazoanalizatory-center/||https://etk-komplekt.ru/izmeritelnye-pribory/datchiki/datchiki-center/",
            ["AK"] = "официальный дилер||Выставочный зал в СПб||Гарантия производителя||Доставка по России"
        };

        public List<string> InvalidWords => throw new NotImplementedException();

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            var source = productsInfo.ToList();

            foreach (var line in source)
            {
                var section = CreateSection(line, startGroupSectionNumber++);

                sb.Append(section);               
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount = 3)
        {
            var data = new YandexMarketSection(this, typeof(CenterYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class CenterYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public CenterYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection) { }

        protected override void FillDictionary(int lineNumber)
        {
            var cells = GetCellsRange();

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
            resultDictionary["F"] = $"{parentSection.GroupIndex}";
            resultDictionary["I"] = GetPhrase(lineNumber);
            resultDictionary["K"] = GetTitle1();
            resultDictionary["L"] = GetTitle2();
            resultDictionary["M"] = GetTitle3();
            resultDictionary["Q"] = FullUrlPath;
            resultDictionary["R"] = GetViewedUrl();
            resultDictionary["AV"] = GetProductPrice();

        }

        private string GetProductPrice()
        {
            string priceString = Product.Price != decimal.Zero ? Product.Price.ToString("F0") : string.Empty;

            return priceString;
        }

        protected override string GetViewedUrl()
        {
            return Product.Model.ToViewedUrl();
        }

        protected override string GetGroupName()
        {
            string groupName = $"{Manufacturer} {Product.Model}".ToLower().Trim();

            return groupName;
        }

        protected override string GetTitle1()
        {
            var title = $"{Manufacturer} {Product.Model} {Product.ProductTypeShort}";

            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{Manufacturer} {Product.Model}";

            return title;
        }

        protected override string GetTitle3()
        {
            var title = $"{Manufacturer} {Product.Model} {Product.ProductTypeFull} c доставкой РФ!";

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string result = null;

            var replacedModel = Regex.Replace(Product.Model, " {2,}", " ").Trim();

            if (lineNumber == 1)
            {
                result = $"{Manufacturer} {replacedModel}".ToLower().RemoveInvalidCharsInYandexKeyPhrase();
            }
            else if (lineNumber == 2)
            {
                result = $"{replacedModel} -{Manufacturer}".ToLower().RemoveInvalidCharsInYandexKeyPhrase();
            }
            else
            {
                throw new FormatException();
            }

            result = Regex.Replace(result, " +", " ")
                .Replace("-", " ")
                .Trim();

            if (result.Split().Length > 7)
            {
                //throw new FormatException("Длина ключа должна содержать не более 7 слов");
                File.AppendAllLines("errors_Center.txt", new string[] { $"[{DateTime.Now.ToShortTimeString()}] {parentSection.ProductLine.Sku}" });
            }

            return result;
        }
    }
}
