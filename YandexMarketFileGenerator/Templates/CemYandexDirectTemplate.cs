using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для CEM")]
    public class CEMYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "CEM";
        public string Host { get; } = "https://etk-komplekt.ru";
        public string TranslatedManufacturer { get; } = "СЕМ";

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
            ["Z"] = "CEM Мультиметры||Токовые клещи||Пирометры||Термометры||Тепловизоры||Измерители влажности||Измерители сопротивления||Анемометры CEM",
            ["AA"] = "Цифровые, автомобильные, с тепловизором||Ваттметры, компактные||Инфракрасные термометры||Бесконтактные, тепловизоры||Мультиметры с тепловизором||Для стройматериалов, древесины||Мегаоометры, миллиомметры||С пирометром, термоанемометры, компактные",
            ["AB"] = "https://etk-komplekt.ru/index.php?route=product/category&path=62703||https://etk-komplekt.ru/index.php?route=product/category&path=62714||https://etk-komplekt.ru/index.php?route=product/category&path=62711||https://etk-komplekt.ru/index.php?route=product/category&path=62705||https://etk-komplekt.ru/index.php?route=product/category&path=62712||https://etk-komplekt.ru/index.php?route=product/category&path=62707||https://etk-komplekt.ru/index.php?route=product/category&path=62723||https://etk-komplekt.ru/index.php?route=product/category&path=62721",
            ["AK"] = "Доступные цены||официальный дилер||доставка||Выставочный зал"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 7));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(CEMYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class CEMYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public CEMYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
        }

        protected override string GetGroupName()
        {
            return $"CEM-{Product.Model}";
        }

        protected override string GetTitle1()
        {
            var title = $"{Product.Model} {Product.ProductTypeShort}";

            return title;
        }

        protected override string GetTitle2()
        {
            return $"{Manufacturer} {Sku} {Product.ProductTypeShort}";
        }

        protected override string GetTitle3()
        {
            var title = $"{ProductTypeFull} CEM {Model} (арт. {Sku}) официальный дилер, доставка по России!";

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
                keyPhrase = $"{Manufacturer} {Model} {Product.ProductTypeShort}";
            }
            else if (lineNumber == 4)
            {
                keyPhrase = $"{Model} {Product.ProductTypeShort}";
            }
            else if (lineNumber == 5)
            {
                keyPhrase = $"{Manufacturer} {Sku}";
            }
            else if (lineNumber == 6)
            {
                keyPhrase = $"{ProductTypeShort} {Sku}";
            }
            else if (lineNumber == 7)
            {
                keyPhrase = $"{Manufacturer} {ProductTypeShort} {Sku}";
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToLower();
        }
    }
}
