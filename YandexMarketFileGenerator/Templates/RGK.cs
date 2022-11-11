using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для RGK")]
    public class RGKYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "RGK";
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
            ["Z"] = "Лазерный уровень||Нивелир||Мультиметры||Термометры||Пирометр||Тепловизоры||Теодолит||Зонды",
            ["AA"] = "Лазерные, оптические||Ротационные, лазерные, оптические||Портативные, цифровые||С поверкой, с зондами||Портативные, с гарантией||Легкие, повышенной точности||Электронные, оптические||Погружные, поверхностные",
            ["AB"] = "https://etk-komplekt.ru/izmeritelnye-pribory/lazernye-urovni/lazernye-urovni-rgk/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/niveliry/niveliry-rgk/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/multimetry-cifrovye/multimetry-rgk/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/termometry/termometry-rgk/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/pirometry/pirometry-rgk/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/teplovizory/teplovizory-rgk/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/teodolity/teodolity-rgk/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/zondy/zondy-rgk/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = line.IsUniquePhrase ? 8 : 7;
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(RGKYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class RGKYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public RGKYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {

        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Sku}";
        }

        protected override string GetViewedUrl()
        {
            if (!string.IsNullOrWhiteSpace(Model))
            {
                return Model.ToViewedUrl();
            }
            else
            {
                return Sku.ToViewedUrl();
            }
            
        }

        protected override string GetTitle1()
        {
            string title = "";
            if (!string.IsNullOrWhiteSpace(Model))
            {
                title = $"{Model} {Product.ProductTypeShort} {Manufacturer}";
            }
            else
            {
                title = $"{Sku} {Product.ProductTypeShort} {Manufacturer}";              
            }
            return title;
        }

        protected override string GetTitle2()
        {
            string title = "";
            if (!string.IsNullOrWhiteSpace(Model))
            {
                title = $"{Model} {Manufacturer}";
            }
            else
            {
                title = $"{Sku} {Manufacturer}";
            }
            return title;
        }

        protected override string GetTitle3()
        {
            string title = "";
            if (!string.IsNullOrWhiteSpace(Model))
            {
                title = $"{ProductTypeFull} {Manufacturer} {Model} (арт. {Sku}) от официального дилера, доставка по России!";
            }
            else
            {
                title = $"{ProductTypeFull} {Manufacturer} {Sku} от официального дилера, доставка по России!";
            }

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
                case 1: keyPhrase = $"{Sku}"; break;
                case 2: keyPhrase = $"{Manufacturer} {Sku}"; break;
                case 3: keyPhrase = $"{Manufacturer} {Model}"; break;
                case 4: keyPhrase = $"{ProductTypeShort} {Sku}"; break;
                case 5: keyPhrase = $"{ProductTypeShort} {Model}"; break;
                case 6: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Sku}"; break;
                case 7: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Model}"; break;
                case 8: keyPhrase = $"{Model}"; break;
                default: throw new NotImplementedException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
