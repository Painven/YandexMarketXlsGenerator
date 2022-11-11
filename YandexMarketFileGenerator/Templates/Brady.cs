using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Brady")]
    public class BradyYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Brady";
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
            ["Z"] = "Принтеры||Этикетки||Риббон||Маркеры||Резаки||Ткани маркировочные||Контейнеры||Маркировка",
            ["AA"] = "Этикеток, промышленные||Для проводов, винил||Термоусадочная лента||Термоусадочные||Перфорационные||Винил, полиэстер||Для кабеля, проводов||Для кабеля, стоек",
            ["AB"] = "https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/oborudovanie-dlya-markirovki/printery-etiketok/printery-etiketok-brady/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/oborudovanie-dlya-markirovki/etiketki/etiketki-brady/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/oborudovanie-dlya-markirovki/termotransfernaya-lenta/ribbon-brady/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/oborudovanie-dlya-markirovki/markery/markery-brady/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/oborudovanie-dlya-markirovki/perforacionnye-rezaki/perforacionnye-rezaki-brady/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/oborudovanie-dlya-markirovki/markirovochnye-tkani/markirovochnye-tkani-brady/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/oborudovanie-dlya-markirovki/konteynery-dlya-markirovki/konteynery-dlya-markirovki-brady/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/oborudovanie-dlya-markirovki/markirovka/markirovka-brady/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = string.IsNullOrEmpty(line.CustomField) ? 6 : 8; 
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(BradyYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class BradyYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public BradyYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Model}";
        }

        protected override string GetViewedUrl()
        {
            return $"{Manufacturer} {Model}".ToViewedUrl();
        }

        protected override string GetTitle1()
        {
            string title = $"{Model} {Product.ProductTypeShort} {Manufacturer}";
            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{Model} {Manufacturer}";
            return title;
        }

        protected override string GetTitle3()
        {
            string title = $"{ProductTypeFull} {Manufacturer} {Model} (арт. {Sku}) с доставкой по России";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title.Replace(" с доставкой по России", string.Empty);
                
                if (title.Length >= TITLE3_MAX_LENGTH)
                {
                    title = title.Replace("арт. ", string.Empty);
                }
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

                case 4: keyPhrase = $"{Model}"; break;
                case 5: keyPhrase = $"{Manufacturer} {Model}"; break;
                case 6: keyPhrase = $"{ProductTypeShort} {Model}"; break;

                case 7: keyPhrase = $"{Product.CustomField} {Model}"; break;
                case 8: keyPhrase = $"{Product.CustomField} {Sku}"; break;

                default: throw new NotImplementedException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
