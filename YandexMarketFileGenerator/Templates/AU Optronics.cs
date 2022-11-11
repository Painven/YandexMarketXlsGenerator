using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для AUO")]
    public class AUOYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "AU Optronics";
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
            ["Z"] = "Стандартные дисплеи||Вытянутые, квадратные, круглые дисплеи||Дисплеи для видеостен||Платы для дисплеев||Проекционно-ёмкостные сенсоры||Модули для дисплеев",
            ["AA"] = "",
            ["AB"] = "https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/displei/tft-lcd-paneli/standartnye-tft-lcd-paneli/standartnye-tft-lcd-paneli-au-optronics/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/displei/tft-lcd-paneli/vytyanutye-kvadratnye-kruglye-tft-lcd-paneli/vytyanutye-kvadratnye-kruglye-tft-lcd-paneli-au-optronics/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/displei/tft-lcd-paneli/tft-lcd-paneli-dlya-videosten/tft-lcd-paneli-dlya-videosten-au-optronics/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/displei/prinadlezhnosti-dlya-tft-paneley-i-moduley/proekcionno-yomkostnye-sensory-au-optronics/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/displei/tft-moduli/tft-lcd-moduli-au-optronics/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/promyshlennoe-oborudovanie-i-kip/displei/prinadlezhnosti-dlya-tft-paneley-i-moduley/kontrollery-platy-drayvery-dlya-tft-lcd-au-optronics/?utm_source=yandex&roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int linesCount = 12;

                if(line.ProductTypeShort == "Дисплей")
                {
                    linesCount += 12;
                }    

                sb.Append(CreateSection(line, startGroupSectionNumber++, linesCount));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(AUOYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class AUOYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        string ManufacturerShort { get; } = "AUO";

        public AUOYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Sku}";
        }

        protected override string GetTitle1()
        {
            var title = $"{Model} {Product.ProductTypeShort} {Manufacturer}";
            if(title.Length >= TITLE1_MAX_LENGTH)
            {
                title = $"{Model} {Product.ProductTypeShort} {ManufacturerShort}";
            }

            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{Product.Model} {Product.ProductTypeFull}";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                title = $"{Product.ProductTypeFull} {ManufacturerShort} {Product.Sku}";
            }

            return title;
        }

        protected override string GetTitle3()
        {
            var title = $"{ProductTypeFull} {Manufacturer} {Model} (арт. {Sku}) официальный дилер, доставка по России!";

            if(title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title.Replace(Manufacturer, ManufacturerShort);
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
                case 5: keyPhrase = $"{ManufacturerShort} {Model}"; break;
                case 6: keyPhrase = $"{ManufacturerShort} {Sku}"; break;
                case 7: keyPhrase = $"{ProductTypeShort} {Model}"; break;
                case 8: keyPhrase = $"{ProductTypeShort} {Sku}"; break;
                case 9: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Model}"; break;
                case 10: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Sku}"; break;
                case 11: keyPhrase = $"{ProductTypeShort} {ManufacturerShort} {Model}"; break;
                case 12: keyPhrase = $"{ProductTypeShort} {ManufacturerShort} {Sku}"; break;

                case 13: keyPhrase = $"Матрица {Model}"; break;
                case 14: keyPhrase = $"Матрица {Sku}"; break;
                case 15: keyPhrase = $"Матрица {Manufacturer} {Model}"; break;
                case 16: keyPhrase = $"Матрица {Manufacturer} {Sku}"; break;
                case 17: keyPhrase = $"Матрица {ManufacturerShort} {Model}"; break;
                case 18: keyPhrase = $"Матрица {ManufacturerShort} {Sku}"; break;
                case 19: keyPhrase = $"Экран {Model}"; break;
                case 20: keyPhrase = $"Экран {Sku}"; break;
                case 21: keyPhrase = $"Экран {Manufacturer} {Model}"; break;
                case 22: keyPhrase = $"Экран {Manufacturer} {Sku}"; break;
                case 23: keyPhrase = $"Экран {ManufacturerShort} {Model}"; break;
                case 24: keyPhrase = $"Экран {ManufacturerShort} {Sku}"; break;
            }

            
            if(string.IsNullOrWhiteSpace(keyPhrase))
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
