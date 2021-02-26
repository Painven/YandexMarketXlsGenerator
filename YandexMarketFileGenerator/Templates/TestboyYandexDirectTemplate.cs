using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Testboy")]
    public class TestboyYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Testboy";
        public string TranslatedManufacturer { get; } = "Тестбой";

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
            ["Z"] = "Индикаторы Testboy||Тестеры||Толщиномеры||Пирометры||Мультиметры||Дальномеры||Клещи токовые||Фонари Testboy",
            ["AA"] = "Напряжения, бесконтактные||чередования фаз||с зондом||инфракрасные||цифровые||электроизмерительные||налобные, светодиодные",
            ["AB"] = "https://etk-komplekt.ru/shop/izmeritelnye-pribory/indikatory/indikatory-napryazheniya/indikatory-napryazheniya-fluke||https://etk-komplekt.ru/shop/izmeritelnye-pribory/testery/testery-cheredovaniya-faz/testery-cheredovaniya-faz-testboy||https://etk-komplekt.ru/shop/izmeritelnye-pribory/tolshchinomery/tolshhinomery-testboy||https://etk-komplekt.ru/testboytv325-testboy-infrakrasnyj-termometr-tv-325||https://etk-komplekt.ru/shop/izmeritelnye-pribory/pirometry/pirometry-testboy||https://etk-komplekt.ru/shop/izmeritelnye-pribory/lazernye-dalnomery/lazernye-dalnomery-testboy||https://etk-komplekt.ru/shop/izmeritelnye-pribory/kleshchi-elektroizmeritelnye/kleshhi-elektroizmeritelnye-testboy||https://etk-komplekt.ru/shop/instrumenty/ruchnoy-instrument/fonari/svetodiodnye-fonari-testboy",
            ["AK"] = "официальный дилер||Доставка по России||Выставочный зал в СПб||Гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            var source = productsInfo.ToList();

            foreach (var line in source)
            {
                int keyPhraseCount = 3;
                var section = CreateSection(line, startGroupSectionNumber++, keyPhraseCount);

                sb.Append(section);               
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int keyPhraseCount)
        {
            var data = new YandexMarketSection(this, typeof(TestboyYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(keyPhraseCount);
        }
    }

    internal class TestboyYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public TestboyYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection) { }

        protected override string GetViewedUrl() => $"{Manufacturer} {Model}".ToViewedUrl();

        protected override string GetGroupName() => $"{Manufacturer} {Model}";

        protected override string GetTitle1() => $"{Manufacturer} {Model} {ProductTypeShort}";

        protected override string GetTitle2() => $"{ProductTypeShort} {Manufacturer} {Model}";

        protected override string GetTitle3() => $"{Manufacturer} {Model} {ProductTypeFull} от дилера с доставкой!";

        protected override string GetPhrase(int lineNumber)
        {
            string result = null;

            //Testboy 11
            //измеритель Testboy 11
            //Testboy 11 -измеритель


            if (lineNumber == 1)
            {
                result = $"{Manufacturer} {Model}".ToKeyPhrase();
            }
            else if (lineNumber == 2)
            {
                result = $"{ProductTypeShort} {Manufacturer} {Model}".ToKeyPhrase();
            }
            else if (lineNumber == 3)
            {
                result = $"{Manufacturer} {Model} -{ProductTypeShort}".ToKeyPhrase();
            }
            else
            {
                throw new FormatException();
            }

            if(result.WordsCount() >= KEY_PHRASE_MAX_WORDS)
            {

            }

            return result;
        }
    }
}
