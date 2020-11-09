using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Heyco")]
    public class HeycoYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Heyco";
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
            ["Z"] = "HEYCO Наборы инструмента||Рожковые ключи||Наборы головок||Монтировки||Ящики для инструмента||Сумки||Молотки||Ножи HEYCO",
            ["AA"] = "Плоские и остроконечные||Ручные и автоматические||Круглые, квадратные и треугольные||Цельные и с вставными пластинами||Спиральные и квадратные",
            ["AB"] = "https://etk-komplekt.ru/nabory-instrumentov/nabory-instrumenta-heyconabor-instrumenta-heyco/||https://etk-komplekt.ru/ruchnoy-instrument/instrumentalnye-klyuchi/rozhkovye-klyuchi/rozhkovye-klyuchi-heyco/||https://etk-komplekt.ru/nabory-instrumentov/nabor-bit/nabory-golovok-heyco/||https://etk-komplekt.ru/ruchnoy-instrument/montirovki/montirovki-heycomontirovki-heyco/||https://etk-komplekt.ru/hranenie-instumenta/yashhiki-dlya-instrumenta/yashhiki-dlya-instrumenta-heyco/||https://etk-komplekt.ru/hranenie-instumenta/sumki-i-ryukzaki-dlya-detaley/sumki-heyco/||https://etk-komplekt.ru/ruchnoy-instrument/molotki-kiyanki-kuvaldy/molotky/molotki-heyco/||https://etk-komplekt.ru/ruchnoy-instrument/nozhi/nozhi-texnicheskie-heyco/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||Немецкое качество"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 2));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(HeycoYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class HeycoYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public string MODEL_WITH_SPACE
        {
            get
            {
                return Product.Model.Replace("HE-", "HE ");
            }
        }

        public string MODEL_WITHOUT_PREFIX
        {
            get
            {
                return Product.Model.Replace("HE-", string.Empty);
            }
        }

        public HeycoYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            return $"{Manufacturer.ToUpper()} {MODEL_WITH_SPACE}";
        }

        protected override string GetViewedUrl()
        {
            string url = $"{Manufacturer} {Product.Model}".ReplaceAll(new[] { " ", ".", "/", "_" }, newSubString: "-");
            
            if(url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url.ToUpper();
        }

        protected override string GetTitle1()
        {
            //Heyco RE 430130 кернер

            var title = $"{Manufacturer} {MODEL_WITH_SPACE} {Product.ProductTypeShort}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //RE 430130 Heyco 
            var title = $"{MODEL_WITH_SPACE} {Manufacturer}";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            var title = $"{Manufacturer} {MODEL_WITH_SPACE} {Product.ProductTypeFull} купить с доставкой по России!";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Manufacturer} {MODEL_WITH_SPACE} {Product.ProductTypeFull} в наличии!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }


            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            //Heyco re 430130
            //re 430130 -Heyco


            string phrase;

            if (lineNumber == 1)
            {
                phrase = $"{Manufacturer} {MODEL_WITH_SPACE}";
            }
            else if (lineNumber == 2)
            {
                phrase = $"{MODEL_WITH_SPACE} -{Manufacturer}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase.ToLower().Trim();
        }
    }
}
