using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Bernstein")]
    public class BernsteinYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Bernstein";
        public string Host { get; } = "https://bernstein-vde.ru";
        public string TranslatedManufacturer { get; } = "Берштайн";

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["Z"] = "Наборы инструмента||Отвертки||Бокорезы и кусачки||Пинцеты",
            ["AB"] = "https://bernstein-vde.ru/nabory-ruchnogo-instrumenta/||https://bernstein-vde.ru/otvertki/||https://bernstein-vde.ru/bokorezy-i-kusachki/||https://bernstein-vde.ru/pincety/",
            ["T"] = "Россия",
            ["Y"] = "Работает везде",
            ["W"] = "+",
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["G"] = "-",
            ["C"] = "-",
            ["AJ"] = "-ag -alliance -briana -byron -catheter -charles -david -elmer -felicia -haleen -heinrichswalde -jaclyn -jacob -joseph -kirara -leonard -mahler -maria -nicole -nina -peter -richard -rise -rugal -stacey -story -zita -бернштейн -выключатель -датчик -инстагр -концева -международный -педаль -перевод -роза -связь -скачать -центр"
        };

        public List<string> InvalidWords => throw new NotImplementedException();

        public event EventHandler<string> WarningMessageRecieved;

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            ShowWarningMessage();
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 3));
            }

            return sb.ToString();
        }

        private void ShowWarningMessage()
        {
            string message = $"Для цен будет использован курс валют RUR => EUR\r\nс соотношением 1:{Math.Round(Helpers.CurrencyRatesHelper.RurInOneEuro, 2)}";
            WarningMessageRecieved?.Invoke(this, message);
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(BernsteinYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(3);
        }
    }

    internal class BernsteinYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public BernsteinYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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

        protected override string GetTitle1()
        {
            var title = $"{Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}";
            if (title.Length >= 30)
            {
                title = $"{ModelWithoutManufacturerName} {Product.ProductTypeFull}";
            }

            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{Manufacturer} {ModelWithoutManufacturerName}";

            if(title.Length >= 30)
            {
                title = ModelWithoutManufacturerName;
            }
            return title;
        }

        protected override string GetTitle3()
        {
            string title = null;

            decimal realPriceInRoubles = Math.Ceiling((Product.Price * Helpers.CurrencyRatesHelper.RurInOneEuro) / 10) * 10;

            if (realPriceInRoubles != decimal.Zero)
            {
                title = $"{Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}. Цена {realPriceInRoubles} руб, в наличии, отправка по России!";
                if (title.Length >= TITLE3_MAX_LENGTH)
                {
                    title = $"{Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}. Цена {realPriceInRoubles} руб, отправка по России!";

                    if (title.Length >= TITLE3_MAX_LENGTH)
                    {
                        title = $"{Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}. Цена {realPriceInRoubles} руб,";
                    }
                }
            }
            else
            {
                title = $"{Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}. В наличии, отправка по России!";
                if (title.Length >= TITLE3_MAX_LENGTH)
                {
                    title = $"{Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}. Отправка по России!";

                    if (title.Length >= TITLE3_MAX_LENGTH)
                    {
                        title = $"{Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeFull}. В наличии";
                    }
                }
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string result = null;

            if (lineNumber == 1)
            {
                result = $"{TranslatedManufacturer} {ModelWithoutManufacturerName}"
                    .ToLower()
                    .RemoveInvalidCharsInYandexKeyPhrase();
            }
            else if (lineNumber == 2)
            {
                result = $"{ModelWithoutManufacturerName} {Manufacturer} {TranslatedManufacturer} {Product.ProductTypeFull}"
                    .ToLower()
                    .RemoveInvalidCharsInYandexKeyPhrase();
            }
            else if (lineNumber == 3)
            {
                result = $"{Manufacturer} {ModelWithoutManufacturerName}"
                    .ToLower()
                    .RemoveInvalidCharsInYandexKeyPhrase();
            }
            else if (lineNumber == 4)
            {
                result = $"{Product.ProductTypeFull} {ModelWithoutManufacturerName}"
                    .ToLower()
                    .RemoveInvalidCharsInYandexKeyPhrase();
            }
            else
            {
                throw new FormatException();
            }

            if(result.Split(new string[] { "-", " "}, StringSplitOptions.None).Length >= 7)
            {
                result = Regex.Replace(result, TranslatedManufacturer + " ", string.Empty, RegexOptions.IgnoreCase).Trim();
            }

            return result;
        }
    }
}
