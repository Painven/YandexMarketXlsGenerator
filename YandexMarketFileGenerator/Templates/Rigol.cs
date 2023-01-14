using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Rigol")]
    public class RigolYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Rigol";
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
            ["Z"] = "Осциллографы||Генераторы||Анализаторы спектра||Источники питания||Электронные нагрузки||Мультиметры||Логгеры||Программное обеспечение",
            ["AA"] = "Цифровые, смешанных сигналов||Универсальные, высокочастотные||Реального времени, с трекинг-генератором||Программируемые, прецизионные||Программируемые, цифровые||Прецизионные, лабораторные||Базовые блоки, модули мультиплексора||Для осциллографов, источников питания",
            ["AB"] = "https://etk-komplekt.ru/izmeritelnye-pribory/oscillografy/cifrovye-oscillografy-rigol/||https://etk-komplekt.ru/izmeritelnye-pribory/generatory/generatory-signalov-rigol/||https://etk-komplekt.ru/izmeritelnye-pribory/analizatoryi-spektra/analizatory-spektra-rigol/||https://etk-komplekt.ru/istochniki-pitaniya/istochniki-pitaniya-rigol/||https://etk-komplekt.ru/izmeritelnye-pribory/elektronnye-nagruzki/elektronnye-nagruzki-rigol/||https://etk-komplekt.ru/izmeritelnye-pribory/multimetry-cifrovye/multimetry-rigol/||https://etk-komplekt.ru/izmeritelnye-pribory/loggery-dannykh/loggery-dannyh-rigol/||https://etk-komplekt.ru/izmeritelnye-pribory/aksessuary-i-prinadlezhnosti/programmnoe-obespechenie-rigol/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int lines = line.IsUniquePhrase ? 4 : 3;
                sb.Append(CreateSection(line, startGroupSectionNumber++, lines));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(RigolYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class RigolYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public RigolYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Model}";
        }

        protected override string GetViewedUrl()
        {
            return Model.ToViewedUrl();
        }

        protected override string GetTitle1()
        {
            var title = $"{Model} {Product.ProductTypeShort} {Manufacturer}";

            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{Product.Model} {Manufacturer}";

            return title;
        }

        protected override string GetTitle3()
        {
            var title = $"{ProductTypeFull} {Manufacturer} {Model} от официального дилера, доставка по России!";

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
            var keyPhrase = "";

            switch (lineNumber)
            {
                case 1: keyPhrase = Product.Model; break;
                case 2: keyPhrase = $"{Manufacturer} {Model}"; break;
                case 4: keyPhrase = $"{ProductTypeFull} {Model}"; break;
                case 3: keyPhrase = $"{ProductTypeShort} {Model}"; break;               
                default: throw new NotImplementedException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
