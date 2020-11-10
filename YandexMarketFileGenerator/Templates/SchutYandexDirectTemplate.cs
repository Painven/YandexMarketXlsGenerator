using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Schut")]
    public class SchutYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Schut";
        public string TranslatedManufacturer { get; } = "Шут";

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
            ["Z"] = "Schut измерительные приборы||Schut Штангенциркули||Микрометры||Идикаторы||Стойки||Толщиномеры||Угломеры||Уровни Schut",
            ["AA"] = "Весь ассортимент измерительных приборов||Цифровые||Цифровые, гладкие||Электронные, часовые||Магнитные индикаторные стойки||Стрелочные||Комбинированные, универсальные||Брусковые, круглые",
            ["AB"] = "https://etk-komplekt.ru/schut||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/shtangencirkuli/shtangencirkuli-schut/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/mikrometry/mikrometry-schut/||https://etk-komplekt.ru/izmeritelnye-pribory/indikatory/indikatory-schut/||https://etk-komplekt.ru/izmeritelnye-pribory/aksessuary-i-prinadlezhnosti/magnitnye-indikatornye-stoyki-schut/||https://etk-komplekt.ru/izmeritelnye-pribory/tolshchinomery/tolshchinomery-schut/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/ugolniki/uglomery-schut/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/urovni/urovni-schut/",
            ["AK"] = "Немецкое качество||Гарантия производителя||Доставка по России"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            var source = productsInfo.ToList();

            foreach (var line in source)
            {
                int lines = 4;

                var section = CreateSection(line, startGroupSectionNumber++, lines);

                sb.Append(section);               
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(SchutYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class SchutYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public SchutYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection) { }

        protected override string GetViewedUrl() => $"{Manufacturer} {Product.Model}".ToViewedUrl();

        protected override string GetGroupName() => $"{Manufacturer} {Product.Model}";

        //Schut 908.757 Микрометр цифровой
        protected override string GetTitle1() => $"{Manufacturer} {Product.Model} {Product.ProductTypeFull}";

        //Schut 908.757 Filetta
        protected override string GetTitle2() => $"{Manufacturer} {Product.Model} Filetta";

        //Schut 908.757 – Микрометр цифровой Filetta от официального дилера
        protected override string GetTitle3() => $"{Manufacturer} {Product.Model} – {Product.ProductTypeFull} Filetta от официального дилера";
        
        protected override string GetPhrase(int lineNumber)
        {
            string result = null;

            //Микрометр 908.757
            //Schut 908.757
            //Filetta 908.757
            //Микрометр цифровой 908.757 Filetta



            if (lineNumber == 1)
            {
                result = $"{Product.ProductTypeShort} {Product.Model}".ToKeyPhrase();
            }
            else if (lineNumber == 2)
            {
                result = $"{Manufacturer} {Product.Model}".ToKeyPhrase();
            }
            else if (lineNumber == 3)
            {
                result = $"Filetta {Product.Model}".ToKeyPhrase();
            }
            else if (lineNumber == 4)
            {
                result = $"{Product.ProductTypeFull} {Product.Model} Filetta".ToKeyPhrase();
            }
            else
            {
                throw new FormatException();
            }

            return result;
        }
    }
}
