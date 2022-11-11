using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для AMO")]
    public class AMOYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "AMO";
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
            ["Z"] = "",
            ["AA"] = "",
            ["AB"] = "?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = 6;
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(AMOYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class AMOYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public AMOYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {ModelOrSku}";
        }

        protected override string GetViewedUrl()
        {
            return $"{Manufacturer} {ModelOrSku}".ToViewedUrl();
        }

        protected override string GetTitle1()
        {
            string title = $"{ModelOrSku} {Product.ProductTypeShort} {Manufacturer}";
            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{ModelOrSku} {Manufacturer}";
            return title;
        }

        protected override string GetTitle3()
        {
            string title = string.Empty;
            if (!string.IsNullOrWhiteSpace(Product.Model))
            {
                title = $"{ProductTypeFull} {Manufacturer} {Model} (арт. {Sku})";
            }
            else
            {
                title = $"{ProductTypeFull} {Manufacturer} {Sku}";
            }

            title += " с доставкой по России";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title.Replace(" с доставкой по России", string.Empty);              
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            var keyPhrase = "";

            switch (lineNumber)
            {
                case 1: keyPhrase = $"{Sku}"; break;
                case 2: keyPhrase = $"{Manufacturer} {Sku}"; break;
                case 3: keyPhrase = $"{ProductTypeShort} {Sku}"; break;

                case 4: keyPhrase = $"{Manufacturer} {Model}"; break;         
                case 5: keyPhrase = $"{ProductTypeShort} {Model}"; break;
                case 6: keyPhrase = $"{Model} {Sku}"; break;

                default: throw new NotImplementedException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
