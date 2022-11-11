using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Hantek")]
    public class HantekYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Hantek";
        
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
            ["Z"] = "Осциллографы||USB||Смешанных сигналов||Портативные||Токовые клещи||Логгеры||Щупы||Аксессуары",
            ["AA"] = "Цифровые, USB, портативные, мини||Для подключения к компьютеру через USB||Портативные, настольные, с генератором||Компактные, USB осциллографы||С разъемами BNC и BANANA||Регистраторы электрических данных||Контактные и беспонтактные, переходники||Делители, щупы, ответвители",
            ["AB"] = "https://etk-komplekt.ru/izmeritelnye-pribory/oscillografy/cifrovye-oscillografy/oscillografy-hantek/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/oscillografy/usb-oscillografy/usb-oscillografy-hantek/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/oscillografy/cifrovye-oscillografy/oscillografy-hantek/oscillografy-smeshannyh-signalov-hantek/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/oscillografy/cifrovye-oscillografy/oscillografy-hantek/portativnye-oscillografy-hantek/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/kleshchi-elektroizmeritelnye/tokovye-kleshchi-hantek/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/loggery-dannykh/registratory-dannyh-hantek/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/oscillografy/deliteli-i-aksessuary-k-oscillografam/deliteli-i-aksessuary-hantek/shchupy-hantek/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/oscillografy/deliteli-i-aksessuary-k-oscillografam/deliteli-i-aksessuary-hantek/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||доступные цены"
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
            var data = new YandexMarketSection(this, typeof(HantekYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class HantekYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public string ManufacturerRussian { get; } = "Хантек";

        public HantekYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            var title = $"{Manufacturer} {Model}";
            return title;
        }

        protected override string GetTitle3()
        {
            string title = $"{ProductTypeFull} {Manufacturer} {Model} от официального дилера с доставкой по России";

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
                case 3: keyPhrase = $"{ProductTypeShort} {Model}"; break;
                case 4: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Model}"; break;
                case 5: keyPhrase = $"{ManufacturerRussian} {Model}"; break;
                case 6: keyPhrase = $"{ProductTypeShort} {ManufacturerRussian} {Model}"; break;

                default: throw new NotImplementedException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
