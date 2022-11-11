using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Erem")]
    public class EremYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Erem";
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
            ["Z"] = "Пинцеты||Кусачки||Бокорезы||Плоскогубцы||Наборы пинцетов||Наборы инструмента||Режущие пинцеты||Прецизионные пинцеты",
            ["AA"] = "Прицезионные, режущие||Остроконечные, загнутые||Антистатические, овальные||Соединительные||Универсальные, прицезионные||Для работы с компонентами||Для мягких проводов||Для точных работ",
            ["AB"] = "https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/pincety/pincety-erem/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/ploskogubcy-kleshchi-kusachki/kusachki/kusachki-erem/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/ploskogubcy-kleshchi-kusachki/bokorezy-i-kusachki/bokorezy-erem/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/ploskogubcy-kleshchi-kusachki/ploskogubcy/ploskogubcy-erem/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/pincety/pincety-erem/nabory-pincetov-erem/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabory-instrumenta-erem/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/pincety/pincety-erem/rezhushchie-pincety-erem/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/instrumenty/precizionnyy-instrument/precizionnye-pincety/precizionnye-pincety-erem/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = 2;
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(EremYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class EremYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public EremYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Model}";
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
            string title = $"{ProductTypeFull} {Manufacturer} {Model} от официального дилера с доставкой по России";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title.Replace(" с доставкой по России", string.Empty);
                if (title.Length >= TITLE3_MAX_LENGTH)
                {
                    title = title.Replace(" от официального дилера", string.Empty);
                }
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            var keyPhrase = "";

            switch (lineNumber)
            {
                case 1: keyPhrase = $"{Manufacturer} {Model}"; break;
                case 2: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Model}"; break;

                default: throw new NotImplementedException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
