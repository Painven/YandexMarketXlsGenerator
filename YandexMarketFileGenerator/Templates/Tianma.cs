using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Tianma")]
    public class TianmaYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Tianma";
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
            ["Z"] = "Стандартные панели||Квадратные и круглые||TFT-LCD модули||Кабели питания||Платы и контроллеры Tianma",
            ["AA"] = "",
            ["AB"] = "https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/displei/tft-lcd-paneli/standartnye-tft-lcd-paneli/standartnye-tft-lcd-paneli-tianma/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/displei/tft-lcd-paneli/vytyanutye-kvadratnye-kruglye-tft-lcd-paneli/vytyanutye-kvadratnye-kruglye-tft-lcd-paneli-tianma/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/displei/tft-moduli/tft-lcd-moduli-tianma/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/displei/prinadlezhnosti-dlya-tft-paneley-i-moduley/lvds-i-kabeli-pitaniya-dlya-tft-lcd-tianma/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/displei/prinadlezhnosti-dlya-tft-paneley-i-moduley/kontrollery-platy-drayvery-dlya-tft-lcd-tianma/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int linesCount = line.ProductTypeShort == "Дисплей" ? 24 : 8;
                sb.Append(CreateSection(line, startGroupSectionNumber++, linesCount));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(TianmaYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class TianmaYandexMarketSectionLine : YandexMarketSectionLineBase
    {

        public TianmaYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Sku}";
        }

        protected override string GetTitle1()
        {
            var title = $"{Product.ProductTypeFull} {Manufacturer} {Model} ";

            return title;
        }

        protected override string GetTitle2()
        {
            string title = string.Empty;
            if (Product.ProductTypeShort == "Дисплей")
            {
                title = $"Матрица {Manufacturer} {Sku}";               
            }
            else
            {
                title = $"{Product.ProductTypeFull} {Manufacturer} {Product.Sku}";
            }
            return title;
        }

        protected override string GetTitle3()
        {
            string title = string.Empty;
            
            if(ProductTypeShort == "Дисплей")
            {
                title = $"Матрица {Manufacturer} {Model} (арт. {Sku}) официальный дилер, доставка по России!";
            }
            else
            {
                title = $"{ProductTypeFull} {Manufacturer} {Model} (арт. {Sku}) официальный дилер, доставка по России!";
            }
            
            if(title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title.Replace(", доставка по России", string.Empty);
            }


            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            switch (lineNumber)
            {
                case 1: keyPhrase = Model; break;
                case 2: keyPhrase = Sku; break;
                case 3: keyPhrase = $"{Manufacturer} {Model}"; break;
                case 4: keyPhrase = $"{Manufacturer} {Sku}"; break;
                case 5: keyPhrase = $"{ProductTypeShort} {Model}"; break;
                case 6: keyPhrase = $"{ProductTypeShort} {Sku}"; break;
                case 7: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Model}"; break;
                case 8: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Sku}"; break;

                case 9: keyPhrase = $"Матрица {Model}"; break;
                case 10: keyPhrase = $"Матрица {Sku}"; break;
                case 11: keyPhrase = $"Матрица {Manufacturer} {Model}"; break;
                case 12: keyPhrase = $"Матрица {Manufacturer} {Sku}"; break;
                case 13: keyPhrase = $"Экран {Model}"; break;
                case 14: keyPhrase = $"Экран {Sku}"; break;
                case 15: keyPhrase = $"Экран {Manufacturer} {Model}"; break;
                case 16: keyPhrase = $"Экран {Manufacturer} {Sku}"; break;
                case 17: keyPhrase = $"ЖК экран {Model}"; break;
                case 18: keyPhrase = $"ЖК экран {Sku}"; break;
                case 19: keyPhrase = $"ЖК экран {Manufacturer} {Model}"; break;
                case 20: keyPhrase = $"ЖК экран {Manufacturer} {Sku}"; break;
                case 21: keyPhrase = $"ЖК дисплей {Model}"; break;
                case 22: keyPhrase = $"ЖК дисплей {Sku}"; break;
                case 23: keyPhrase = $"ЖК дисплей {Manufacturer} {Model}"; break;
                case 24: keyPhrase = $"ЖК дисплей {Manufacturer} {Sku}"; break;
            }

            
            if(string.IsNullOrWhiteSpace(keyPhrase))
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
