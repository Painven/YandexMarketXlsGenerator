using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Berger")]
    public class BergerYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Berger";
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
            ["Z"] = "Наборы инструмента||Отвертки||Ключи||Головки||Наборы отверток||Наборы ключей||Наборы головок||Наборы бит",
            ["AA"] = "Отверток, ключей, универсальные||Наборы, диэлектрические, часовые||Наборы, комбинированные, разводные||Наборы, торцевые, шестригранные||Диэлектричские, ударные, шестигранные||Комбинированные, динамометрические, трещоточные||Торцевые, удлиненные, шестигранные||Магнитные, с отверткой",
            ["AB"] = "https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabory-instrumenta-berger/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/otvertka/otvertki-berger/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/instrumentalnye-klyuchi/klyuchi-berger/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/golovki/golovki-berger/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabor-otvertok/nabory-otvertok-berger/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/instrumentalnye-klyuchi/klyuchi-berger/nabory-klyuchey-berger/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabor-bit/nabory-golovok-berger/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabor-bit/nabory-bit-berger/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = !string.IsNullOrEmpty(line.Model) ? 8 : 3; 
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(BergerYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class BergerYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public BergerYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Sku}";
        }

        protected override string GetViewedUrl()
        {
            return $"{Manufacturer} {Sku}".ToViewedUrl();
        }

        protected override string GetTitle1()
        {
            string title = $"{Sku} {Product.ProductTypeShort} {Manufacturer}";
            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{Manufacturer} {Sku}";
            return title;
        }

        protected override string GetTitle3()
        {
            string title = "";
            if (!string.IsNullOrWhiteSpace(Product.Model))
            {
                title = $"{ProductTypeFull} \"{Model}\" {Manufacturer} (арт. {Sku}) с доставкой по России";
            }
            else
            {
                title = $"{ProductTypeFull} {Manufacturer} {Sku} с доставкой по России";
            }

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
                case 6: keyPhrase = $"{Sku} {Model}"; break;
                case 7: keyPhrase = $"{Manufacturer} {Model} {ProductTypeShort}"; break;
                case 8: keyPhrase = $"{Manufacturer} {Model} {Sku} "; break;

                default: throw new NotImplementedException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
