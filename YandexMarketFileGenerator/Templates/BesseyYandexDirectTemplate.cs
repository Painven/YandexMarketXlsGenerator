using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Bessey")]
    public class BesseyYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "BESSEY";
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
            ["Z"] = "Bessey Струбцины||Зажимы Bessey||Ножницы Bessey",
            ["AA"] = "Ручные. C, U образные, легкие, рычажные, для сварки||Ручные, настольные, наборы, стелажи||Идеальные ножницы для прорезания отверстий",
            ["AB"] = "https://etk-komplekt.ru/ruchnoy-instrument/strubcziny/strubcziny-bessey/||https://etk-komplekt.ru/ruchnoy-instrument/tiski-i-derzhateli/zazhimnye-elementy-i-vspomogatelnoe-oborudovanie-bessey/||https://etk-komplekt.ru/ruchnoy-instrument/nozhnicy/nozhniczy-bessey/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||Гарантия качества||Выставочный зал в СПб"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 3));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(BesseyYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class BesseyYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public string MODEL_WITH_SPACE
        {
            get
            {
                return Product.Model.Replace("BE-", "BE ");
            }
        }
        public string MODEL_WITHOUT_PREFIX
        {
            get
            {
                return Product.Model.Replace("BE-", string.Empty);
            }
        }

        public BesseyYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            return $"{Manufacturer.ToUpper()} {Product.Model}";
        }

        protected override string GetViewedUrl()
        {
            string url = $"{Manufacturer} {MODEL_WITHOUT_PREFIX}".ReplaceAll(new[] { " ", ".", "/", "_" }, newSubString: "-");

            if (url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                url = Product.Model.ReplaceAll(new[] { " ", ".", "/", "_" }, newSubString: "-");
            }

            if (url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url.ToUpper();
        }

        protected override string GetTitle1()
        {
            //BESSEY BE ST290 Подпорка

            var title = $"{Manufacturer} {MODEL_WITH_SPACE} {Product.ProductTypeShort}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //BE ST290 BESSEY
            var title = $"{MODEL_WITH_SPACE} {Manufacturer}";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            //Bessey RE 430130 кернер автоматический с доставкой по России
            //BESSEY RE 430130 Цельнометаллическая струбцина c доставкой по России!

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
            //BE-ST290
            //Bessey ST290
            //Bessey BE-ST290


            string phrase;

            if (lineNumber == 1)
            {
                phrase = Product.Model;
            }
            else if (lineNumber == 2)
            {
                phrase = $"Bessey {MODEL_WITHOUT_PREFIX}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"Bessey {Product.Model}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase.ToLower().Trim();
        }
    }
}
