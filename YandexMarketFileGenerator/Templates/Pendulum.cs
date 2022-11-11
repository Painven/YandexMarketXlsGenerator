using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Pendulum")]
    public class PendulumYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Pendulum";
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
            ["Z"] = "Pendulum частотомеры||Опции||Стандарты частоты||Усилители",
            ["AA"] = "Электронно-счетные||Для частотомеров||Рубидиевые, термостатированные||Высоковольтные",
            ["AB"] = "https://etk-komplekt.ru/izmeritelnye-pribory/chastotomery/chastotomery-pendulum/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/chastotomery/chastotomery-pendulum/opcii-dlya-izmeritelnyh-priborov-pendulum/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/chastotomery/standarty-chastoty/standarty-chastoty-pendulum/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/amplitudnye-usiliteli/usiliteli-vysokovoltnye-pendulum/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int linesCount = 5;
                sb.Append(CreateSection(line, startGroupSectionNumber++, linesCount));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(PendulumYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class PendulumYandexMarketSectionLine : YandexMarketSectionLineBase
    {

        public PendulumYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetViewedUrl()
        {
            return Model.ToViewedUrl();
        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Model}";
        }

        protected override string GetTitle1()
        {
            var title = $"{Product.ProductTypeShort} {Manufacturer} {Model} ";

            return title;
        }

        protected override string GetTitle2()
        {
            string title = $"{Model} {Manufacturer}";
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
            var keyPhrase = string.Empty;

            switch (lineNumber)
            {
                case 1: keyPhrase = $"{Manufacturer} {Model}"; break;
                case 2: keyPhrase = $"{ProductTypeShort} {Model}"; break;
                case 3: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Model}"; break;
                case 4: keyPhrase = $"{ProductTypeFull} {Model}"; break;
                case 5: keyPhrase = $"{ProductTypeFull} {Manufacturer} {Model}"; break;
            }

            
            if(string.IsNullOrWhiteSpace(keyPhrase))
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
