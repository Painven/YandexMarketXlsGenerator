using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Electrolube")]
    public class ElectrolubeYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Electrolube";
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
            ["Z"] = "Смазывающие составы||Антистатические||Для электроники||Гели||Краска||Смолы||Оплетки||Замораживатели",
            ["AA"] = "Контактные, универсальные||Покрытия, распылители||Очистители для отмывки||Теплопроводящие, экранирующие||Токопроводящая||Огнестойкие, эпоксидные||Для удаления припоя||С мин. накоплением заряда",
            ["AB"] = "https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/smazki/smazyvayushchie-sostavy-electrolube/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/antistaticheskie-sredstva/antistaticheskie-sredstva-electrolube/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/ochistiteli/ochistiteli-electrolube/ochistiteli-dlya-elektroniki-electrolube/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/teploprovodyashchie-geli/teploprovodyashchie-geli-electrolube/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/kraska/kraska-electrolube/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/smoly/smoly-electrolube/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/payalnoe-oborudovanie/raskhodnye-materialy/opletki-dlya-udaleniya-pripoya-electrolube/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/stroitelstvo/stroitelnaya-ximiya/hladagenty/zamorazhivateli-electrolube/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int linesCount = line.IsUniquePhrase ? 7 : 4;
                sb.Append(CreateSection(line, startGroupSectionNumber++, linesCount));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(ElectrolubeYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class ElectrolubeYandexMarketSectionLine : YandexMarketSectionLineBase
    {

        public ElectrolubeYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetViewedUrl()
        {
            return Sku.ToViewedUrl();
        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Sku}";
        }

        protected override string GetTitle1()
        {
            var title = $"{Product.ProductTypeShort} {Manufacturer} {Sku} ";

            return title;
        }

        protected override string GetTitle2()
        {
            string title = $"{Sku} {Manufacturer}";
            return title;
        }

        protected override string GetTitle3()
        {
            string title = $"{ProductTypeFull} {Manufacturer} {Sku} ";

            if (!string.IsNullOrWhiteSpace(Model))
            {
                title += $"\"{Model}\" ";
            }

            title += "с доставкой по России";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title.Replace(" с доставкой по России", string.Empty);
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            switch (lineNumber)
            {
                case 1: keyPhrase = Sku; break;
                case 2: keyPhrase = $"{Manufacturer} {Sku}"; break;
                case 3: keyPhrase = $"{ProductTypeShort} {Sku}"; break;
                case 4: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Sku}"; break;

                case 5: keyPhrase = $"{Sku} {Model}"; break;
                case 6: keyPhrase = $"{ProductTypeShort} {Sku} {Model}"; break;
                case 7: keyPhrase = $"{Manufacturer} {Sku} {Model}"; break;
            }

            
            if(string.IsNullOrWhiteSpace(keyPhrase))
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
