using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для UNI-T")]
    public class UNITYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "UNI-T";
        public string Host { get; } = "https://etk-komplekt.ru";
        public string TranslatedManufacturer => "Юнит";

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
            ["Z"] = "Мультиметры||Пирометры||Анемоментры||Шумомеры||Люксметры||Осциллографы||Токовые клещи||Мегаомметры",
            ["AA"] = "Цифровые, с автовыборов диапазона||Инфракрасные термометры||Измерители скорости воздушного потока||Бытовые, промышленные||Измерители освещенности, USB||Цифровые, мультиметры-осциллографы||Цифровые||Измерители сопротивления изоляции",
            ["AB"] = "https://etk-komplekt.ru/shop/izmeritelnye-pribory/multimetry-cifrovye/multimetry-uni-t||https://etk-komplekt.ru/shop/izmeritelnye-pribory/pirometry/pirometry-uni-t||https://etk-komplekt.ru/shop/izmeritelnye-pribory/anemometry/anemometry-uni-t||https://etk-komplekt.ru/shop/izmeritelnye-pribory/shumomery/shumomery-uni-t||https://etk-komplekt.ru/shop/izmeritelnye-pribory/testery/lyuksmetry/lyuksmetry-uni-t||https://etk-komplekt.ru/shop/izmeritelnye-pribory/oscillografy/cifrovye-oscillografy/oscillografy-uni-t||https://etk-komplekt.ru/shop/izmeritelnye-pribory/kleshchi-elektroizmeritelnye/kleshchi-elektroizmeritelnye-uni-t||https://etk-komplekt.ru/shop/izmeritelnye-pribory/izmeriteli-soprotivleniya/megaommetry/megaommetry-uni-t",
            ["AK"] = "Доступные цены||официальный дилер||доставка||Выставочный зал"
        };
       
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 6));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(UNITYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class UNITYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public UNITYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer}-{Product.Model}";
        }

        protected override string GetTitle1()
        {
            return $"{Model} {Product.ProductTypeShort} {Manufacturer}";
        }

        protected override string GetTitle2()
        {
            return $"{Product.Model} {Product.ProductTypeShort}";
        }

        protected override string GetTitle3()
        {
            var title = $"{ProductTypeFull} {Manufacturer} {Model} официальный дилер, доставка по России!";

            if(title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title.Replace(", доставка по России!", string.Empty);
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            if (lineNumber == 1)
            {
                keyPhrase = Model;
            }
            else if (lineNumber == 2)
            {
                keyPhrase = $"{Manufacturer} {Model}";
            }
            else if (lineNumber == 3)
            {
                keyPhrase = $"{Product.ProductTypeShort} {Manufacturer} {Model}";
            }
            else if (lineNumber == 4)
            {
                keyPhrase = $"{Product.ProductTypeShort} {Model}";
            }
            else if (lineNumber == 5)
            {
                keyPhrase = $"{TranslatedManufacturer} {Model}";
            }
            else if (lineNumber == 6)
            {
                keyPhrase = $"{ProductTypeShort} {TranslatedManufacturer} {Model}";
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToLower();
        }
    }
}
