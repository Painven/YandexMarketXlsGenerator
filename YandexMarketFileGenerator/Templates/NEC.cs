using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для NEC")]
    public class NECYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "NEC";
        public string Host { get; } = "https://etk-komplekt.ru";

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
            ["Z"] = "TFT-LCD панели",
            ["AA"] = "",
            ["AB"] = "https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/displei/tft-lcd-displei/standartnye-tft-lcd-paneli/tft-lcd-paneli-nec/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int linesCount = 12;
                sb.Append(CreateSection(line, startGroupSectionNumber++, linesCount));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(NECYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class NECYandexMarketSectionLine : YandexMarketSectionLineBase
    {

        public NECYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Model}";
        }

        protected override string GetTitle1()
        {
            var title = $"{Product.ProductTypeFull} {Manufacturer} {Model} ";

            return title;
        }

        protected override string GetTitle2()
        {
            string title = $"{ProductTypeShort} {Manufacturer} {Model}";
            return title;
        }

        protected override string GetTitle3()
        {
            string title = $"{ProductTypeFull} {Manufacturer} {Model} официальный дилер, доставка по России!";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title.Replace(", доставка по России", string.Empty);
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            switch (lineNumber)
            {
                case 1: keyPhrase = Model; break;
                case 2: keyPhrase = $"{Manufacturer} {Model}"; break;
                case 3: keyPhrase = $"{ProductTypeShort} {Model}"; break;
                case 4: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Model}"; break;

                case 5: keyPhrase = $"Матрица {Model}"; break;
                case 6: keyPhrase = $"Матрица {Manufacturer} {Model}"; break;
                case 7: keyPhrase = $"Экран {Model}"; break;
                case 8: keyPhrase = $"Экран {Manufacturer} {Model}"; break;
                case 9: keyPhrase = $"ЖК экран {Model}"; break;
                case 10: keyPhrase = $"ЖК экран {Manufacturer} {Model}"; break;
                case 11: keyPhrase = $"Дисплей {Model}"; break;
                case 12: keyPhrase = $"Дисплей {Manufacturer} {Model}"; break;
            }

            
            if(string.IsNullOrWhiteSpace(keyPhrase))
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
