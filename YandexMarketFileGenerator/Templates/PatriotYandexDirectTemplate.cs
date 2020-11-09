using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Patriot")]
    public class PatriotYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Patriot";
        public string TranslatedManufacturer => "Патриот";
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
            ["Z"] = "PATRIOT триммеры||ранцевые распылители||измельчители||мотоблоки||перфораторы||бензопилы||генераторы||станки PATRIOT",
            ["AA"] = "триммеры бензиновые, электрические||распылители, опрыскиватели ранцевые, аккумуляторные||измельчители садовые, бензиновые и электрические||мотоблоки бензиновые||перфораторы сетевые, аккумуляторные||цепные бензиновые пилы||генераторы бензиновые, инверторные||станки сверлильные, рейсмусовые, точильные, фуговальные",
            ["AB"] = "https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/trimmery/trimmery-patriot/trimmery-benzinovye-patriot/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/polivochnoe-oborudovanie/opryskivateli-patriot/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/izmelchiteli-sadovye/izmelchiteli-sadovye-patriot/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/motobloki/motobloki-patriot/||https://etk-komplekt.ru/instrumenty/elektroinstrument/perforatory-i-otboynye-molotki/perforatory/perforatory-patriot/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/czepnye-pily/cepnye-pily-patriot/cepnye-benzinovye-pily-patriot/||https://etk-komplekt.ru/instrumenty/benzoinstrument/generatory-i-elektrostancii/generatory-patriot/||https://etk-komplekt.ru/instrumenty/elektroinstrument/stanki/stanki-patriot/",
            ["AK"] = "Доступные цены||официальный дилер||доставка||в наличии"
        };
     
        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int count = (line.Model != line.Sku) ? 5 : 3; 
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
            var data = new YandexMarketSection(this, typeof(PatriotYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(linesCount);
        }
       
    }

    internal class PatriotYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public PatriotYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
            Product.Manufacturer = "Patriot";
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

            return url.ToUpper();
        }

        protected override string GetTitle1()
        {
            //Patriot Sku ShortName 

            var title = $"{Product.Manufacturer} {Product.Sku} {Product.ProductTypeShort}";
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {

            string title = null;

            if(Product.Model != Product.Sku)
            {
                title = $"{Product.Manufacturer} {Product.Model} ({Product.Sku})";
            }
            else
            {
                title = $"{Product.Manufacturer} {Product.Sku}";
            }

            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                title = $"{Product.Manufacturer} {Product.Sku}";
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            string modelOrSku = !string.IsNullOrEmpty(Product.Model) ? $"{Product.Model} {Product.Sku}" : Product.Sku;

            string title = null;

            if(Product.Model != Product.Sku)
            {
                title = $"{Product.Manufacturer} {Product.Model} (арт. {Product.Sku}) {Product.ProductTypeFull} с доставкой по России!";
            }
            else
            {
                title = $"{Product.Manufacturer} {Product.Sku} {Product.ProductTypeFull} с доставкой по России!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Product.Manufacturer} {Product.Sku} {Product.ProductTypeFull} в наличии!";
            }

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {

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
                phrase = Product.Sku; 
            }
            else if (lineNumber == 4 && Product.Model != Product.Sku)
            {
                phrase = $"{Product.Model} {Product.Sku}";
            }
            else if(lineNumber == 5 && Product.Model != Product.Sku)
            {
                phrase = $"{Product.Manufacturer} {Product.Model} {Product.Sku}";
            }
            if((lineNumber == 4 || lineNumber == 6) && Product.IsUniquePhrase && (Product.Model != Product.Sku))
            {
                phrase = Product.Model;
            }

            return phrase.ToLower().Trim();
        }
    
    }
}
