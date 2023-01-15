using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Guide")]
    public class GuiderYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Guide";
        public string Host { get; } = "https://etk-komplekt.ru";
        public string TranslatedManufacturer => "Гайд";

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
            ["Z"] = "Серия PS||Серия D||Серия MobIR||Серия T100",
            ["AA"] = "Для плохих погодных условий||Для мониторинга изменения температуры||Для подключения к смартфону||Портативные теппловизоры начального уровня",
            ["AB"] = "https://etk-komplekt.ru/ps-guide-vysokoeffektivnye-teplovye-kamery||https://etk-komplekt.ru/d-guide-intellektualnye-teplovizionnye-kamery||https://etk-komplekt.ru/moblr-testo-teplovizory-dlya-smartfona-guide-seriya||https://etk-komplekt.ru/t100-testo-portativnye-teplovizionnye-kamery-guide-seriya",
            ["AK"] = "доступные цены||Поверка||Аксесуары||в наличии"
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
            var data = new YandexMarketSection(this, typeof(GuideYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class GuideYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public GuideYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Model}";
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
            var title = $"{ProductTypeFull} {Manufacturer} {Model} от официального дилера с доставкой по России";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title.Replace(" от официального дилера с доставкой по России", " от официального дилера");
                if (title.Length >= TITLE3_MAX_LENGTH)
                {
                    throw new ArgumentOutOfRangeException(nameof(title));
                }
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
                keyPhrase = $"{Product.ProductTypeShort} {Model}";
            }
            else if (lineNumber == 4)
            {
                keyPhrase = $"{Product.ProductTypeShort} Гайд {Model}";
            }
            else if (lineNumber == 5)
            {
                keyPhrase = $"Гайд {Model}";
            }
            else if (lineNumber == 6)
            {
                keyPhrase = "'---autotargeting";
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToLower();
        }
    }
}
