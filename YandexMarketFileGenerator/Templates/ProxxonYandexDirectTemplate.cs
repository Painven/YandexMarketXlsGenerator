using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Proxxon")]
    public class ProxxonYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "Proxxon";
        public string Host { get; } = "https://etk-komplekt.ru";
        public string TranslatedManufacturer { get; } = "Проксон";

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
            ["Z"] = "Станки||Шлифмашины||Инструментальные ключи||Отвертки||Граверы||Наборы бит и головок||Наборы инструмента||Токарные станки",
            ["AA"] = "Токарные, Полировальные, сверлильные||Аккумуляторные, удлиненные, угловые||Электрические, динамометрические, наборы||Наборы, в чемодане||С трещоткой, с обгонной муфтой, ударные||Ключи, головки, отвертки||Динамометрические, комбинированные, накидные||Серия PD, FD, с ЧПУ",
            ["AB"] = "https://etk-komplekt.ru/index.php?route=product/category&path=62648||https://etk-komplekt.ru/index.php?route=product/category&path=62662||https://etk-komplekt.ru/index.php?route=product/category&path=62656||https://etk-komplekt.ru/index.php?route=product/category&path=62663||https://etk-komplekt.ru/index.php?route=product/category&path=62678||https://etk-komplekt.ru/index.php?route=product/category&path=62677||https://etk-komplekt.ru/index.php?route=product/category&path=62674||https://etk-komplekt.ru/index.php?route=product/category&path=62653"
        };

        public List<string> InvalidWords => throw new NotImplementedException();

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                int lines = line.IsUniquePhrase ? 7 : (string.IsNullOrEmpty(line.Model) ? 4 : 5);

                sb.Append(CreateSection(line, startGroupSectionNumber++, lines));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(ProxxonYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class ProxxonYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public ProxxonYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Sku}";
        }

        protected override string GetTitle1()
        {
            var title = $"{Manufacturer} {(Product.IsUniquePhrase ? Product.Model : Product.Sku)} {Product.ProductTypeShort}".Trim();
            title = Regex.Replace(title, " +", " ");

            return title;
        }

        protected override string GetTitle2()
        {
            var title = $"{Manufacturer} {Product.Sku}".Trim();
            title = Regex.Replace(title, " +", " ");

            return title;
        }

        protected override string GetTitle3()
        {
            string title = string.Empty;

            if (!string.IsNullOrWhiteSpace(Product.Model))
            {
                title = $"{Product.ProductTypeFull} {Manufacturer} {Product.Model} {Sku} от фициального дилера".Trim();
            }
            else
            {
                title = $"{Product.ProductTypeFull} {Manufacturer} {Sku} от фициального дилера".Trim();
            }

            if(title.Length >= TITLE3_MAX_LENGTH)
            {
                title = $"{Product.ProductTypeFull} {Manufacturer} {Sku} в наличии".Trim();
            }

            title = Regex.Replace(title, " +", " ");
            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            if (lineNumber == 1)
            {
                keyPhrase = $"{Manufacturer} {Product.Sku}";
            }
            else if (lineNumber == 2)
            {
                keyPhrase = $"{Product.Sku} -{Manufacturer}";
            }
            else if (lineNumber == 3)
            {
                keyPhrase = $"{Product.ProductTypeShort} {Manufacturer} {Product.Sku}";
            }
            else if (lineNumber == 4)
            {
                keyPhrase = $"{Manufacturer} {Product.Sku} -{ProductTypeShort}";
            }
            else
            {
                if(Product.IsUniquePhrase)
                {
                    if (lineNumber == 5)
                    {
                        keyPhrase = Product.Model;
                    }
                    if (lineNumber == 6)
                    {
                        keyPhrase = $"{Manufacturer} {Product.Model}";
                    }
                    if (lineNumber == 7)
                    {
                        keyPhrase = $"{Product.Model} {Product.ProductTypeShort}";
                    }
                }
                else
                {
                    if (lineNumber == 5)
                    {
                        keyPhrase = $"{Product.Model} {Product.Sku}";
                    }
                }
            }

            if(string.IsNullOrWhiteSpace(keyPhrase))
            {
                throw new ArgumentNullException();
            }

            keyPhrase = keyPhrase.Replace("/", " ").Replace(",", ".").Replace("(", string.Empty).Replace(")", string.Empty)
                                 .Replace("\"", string.Empty).Replace("'", string.Empty);


            if(keyPhrase.Split().Length >= 7)
            {
                throw new ArgumentOutOfRangeException();
            }

            return keyPhrase.ToLower();
        }

        protected override string GetViewedUrl()
        {
            return GetGroupName().ToViewedUrl();
        }
    }
}
