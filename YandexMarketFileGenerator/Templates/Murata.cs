using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Murata PS")]
    public class MurataYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Murata Power Solutions";
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
            ["Z"] = "Источники питания||AC/DC||DC/DC||Программируемые",
            ["AA"] = "AC/DC, DC/DC, программируемые||преобразователи AC/DC||преобразователи DC/DC||Источники питания",
            ["AB"] = "https://etk-komplekt.ru/istochniki-pitaniya/istochniki-pitaniya-murata-power-solutions/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/istochniki-pitaniya/istochniki-pitaniya-murata-power-solutions/acdc-preobrazovateli-murata-power-solutions/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/istochniki-pitaniya/istochniki-pitaniya-murata-power-solutions/dcdc-preobrazovateli-murata-power-solutions?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/istochniki-pitaniya/istochniki-pitaniya-murata-power-solutions/programmiruemye-istochniki-pitaniya-murata-power-solutions/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = 11;
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(MurataYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class MurataYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public MurataYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetGroupName()
        {
            return $"Murata PS {Model}";
        }

        protected override string GetViewedUrl()
        {
            return $"{Model}".ToViewedUrl();
        }

        protected override string GetTitle1()
        {
            string title = $"{Model} {Product.ProductTypeShort} Murata PS";
            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{Model} Murata PS";
            return title;
        }

        protected override string GetTitle3()
        {
            string title = $"{ProductTypeFull} {Manufacturer} {Model} с доставкой по России";

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
                case 1: keyPhrase = $"{Model}"; break;
                case 2: keyPhrase = $"{Manufacturer} {Model}"; break;
                case 3: keyPhrase = $"Murata PS {Model}"; break;
                case 4: keyPhrase = $"Murata {Model}"; break;           
                case 5: keyPhrase = $"{ProductTypeShort} {Model}"; break;
                case 6: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Model}"; break;
                case 7: keyPhrase = $"{ProductTypeShort} Murata PS {Model}"; break;
                case 8: keyPhrase = $"{ProductTypeShort} Murata {Model}"; break;
                case 9: keyPhrase = $"{Manufacturer} {Sku}"; break;
                case 10: keyPhrase = $"Murata PS {Sku}"; break;
                case 11: keyPhrase = $"Murata {Sku}"; break;

                default: throw new NotImplementedException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
