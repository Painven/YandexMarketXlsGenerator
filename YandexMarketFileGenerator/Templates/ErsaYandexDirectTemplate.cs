using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Ersa")]
    public class ErsaYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Ersa";
        public string Host { get; } = "https://etk-komplekt.ru";
        public string TranslatedManufacturer { get; } = "Эрса";

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>() { };

        public List<string> InvalidWords => throw new NotImplementedException();


        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 1));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(ErsaYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class ErsaYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public ErsaYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
        }

        protected override void FillDictionary(int lineNumber)
        {
            resultDictionary["A"] = "-";
            resultDictionary["B"] = "Текстово-графическое";
            resultDictionary["C"] = "-";
            resultDictionary["D"] = string.Empty;
            resultDictionary["E"] = GetGroupName();
            resultDictionary["F"] = $"{parentSection.GroupIndex}";
            resultDictionary["G"] = "-";
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
            resultDictionary["S"] = "Россия";
            resultDictionary["T"] = string.Empty;
            resultDictionary["U"] = string.Empty;
            resultDictionary["V"] = string.Empty;
            resultDictionary["W"] = "Активно";
            resultDictionary["X"] = "Работает везде";
            resultDictionary["Y"] = "Паяльные станции||Паяльники||Паяльные ванны||Расходные материалы";
            resultDictionary["Z"] = string.Empty;
            resultDictionary["AA"] = "https://ersa-kurtz.ru/payalnye-stancii/?utm_source=yandex||https://ersa-kurtz.ru/payalniki/?utm_source=yandex||https://ersa-kurtz.ru/payalnye-vanny/?utm_source=yandex||https://ersa-kurtz.ru/raskhodnye-materialy/?utm_source=yandex";
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
            resultDictionary["AU"] = string.Empty;
        }

        protected override string GetGroupName()
        {
            return ReplaceManufacturerStart(base.GetGroupName());
        }

        private string ReplaceManufacturerStart(string s)
        {
            return s.ToUpper().Replace("Ersa ", "Ersa-");
        }

        protected override string GetTitle1()
        {
            var title = $"{ReplaceManufacturerStart(Product.Model)} Ersa {Product.ProductTypeShort}";

            //if(title.Length >= TITLE1_MAX_LENGTH)
            //{
            //    throw new FormatException($"Превышена длина заголовка: \n'{title}'\nТекущая: {title.Length}\nМаксимально допустимая: {TITLE1_MAX_LENGTH}");
            //}

            return title;
        }

        protected override string GetTitle2()
        {
            return string.Empty;
        }

        protected override string GetTitle3()
        {
            var title = $"{ReplaceManufacturerStart(Product.Model)} Ersa {Product.ProductTypeFull}. Официальный дилер Эрса, доставка по России!";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title.Replace(", доставка по России!", string.Empty);
                //throw new FormatException($"Превышена длина заголовка: \n'{title}'\nТекущая: {title.Length}\nМаксимально допустимая: {TITLE3_MAX_LENGTH}");
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            if (lineNumber == 1)
            {
                if(Product.Model.Length <= 5 || Regex.IsMatch(Product.Model, @"^\d+$"))
                {
                    keyPhrase = Product.Model + " Ersa";
                }
                else
                {
                    return Product.Model;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToLower();
        }
    }
}
