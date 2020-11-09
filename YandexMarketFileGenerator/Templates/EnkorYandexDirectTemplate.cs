using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Энкор")]
    public class ЭнкорYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Энкор";
        public string TranslatedManufacturer => "Enkor";
        public List<string> InvalidWords => throw new NotImplementedException();

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["T"] = "23605641314",
            ["U"] = "",
            ["W"] = "+",
            ["X"] = "Активно",
            ["Y"] = "Работает везде",
            ["Z"] = "ЭНКОР дрели||наборы||пилы||станки||триммеры||фрезы||шуруповерты||газонокосилки ЭНКОР",
            ["AA"] = "дрели-миксеры, ударные, аккумуляторные, дрели-шуруповерты||для сборочных и ремонтных работ, наборы отверток и ключей||электропилы сабельные, циркулярные, дисковые||станки фрезерные, токарные, шлифовальные||триммеры электрические и бензиновые||пазовые, кромочные, универсальные, фигерейные, фальцевые||шуруповерты сетевые, аккумуляторные, ударные||газонокосилки электрические и бензиновые",
            ["AB"] = "https://etk-komplekt.ru/instrumenty/elektroinstrument/dreli/dreli-enkor/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabory-instrumenta-enkor/||https://etk-komplekt.ru/instrumenty/elektroinstrument/elektropily/pily-enkor/||https://etk-komplekt.ru/instrumenty/elektroinstrument/stanki/stanki-enkor/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/trimmery/trimmery-enkor/||https://etk-komplekt.ru/instrumenty/elektroinstrument/aksessuary-prinadlezhnosti-i-osnastka-dlya-elektroinstrumenta/frezy/frezy-enkor/||https://etk-komplekt.ru/instrumenty/elektroinstrument/shurupoverty/shurupoverty-enkor/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/gazonokosilki/gazonokosilki-enkor/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||Сделано в России"
        };
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                bool notSame = line.Model != line.Sku;

                int count = notSame ? 4 : 2;
                if(line.IsUniquePhrase)
                {
                    count += 2;
                }

                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(ЭнкорYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class ЭнкорYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public ЭнкорYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection) { }

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
            string url = $"{Manufacturer} {Product.Sku}".ToViewedUrl();

            if (url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                url = Product.Model.ToViewedUrl();
            }

            if (url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url.ToUpper();
        }

        protected override string GetTitle1()
        {
            var title = $"{Manufacturer} {Product.Sku} {Product.ProductTypeShort}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //Энкор ЗПТ-305-1800 ПЛ
            string title = null;

            if(Product.Model != Product.Sku)
            {
                title = $"{Manufacturer} {Product.Model} {Product.Sku}";
            }
            else
            {
                title = $"{Manufacturer} {Product.Sku}";
            }

            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            string title = null;

            if(Product.Model != Product.Sku && Product.IsUniquePhrase)
            {
                title = $"{Manufacturer} {Product.Model} (арт. {Product.Sku}) {Product.ProductTypeFull} с доставкой по России!";
            }
            else if(Product.Model != Product.Sku)
            {
                title = $"{Manufacturer} {Product.Model} {Product.Sku} {Product.ProductTypeFull} с доставкой по России!";
            }
            else
            {
                title = $"{Manufacturer} {Product.Sku} {Product.ProductTypeFull} с доставкой по России!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Manufacturer} {Product.Sku} {Product.ProductTypeFull} в наличии";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            bool notSame = Product.Model != Product.Sku;
            string phrase = string.Empty;

            if (lineNumber == 1)
            {
                phrase = $"{Manufacturer} {Product.Sku}";
            }
            else if (lineNumber == 2)
            {
                phrase = $"{Product.ProductTypeShort} {Manufacturer} {Product.Sku}";
            }
            else if (lineNumber == 3 && notSame)
            {
                phrase = $"{Manufacturer} {Product.Model} {Product.Sku}";
            }
            else if (lineNumber == 4 && notSame)
            {
                phrase = $"{Product.Model} {Product.Sku}";
            }
            else if ((lineNumber == 3 || lineNumber == 5) && Product.IsUniquePhrase)
            {
                phrase = $"{Product.ProductTypeShort} {Product.Model}";
            }
            else if ((lineNumber == 4 || lineNumber == 6) && Product.IsUniquePhrase)
            {
                phrase = Product.Model;
            }
            else
            {
                throw new NotImplementedException();
            }

            return phrase.ToLower().Trim();
        }
    }
}
