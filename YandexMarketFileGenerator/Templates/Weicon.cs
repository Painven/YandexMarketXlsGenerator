using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Weicon")]
    public class WeiconYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Weicon";
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
            ["Z"] = "Спреи||Стрипперы||Контактный клей||Клей герметик||Эпоксидный клей||Анаэробный клей||Герметики||Очистители",
            ["AA"] = "Для ухода, клей-спрей||Кабельные, универсальные||Водостойкие||Силиконовый||Для метала||Фиксатор||Силиконовые, универсальные||Для электроники, универсальные",
            ["AB"] = "https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/sprei/sprei-weicon/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/ploskogubcy-kleshchi-kusachki/stripper/strippery-weicon/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/kley/kontaktnyy-kley/kontaktnyy-kley-weicon/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/kley/silikonovyy-kley/silikonovyy-kley-weicon/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/kley/epoksidnye-kley/epoksidnyy-kley-weicon/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/kley/anaerobnyy-kley/anaerobnyy-kley-weicon/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/germetiki/germetiki-weicon/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/ochistiteli/ochistiteli-weicon/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = (line.IsUniquePhrase && !string.IsNullOrEmpty(line.CustomField)) ? 8 : 
                    (line.IsUniquePhrase ? 6 : 3); 
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(WeiconYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class WeiconYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public WeiconYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
