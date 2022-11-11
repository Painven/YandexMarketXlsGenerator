using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Dremel")]
    public class DremelYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Dremel";
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
            ["Z"] = "МФУ инструмент DREMEL||Паяльники||Клеевые пистолеты||Насадки||Щетки||Круги отрезные||Буры||Стержни клеевые",
            ["AA"] = "Граверы и реноваторы||Газовые, термофены||Компактные и легкие||Абразивные, алмазные||Абразивные полировальные||По дереву, металлу, алмазные||Для раствора||Высоко/низко температурные",
            ["AB"] = "https://etk-komplekt.ru/instrumenty/elektroinstrument/mfu-instrument/mfu-instrument-dremel/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/payalnoe-oborudovanie/payalniki/gazovye-payalniki/gazovye-payalniki-dremel/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/elektroinstrument/kleevye-pistolety/kleevye-pistolety-dremel/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/elektroinstrument/aksessuary-prinadlezhnosti-i-osnastka-dlya-elektroinstrumenta/nasadki-dremel/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/elektroinstrument/aksessuary-prinadlezhnosti-i-osnastka-dlya-elektroinstrumenta/diski-i-chashki/abrazivnye-shchetki-dremel/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/elektroinstrument/aksessuary-prinadlezhnosti-i-osnastka-dlya-elektroinstrumenta/diski-i-chashki/otreznye-krugi-dremel/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/elektroinstrument/aksessuary-prinadlezhnosti-i-osnastka-dlya-elektroinstrumenta/bury-dremel/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/kleevye-sterzhni/kleevye-sterzhni-dremel/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int linesCount = string.IsNullOrEmpty(line.Model) ? 10 : (line.IsUniquePhrase ? 17 : 16);
                sb.Append(CreateSection(line, startGroupSectionNumber++, linesCount));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(DremelYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class DremelYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        string Sku_1 => Sku.Replace(" ", ".").Trim();
        string Sku_2 => Sku.Replace(" ", string.Empty).Trim();

        public DremelYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetViewedUrl()
        {
            return Sku.Replace(" ", "-");
        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Sku}";
        }

        protected override string GetTitle1()
        {
            var title = $"{Product.ProductTypeShort} {Manufacturer} {Sku_1}";

            return title;
        }

        protected override string GetTitle2()
        {
            string title = $"{Manufacturer} {Sku_1}";
            return title;
        }

        protected override string GetTitle3()
        {
            string title = $"{ProductTypeFull} {Manufacturer} (арт. {Sku_1}) официальный дилер, доставка по России!";
            
            if(title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title.Replace(", доставка по России!", string.Empty);
                if (title.Length >= TITLE3_MAX_LENGTH)
                {
                    title = title.Replace(" официальный дилер", string.Empty);
                }
            }


            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            switch (lineNumber)
            {
                case 1: keyPhrase = $"{Sku}"; break;
                case 2: keyPhrase = $"{Sku_1}"; break;
                case 3: keyPhrase = $"{Sku_2}"; break;
                case 4: keyPhrase = $"{Manufacturer} {Sku}"; break;
                case 5: keyPhrase = $"{Manufacturer} {Sku_1}"; break;
                case 6: keyPhrase = $"{Manufacturer} {Sku_2}"; break;
                case 7: keyPhrase = $"{ProductTypeShort} {Sku_1}"; break;
                case 8: keyPhrase = $"{ProductTypeShort} {Sku_2}"; break;
                case 9: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Sku_1}"; break;
                case 10: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Sku_2}"; break;

                case 11: keyPhrase = $"{Model} {Sku_1}"; break;
                case 12: keyPhrase = $"{Model} {Sku_2}"; break;
                case 13: keyPhrase = $"{ProductTypeShort} {Model} {Sku_1}"; break;
                case 14: keyPhrase = $"{ProductTypeShort} {Model} {Sku_2}"; break;
                case 15: keyPhrase = $"{Manufacturer} {Model} {Sku_1}"; break;
                case 16: keyPhrase = $"{Manufacturer} {Model} {Sku_2}"; break;

                case 17: keyPhrase = $"{Manufacturer} {Model}"; break;
            }

            
            if(string.IsNullOrWhiteSpace(keyPhrase))
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
