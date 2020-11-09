using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Sew")]
    public class SewYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "SEW";
        public string TranslatedManufacturer { get; } = "Сев";

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
            ["Z"] = "Измерительное оборудование||SEW Анализаторы эл. сетей||Индикаторы||Клещи||Микрометры||Мультиметры||Тестеры||Шумомеры SEW",
            ["AA"] = "Весь ассортимент от официального дилера||электрических||обрыва, напряжения, тестеры||электроизмерительные||||Цифровые||Многофунциональные||измеритель уровня шума",
            ["AB"] = "https://etk-komplekt.ru/sew||https://etk-komplekt.ru/izmeritelnye-pribory/analizatory-cepej/analizatory-elektricheskikh-setej-sew/||https://etk-komplekt.ru/izmeritelnye-pribory/indikatory/indikatory-sew/||https://etk-komplekt.ru/izmeritelnye-pribory/kleshchi-elektroizmeritelnye/kleshhi-elektroizmeritelnye-sew/||https://etk-komplekt.ru/izmeritelnye-pribory/izmeriteli-soprotivleniya/mikroommetry/mikroommetry-sew/||https://etk-komplekt.ru/izmeritelnye-pribory/multimetry-cifrovye/multimetry-sew/||https://etk-komplekt.ru/izmeritelnye-pribory/testery/mnogofunkcionalnye-testery/mnogofunkczionalnye-testery-sew/||https://etk-komplekt.ru/izmeritelnye-pribory/shumomery/shumomery-sew/",
            ["AK"] = "Гарантия качества||в наличии||Выставочный зал в СПб||Доставка по России||Доступные цены||Поверка"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            var source = productsInfo.ToList();

            foreach (var line in source)
            {
                int lines = !string.IsNullOrEmpty(line.CustomField) ? 4 : 3;

                var section = CreateSection(line, startGroupSectionNumber++, lines);

                sb.Append(section);               
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(SewYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class SewYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public SewYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection) { }

        protected override string GetViewedUrl() => $"{Manufacturer} {Product.Model}".ToViewedUrl();
        protected override string GetGroupName() => Product.Model.ToUpper();

        //SEW VOT-52 Цифровой электротестер
        protected override string GetTitle1() => $"{Manufacturer} {Product.Model} {Product.ProductTypeFull}";

        //SEW VOT-52 электротестер
        protected override string GetTitle2() => $"{Product.Model} {Product.ProductTypeShort}";

        //SEW VOT-52 Цифровой 2-х полюсный электротестер от официального дилера с доставкой!
        protected override string GetTitle3() => $"{Product.Model} {Product.ProductTypeFull} от официального дилера с доставкой!";
        
        protected override string GetPhrase(int lineNumber)
        {
            string result = null;

            var words = Product.ProductTypeShort.Split().Where(word => word.Length >= 4 && !Regex.IsMatch(word, @"^\d+$")).ToList();
            if(!string.IsNullOrWhiteSpace(Product.CustomField))
            {
                words.Add(Product.CustomField);
            }

            var wordsMinusArray = string.Join(" ", words.Select(w => $"-{w}"));

            //VOT-52 -sew -электротестер -тестер
            //sew VOT-52
            //электротестер VOT-52
            //тестер VOT-52


            if (lineNumber == 1)
            {
                result = $"{Product.Model} -sew {wordsMinusArray}".ToKeyPhrase();
            }
            else if (lineNumber == 2)
            {
                result = $"sew {Product.Model}".ToKeyPhrase();
            }
            else if (lineNumber == 3)
            {
                result = $"{Product.ProductTypeShort} {Product.Model}".ToKeyPhrase();
            }
            else if (lineNumber == 4)
            {
                result = $"{Product.CustomField} {Product.Model}".ToKeyPhrase();
            }
            else
            {
                throw new FormatException();
            }

            return result;
        }
    }
}
