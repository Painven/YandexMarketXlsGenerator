using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Rennsteig")]
    public class RennsteigYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Rennsteig";
        public string TranslatedManufacturer => throw new NotImplementedException();
        public List<string> InvalidWords => throw new NotImplementedException();    

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["T"] = "",
            ["U"] = "",
            ["W"] = "+",
            ["X"] = "Активно",
            ["Y"] = "Работает везде",
            ["Z"] = "Rennsteig Зубила||Кернеры||Пробойники||Шаберы||Экстракторы Rennsteig",
            ["AA"] = "Плоские и остроконечные||Ручные и автоматические||Круглые, квадратные и треугольные||Цельные и с вставными пластинами||Спиральные и квадратные",
            ["AB"] = "https://etk-komplekt.ru/ruchnoy-instrument/zubila/shabery-rennsteig/||https://etk-komplekt.ru/ruchnoy-instrument/kernery/kernery-rennsteig/||https://etk-komplekt.ru/ruchnoy-instrument/ekstraktory/ekstraktory-rennsteig/||https://etk-komplekt.ru/elektroinstrument/aksessuary-prinadlezhnosti-i-osnastka-dlya-elektroinstrumenta/probojniki-rennsteig/||https://etk-komplekt.ru/ruchnoy-instrument/zubila/shabery-rennsteig/||https://etk-komplekt.ru/ruchnoy-instrument/ekstraktory/ekstraktory-rennsteig/",
            ["AK"] = "доставка||Доступные цены||Немецкое качество||в наличии"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int lines = string.IsNullOrWhiteSpace(line.Model) ? 3 : 4;
                sb.Append(CreateSection(line, startGroupSectionNumber++, lines));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(RennsteigYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class RennsteigYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public string MODEL_REPLACED
        {
            get
            {
                return Product.Sku.Replace("RE-", "RE ");
            }
        }

        public RennsteigYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
        }

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
            return $"{Product.Sku}";
        }

        protected override string GetViewedUrl()
        {
            string url = $"{Manufacturer} {Product.Model}".ToViewedUrl();
            if(url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url;
        }

        protected override string GetTitle1()
        {
            //Rennsteig RE 430130 кернер

            var title = $"{Manufacturer} {MODEL_REPLACED} {Product.ProductTypeShort}".Trim();
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //RE 430130 Rennsteig 
            var title = $"{MODEL_REPLACED} {Manufacturer}".Trim();
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            //Rennsteig RE 430130 кернер автоматический с доставкой по России

            var title = $"{Manufacturer} {MODEL_REPLACED} {Product.ProductTypeFull} с доставкой по России";
            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            //rennsteig re 430130
            //re 430130 -rennsteig


            string phrase;

            if (lineNumber == 1)
            {
                phrase = $"{Manufacturer} {MODEL_REPLACED}";
            }
            else if (lineNumber == 2)
            {
                phrase = $"{MODEL_REPLACED} -{Manufacturer}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Product.Sku} {Product.ProductTypeShort}";
            }
            else if (lineNumber == 4)
            {
                phrase = $"{Product.Model} {Product.Sku}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase.ToLower().Trim();
        }
    }
}
