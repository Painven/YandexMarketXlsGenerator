using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для MTD")]
    public class MTDYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "MTD";
        public string TranslatedManufacturer { get; } = "МТД";
        public List<string> InvalidWords => throw new NotImplementedException();    
        
        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Санкт-Петербург и Ленинградская область",
            ["T"] = "",
            ["U"] = "25",
            ["W"] = "+",
            ["X"] = "Активно",
            ["Y"] = "Работает везде",
            ["Z"] = "MTD||Газонокосилки MTD||Культиваторы MTD||Тримеры MTD||Вертикуттеры MTD||Аэраторы MTD||Измельчитель MTD||Трактора MTD ",
            ["AA"] = "Весь инструмент||Бензиновые и электрические||Бензиновые||Бензиновые, электрические, колесные||||Прицепной||Садовый бензиновый||Минитрактора и райдеры садовые",
            ["AB"] = "https://etk-komplekt.ru/mtd||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/gazonokosilki/gazonokosilki-mtd/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/kultivatory/kultivatory-mtd/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/trimmery/trimmery-mtd/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/vertikuttery/vertikuttery-mtd/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/aeratory/aeratory-mtd/||https://etk-komplekt.ru/izmelchitel-sadovyj-benzinovyj-mtd-rover-464-q||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/minitraktora-i-minirajdery/minitraktora-mtd/",
            ["AK"] = "Доступные цены||официальный дилер||доставка"
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
            var data = new YandexMarketSection(this, typeof(MTDYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }
   
    }

    internal class MTDYandexMarketSectionLine : YandexMarketSectionLineBase
    {
 
        public MTDYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            return $"{Manufacturer.ToUpper()} {Product.Sku}";
        }

        protected override string GetViewedUrl()
        {
            string url = $"{Manufacturer} {Product.Sku}".ToUpper().ReplaceAll(new[] { " ", ".", "/", "_" }, newSubString: "-");
            if(url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url;
        }

        protected override string GetTitle1()
        {
            var title = $"{Manufacturer} {Product.Model} {Product.ProductTypeShort}".Trim();
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            string part = string.IsNullOrWhiteSpace(Product.Sku) ? Product.Model : Product.Sku;
            var title = $"{Manufacturer} {part}".Trim();
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            //MTD OPTIMA 46 SPB HW газонокосилка бензиновая самоходная с доставкой по России!
            var title = $"{Manufacturer} {Product.Model} {Product.ProductTypeFull} с доставкой по России!";

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string part = string.IsNullOrWhiteSpace(Product.Sku) ? Product.Model : Product.Sku;
            string phrase;
            //mtd 46 spb hw
            //46 spb hw -mtd
            //газонокосилки mtd 46 spb


            if (lineNumber == 1)
            {
                phrase = $"{Manufacturer} {Product.Model}";
            }
            else if (lineNumber == 2)
            {
                phrase = $"{part} -mtd";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Model}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            return phrase.ToLower().Trim(); ;
        }
    }
}
