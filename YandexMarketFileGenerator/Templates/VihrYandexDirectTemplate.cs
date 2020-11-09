using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Вихрь")]
    public class ВихрьYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Вихрь";
        public string TranslatedManufacturer => "Vihr";
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
            ["Z"] = "ВИХРЬ насосы||шуруповерты||дренажные насосы||насосные станции||дрели||пилы||бетономешалки||умывальники ВИХРЬ",
            ["AA"] = "насосы вибрационные, дренажные, фекальные, циркуляционные||шуруповерты аккумуляторные, сетевые, дрели-шуруповерты||дренажные насосы дн, дн-н||поверхностные с электрическим двигателем, синхронные||дрели сетевые, аккумуляторные, дрели-миксеры, ударные||пилы дисковые, циркулярные электропилы||бетоносмести с электромотором, бетономешалки строителньые||умывальники для дачи с подогревом и без",
            ["AB"] = "https://etk-komplekt.ru/stroitelstvo/nasosy-i-nasosnye-stancii/nasosy-vihr/||https://etk-komplekt.ru/instrumenty/elektroinstrument/shurupoverty/shurupoverty-vihr/||https://etk-komplekt.ru/stroitelstvo/nasosy-i-nasosnye-stancii/nasosy-vihr/drenazhnye-nasosy-vihr/||https://etk-komplekt.ru/stroitelstvo/nasosy-i-nasosnye-stancii/nasosnye-stancii-vihr/||https://etk-komplekt.ru/instrumenty/elektroinstrument/dreli/dreli-vihr/||https://etk-komplekt.ru/instrumenty/elektroinstrument/elektropily/diskovye-pily/diskovye-pily-vihr/||https://etk-komplekt.ru/instrumenty/elektroinstrument/betonomeshalki/betonomeshalki-vihr/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/umyvalniki/umyvalniki-vihr/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||Сделано в России"
        };
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                bool notSame = line.Model != line.Sku;

                int count = notSame ? 5 : 3;
                if(line.IsUniquePhrase)
                {
                    count++;
                }

                sb.Append(CreateSection(line, startGroupSectionNumber++, count));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(ВихрьYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }  
    }

    internal class ВихрьYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public ВихрьYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
            Product.Manufacturer = "Вихрь";
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
            string url = $"{Manufacturer} {Product.Sku}".ReplaceAll(new[] { " ", ".", "/", "_" }, newSubString: "-");

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
            var title = $"{Product.Manufacturer} {Product.Sku} {Product.ProductTypeShort}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //Вихрь ЗПТ-305-1800 ПЛ
            string title = null;

            if(Product.Model != Product.Sku)
            {
                title = $"{Product.Manufacturer} {Product.Model} {Product.Sku}";
            }
            else
            {
                title = $"{Product.Manufacturer} {Product.Sku}";
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
                title = $"{Product.Manufacturer} {Product.Model} (арт. {Product.Sku}) {Product.ProductTypeFull} с доставкой по России!";
            }
            else if(Product.Model != Product.Sku)
            {
                title = $"{Product.Manufacturer} {Product.Sku} {Product.Model} {Product.ProductTypeFull} с доставкой по России!";
            }
            else
            {
                title = $"{Product.Manufacturer} {Product.Sku} {Product.ProductTypeFull} с доставкой по России!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = title = $"{Product.Manufacturer} {Product.Sku} {Product.ProductTypeFull} в наличии";
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
                phrase = $"{Product.Manufacturer} {Product.Sku}";
            }
            else if (lineNumber == 2)
            {
                phrase = $"{TranslatedManufacturer} {Product.Sku}";
            }
            else if (lineNumber == 3)
            {
                phrase = $"{Product.Sku} -{Product.Manufacturer}";
            }
            else if (lineNumber == 4 && notSame)
            {
                phrase = $"{Product.Manufacturer} {Product.Model} {Product.Sku}";
            }
            else if (lineNumber == 5 && notSame)
            {
                phrase = $"{Product.Model} {Product.Sku}";
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
