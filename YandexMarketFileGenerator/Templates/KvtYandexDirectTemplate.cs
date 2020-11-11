using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для КВТ")]
    public class КВТYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "КВТ";
        public string TranslatedManufacturer { get; } = "kvt";

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
            ["Z"] = "КВТ инструмент||Диэлектрический инструмент||Клещи КВТ||Кримперы||Мультиметры||Наборы инструмента КВТ||Набор отверток||Ножницы КВТ",
            ["AA"] = "Весь ассортимент||VDE||Токовые, пресс клещи, электрические||Для обжима||Цифровые||Автомобильный, диэлектрический, домашний||Диэлектрические, индикаторные, с битами||Секаторные, гидравлические, для кабеля",
            ["AB"] = "https://etk-komplekt.ru/kvt||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/antistaticheskiy-i-dielektricheskiy-instrument/dielektricheskij-instrument-vde/dielektricheskiy-instrument-kvt/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/ploskogubcy-kleshchi-kusachki/kleshchi/kleshchi-kvt/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/ploskogubcy-kleshchi-kusachki/krimper/krimpery-kvt/||https://etk-komplekt.ru/izmeritelnye-pribory/multimetry-cifrovye/multimetry-kvt/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabory-instrumentov-kvt/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabor-otvertok/nabory-otvertok-kvt/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nozhnicy/nozhnicy-kvt/",
            ["AK"] = "официальный дилер||Доставка по России||Выставочный зал в СПб||Гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            var source = productsInfo.ToList();

            foreach (var line in source)
            {
                int keyPhraseCount = 7;
                var section = CreateSection(line, startGroupSectionNumber++, keyPhraseCount);

                sb.Append(section);               
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int keyPhraseCount)
        {
            var data = new YandexMarketSection(this, typeof(КВТYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(keyPhraseCount);
        }
    }

    internal class КВТYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public КВТYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection) { }

        private string SkuDigits => Product.Model.Replace("KV-", string.Empty);

        protected override string GetViewedUrl() => $"{Manufacturer} {Model}".ToViewedUrl();
        protected override string GetGroupName() => $"{Manufacturer} {Model}";

        //КВТ БРГ-12 Болторез 67657
        protected override string GetTitle1() => $"{Manufacturer} {Model} {ProductTypeShort} {SkuDigits}";

        //Болторез КВТ БРГ-12
        protected override string GetTitle2() => $"{ProductTypeShort} {Manufacturer} {Model}";

        //КВТ БРГ-12 Гидравлический болторез 67657. Артикул: KV-67657. От дилера с доставкой!
        protected override string GetTitle3()
        {
            var title = $"{Manufacturer} {Model} {ProductTypeFull} {SkuDigits}. Артикул: {Sku}. От дилера с доставкой!";
            if(title.GetTitleLength() >= TITLE3_MAX_LENGTH)
            {
                title = $"{Manufacturer} {Model} {ProductTypeFull}. Артикул: {Sku}. От дилера с доставкой!";

                if (title.GetTitleLength() >= TITLE3_MAX_LENGTH)
                {
                    title = $"{Manufacturer} {Model} {ProductTypeFull}. арт. {Sku}. От дилера с доставкой!";

                    if (title.GetTitleLength() >= TITLE3_MAX_LENGTH)
                    {
                        title = $"{Manufacturer} {Model} {ProductTypeFull}. арт. {Sku} от дилера";

                        if (title.GetTitleLength() >= TITLE3_MAX_LENGTH)
                        {
                            title = $"{Manufacturer} {Model} {ProductTypeFull}. арт. {Sku}";
                        }
                    }
                }
            }

            return title;
        }
        
        protected override string GetPhrase(int lineNumber)
        {
            string result = null;

            //квт 67657
            //Гидравлический болторез КВТ БРГ-12 67657
            //БРГ-12 -квт -болторез
            //болторез 67657
            //КВТ БРГ-12
            //болторез БРГ-12
            //KV-67657


            if (lineNumber == 1)
            {
                result = $"{Manufacturer} {SkuDigits}".ToKeyPhrase();
            }
            else if (lineNumber == 2)
            {
                result = $"{ProductTypeFull} {Manufacturer} {ModelWithoutManufacturerName} {SkuDigits}".ToKeyPhrase();
            }
            else if (lineNumber == 3)
            {
                result = $"{ModelWithoutManufacturerName} -квт -{ProductTypeShort}".ToKeyPhrase();
            }
            else if (lineNumber == 4)
            {
                result = $"{ProductTypeShort} {SkuDigits}".ToKeyPhrase();
            }
            else if (lineNumber == 5)
            {
                result = $"{Manufacturer} {ModelWithoutManufacturerName}".ToKeyPhrase();
            }
            else if (lineNumber == 6)
            {
                result = $"{ProductTypeShort} {Model}".ToKeyPhrase();
            }
            else if (lineNumber == 7)
            {
                result = $"{Sku}".ToKeyPhrase();
            }
            else
            {
                throw new FormatException();
            }

            return result;
        }
    }
}
