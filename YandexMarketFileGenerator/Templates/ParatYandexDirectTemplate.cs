using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Parat")]
    public class ParatYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "PARAT";
        public string TranslatedManufacturer => "Парат";
        public List<string> InvalidWords => throw new NotImplementedException();    

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["W"] = "+",
            ["X"] = "Активно",
            ["Y"] = "Работает везде",
            ["Z"] = "PARAT портфели чемоданы||Светодиодные фонари||Контейнеры||Принадлежности",
            ["AA"] = "Профессиональные кейсы для инструмента||||Пластиковые для деталей и инструмента Parat||Принадлежности для портфелей и чемоданов Parat",
            ["AB"] = "https://etk-komplekt.ru/hranenie-instumenta/kejs-dlya-instrumenta/portfeli-i-chemodany-dlya-instrumentov-parat/||https://etk-komplekt.ru/ruchnoy-instrument/fonari/svetodiodnye-fonari-parat/||https://etk-komplekt.ru/hranenie-instumenta/kontejneryi-dly-detaleyi/kontejnery-dlya-detalej-i-instrumenta-parat/||https://etk-komplekt.ru/hranenie-instumenta/kejs-dlya-instrumenta/portfeli-i-chemodany-dlya-instrumentov-parat/prinadlezhnosti-dlya-portfelej-i-chemodanov-parat/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||Немецкое качество"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 5));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(ParatYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class ParatYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public ParatYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
        }

        public string MODEL_REPLACED => Product.Model.Replace("PA-", string.Empty);

        protected override void FillDictionary(int lineNumber)
        {
            var range1 = Enumerable.Range(0, 26)
                .Select(i => (char)((int)'A' + i))
                .Select(c => c.ToString())
                .ToList();

            var range2 = Enumerable.Range(0, 21)
                .Select(i => (char)((int)'A' + i))
                .Select(c => string.Concat("A", c.ToString()))
                .ToList();

            var cells = range1.Concat(range2).ToList();

            foreach (var cellLetter in cells)
            {
                if (parentSection.ParentTemplate.ColumnStaticValues.ContainsKey(cellLetter))
                {
                    resultDictionary[cellLetter] = parentSection.ParentTemplate.ColumnStaticValues[cellLetter];
                }
                else
                {
                    resultDictionary[cellLetter] = string.Empty;
                }
            }

            resultDictionary["E"] = GetGroupName();
            resultDictionary["F"] = parentSection.GroupIndex.ToString();
            resultDictionary["I"] = GetPhrase(lineNumber);
            resultDictionary["K"] = GetTitle1();
            resultDictionary["L"] = GetTitle2();
            resultDictionary["M"] = GetTitle3();
            resultDictionary["Q"] = FullUrlPath;
            resultDictionary["R"] = GetViewedUrl();
            resultDictionary["AV"] = Product.Price == decimal.Zero ? "" : Product.Price.ToString("F0");
        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Model}";
        }

        protected override string GetViewedUrl()
        {
            string url = $"{Manufacturer} {MODEL_REPLACED}".ReplaceAll(new[] { " ", ".", "/", "_" }, newSubString: "-");

            if (url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                url = Product.Model.ReplaceAll(new[] { " ", ".", "/", "_" }, newSubString: "-");
            }

            if (url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url;
        }

        protected override string GetTitle1()
        {
            var title = $"{Manufacturer} {MODEL_REPLACED} {Product.ProductTypeShort}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{Manufacturer} {Product.Model}";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            var title = $"{Manufacturer} {Product.Model} {Product.ProductTypeFull} с доставкой по России!";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Manufacturer} {Product.Model} {Product.ProductTypeFull} в наличии!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            // PA-5533000060
            // Parat 5533000060
            // Парат 5533000060
            // Parat 5533000060 название
            // 5533000060 название -Parat

            string phrase;

            if (lineNumber == 1)
            {
                phrase = Product.Model;
            }
            else if (lineNumber == 2)
            {
                phrase = $"{Manufacturer} {MODEL_REPLACED}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{TranslatedManufacturer} {MODEL_REPLACED}";
            }
            else if (lineNumber == 4)
            {
                phrase = $"{Manufacturer} {MODEL_REPLACED} {Product.ProductTypeShort}";
            }
            else if (lineNumber == 5)
            {
                phrase = $"{MODEL_REPLACED} {Product.ProductTypeShort} -{Manufacturer}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase.ToLower().Trim();
        }
    }
}
