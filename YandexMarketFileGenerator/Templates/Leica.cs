using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Leica")]
    public class LeicaYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Leica";
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
            ["Z"] = "Лазерный дальномер||Лазерные нивелиры||Оптические нивелиры||Ротационные нивелиры||Нивелиры||Аксессуары для нивелиров||Штативы||Приемники лазерного луча",
            ["AA"] = "Беспроводные, компактные||Самовыравнивающиеся, комплекты||Для точных работ, с поверкой||Для ремонтных работ||Лазерные, оптические, ротационные||Насадки, пульты, адаптеры||Фибергласовые, элевационные||С функцией индикации высоты",
            ["AB"] = "https://etk-komplekt.ru/izmeritelnye-pribory/lazernye-dalnomery/lazernye-dalnomery-leica/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/niveliry/lazernye-niveliry/lazernye-niveliry-leica/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/niveliry/opticheskie-niveliry/opticheskie-niveliry-leica/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/niveliry/niveliry-leica/aksessuary-dlya-nivelirov-leica/priemniki-lazernogo-lucha-leica/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/niveliry/niveliry-leica/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/niveliry/niveliry-leica/aksessuary-dlya-nivelirov-leica/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/niveliry/niveliry-leica/aksessuary-dlya-nivelirov-leica/shtativy-leica/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}||https://etk-komplekt.ru/izmeritelnye-pribory/niveliry/rotacionnye-niveliry/rotacionnye-niveliry-leica/?roistat=direct1_{source_type}_{banner_id}_{keyword}&roistat_referrer={source}&roistat_pos={position_type}_{position}",
            ["AK"] = "Доступные цены||официальный дилер||доставка||гарантия производителя"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = string.IsNullOrEmpty(line.Model) ? 4 : (!line.IsUniquePhrase ? 7 : 8);
                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(LeicaYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class LeicaYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public LeicaYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
                return $"{Manufacturer} {Model}".ToViewedUrl();
            }
            else
            {
                return $"{Manufacturer} {Sku}".ToViewedUrl();
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
                case 3: keyPhrase = $"{ProductTypeShort} {Sku}"; break;
                case 4: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Sku}"; break;
                
                case 5: keyPhrase = $"{Manufacturer} {Model}"; break;
                case 6: keyPhrase = $"{ProductTypeShort} {Model}"; break;
                case 7: keyPhrase = $"{ProductTypeShort} {Manufacturer} {Model}"; break;
                case 8: keyPhrase = $"{Model}"; break;

                default: throw new NotImplementedException();
            }

            return keyPhrase.ToKeyPhrase();
        }
    }
}
