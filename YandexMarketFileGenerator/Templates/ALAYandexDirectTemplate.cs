using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Anapico|LeCroy|Anritsu")]
    public class ALAYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer => throw new NotSupportedException();
        public string Host { get; } = "https://etk-komplekt.ru";
        public string TranslatedManufacturer => throw new NotSupportedException();

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "мало показов",
            ["S"] = "Россия",
            ["W"] = "Активно",
            ["X"] = "Работает везде"
            };

        public List<string> InvalidWords => throw new NotImplementedException();


        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, GetGroupIndex(line), 2));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(ALAYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }

        private int GetGroupIndex(OpenCartProductLine product)
        {
            switch(product.Manufacturer)
            {
                case "AnaPico": return 1 ;
                case "LeCroy": return 2;
                case "Anritsu": return 3;

                default: throw new NotSupportedException();
            }
        }
    }

    internal class ALAYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public ALAYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
        {
        }

        protected override void FillDictionary(int lineNumber)
        {
            resultDictionary["A"] = parentSection.ParentTemplate.ColumnStaticValues["A"];
            resultDictionary["B"] = parentSection.ParentTemplate.ColumnStaticValues["B"];
            resultDictionary["C"] = parentSection.ParentTemplate.ColumnStaticValues["C"];
            resultDictionary["D"] = string.Empty;
            resultDictionary["E"] = GetGroupName();
            resultDictionary["F"] = parentSection.GroupIndex.ToString();
            resultDictionary["G"] = parentSection.ParentTemplate.ColumnStaticValues["G"];
            resultDictionary["H"] = string.Empty;
            resultDictionary["I"] = GetPhrase(lineNumber);
            resultDictionary["J"] = string.Empty;
            resultDictionary["K"] = GetTitle1();
            resultDictionary["L"] = GetTitle2();
            resultDictionary["M"] = GetTitle3();
            resultDictionary["N"] = string.Empty;
            resultDictionary["O"] = string.Empty;
            resultDictionary["P"] = string.Empty;
            resultDictionary["Q"] = FullUrlPath + "?utm_source=yandex";
            resultDictionary["R"] = GetViewedUrl();
            resultDictionary["S"] = parentSection.ParentTemplate.ColumnStaticValues["S"];
            resultDictionary["T"] = string.Empty;
            resultDictionary["U"] = string.Empty;
            resultDictionary["V"] = string.Empty;
            resultDictionary["W"] = parentSection.ParentTemplate.ColumnStaticValues["W"];
            resultDictionary["X"] = parentSection.ParentTemplate.ColumnStaticValues["X"];
            resultDictionary["Y"] = GetFastGroupCaption();
            resultDictionary["Z"] = string.Empty;
            resultDictionary["AA"] = GetFastUri();
            resultDictionary["AB"] = string.Empty;
            resultDictionary["AC"] = string.Empty;
            resultDictionary["AD"] = string.Empty;
            resultDictionary["AE"] = string.Empty;
            resultDictionary["AF"] = string.Empty;
            resultDictionary["AG"] = string.Empty;
            resultDictionary["AH"] = string.Empty;
            resultDictionary["AI"] = string.Empty;
            resultDictionary["AJ"] = GetDetailsString();
            resultDictionary["AK"] = GetMinusPharese();
            resultDictionary["AL"] = string.Empty;
            resultDictionary["AM"] = string.Empty;
            resultDictionary["AN"] = string.Empty;
            resultDictionary["AO"] = string.Empty;
            resultDictionary["AP"] = string.Empty;
            resultDictionary["AQ"] = string.Empty;
            resultDictionary["AR"] = string.Empty;
            resultDictionary["AS"] = string.Empty;
            resultDictionary["AT"] = string.Empty;
            resultDictionary["AU"] = string.Empty;
        }

        private string GetMinusPharese()
        {
            switch (Product.Manufacturer)
            {
                case "AnaPico": return "-поверка -сертификат -скачать";
                case "LeCroy": return string.Empty;
                case "Anritsu": return string.Empty;
                default: throw new NotSupportedException();
            }
        }

        private string GetDetailsString()
        {
            switch (Product.Manufacturer)
            {
                case "AnaPico": return "официальный дилер||Генераторы AnaPico";
                case "LeCroy": return "официальный дилер||осцилограф||анализатор";
                case "Anritsu": return string.Empty;
                default: throw new NotSupportedException();
            }
        }

        private string GetFastUri()
        {
            switch(Product.Manufacturer)
            {
                case "AnaPico": return "https://etk-komplekt.ru/anapico||https://etk-komplekt.ru/izmeritelnye-pribory/generatory/generatory-anapico/||https://etk-komplekt.ru/izmeritelnye-pribory/analizatoryi-spektra/analizatory-spektra-anapico/||https://etk-komplekt.ru/izmeritelnye-pribory/sintezatory-chastot/sintezatory-chastot-anapico/";
                case "LeCroy": return "https://etk-komplekt.ru/lecroy||https://etk-komplekt.ru/izmeritelnye-pribory/analizatory-protokolov/analizatory-protokolov-lecroy/||https://etk-komplekt.ru/izmeritelnye-pribory/oscillografy/cifrovye-oscillografy/oscillografy-lecroy/||https://etk-komplekt.ru/izmeritelnye-pribory/aksessuary-i-prinadlezhnosti/aksessuary-dlya-priborov-lecroy-kupit-v-sankt-peterburge/";
                case "Anritsu": return "https://etk-komplekt.ru/anritsu||https://etk-komplekt.ru/izmeritelnye-pribory/analizatory-cepej/analizatory-cepey-anritsu/||https://etk-komplekt.ru/izmeritelnye-pribory/generatory/generatory-anritsu/||https://etk-komplekt.ru/izmeritelnye-pribory/reflektometry/reflektometry-anritsu/";
                default: throw new NotSupportedException();
            }
        }

        private string GetFastGroupCaption()
        {
            switch (Product.Manufacturer)
            {
                case "AnaPico": return "AnaPico||Генераторы AnaPico||Анализаторы||Синтезаторы AnaPico";
                case "LeCroy": return "LeCroy||Анализаторы||Осцилографы||Аксессуары LeCroy";
                case "Anritsu": return "Antitsu||Анализаторы||Генераторы||Рефлектометры Anritsu";
                default: throw new NotSupportedException();
            }
        }

        protected override string GetGroupName()
        {
            return Product.Manufacturer;
        }

        protected override string GetTitle1()
        {
            if(Product.Manufacturer.Equals("AnaPico"))
            {
                //AnaPico PNA20 Анализатор 

                return $"AnaPico {Product.Model} {Product.ProductTypeShort}";
            }
            else if (Product.Manufacturer.Equals("LeCroy"))
            {
                //T3AWG3 LeCroy Генератор

                return $"{Product.Model} LeCroy {Product.ProductTypeShort}";
            }
            else if (Product.Manufacturer.Equals("Anritsu"))
            {
                //MS2037С Anritsu Анализатор 

                return $"{Product.Model} Anritsu {Product.ProductTypeShort}";
            }
            else
            {
                throw new NotSupportedException();
            }

        }

        protected override string GetTitle2()
        {
            if (Product.Manufacturer.Equals("AnaPico"))
            {
                //AnaPico PNA20

                return $"AnaPico {Product.Model}";
            }
            else if (Product.Manufacturer.Equals("LeCroy"))
            {
                //T3AWG3 LeCroy 

                return $"{Product.Model} LeCroy";
            }
            else if (Product.Manufacturer.Equals("Anritsu"))
            {
                //MS2037С Anritsu

                return $"{Product.Model} Anritsu";
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        protected override string GetTitle3()
        {
            if (Product.Manufacturer.Equals("AnaPico"))
            {
                //AnaPico PNA20 Анализатор фазовых шумов! Официальный дилер, доставка по России!

                return $"AnaPico {Product.Model} {Product.ProductTypeFull}! Официальный дилер, доставка по России!";
            }
            else if (Product.Manufacturer.Equals("LeCroy"))
            {
                //T3AWG3 LeCroy Генераторы от официального дилера

                return $"{Product.Model} LeCroy {Product.ProductTypeFull} от официального дилера";
            }
            else if (Product.Manufacturer.Equals("Anritsu"))
            {
                //MS2037С Anritsu Анализатор от официального дилера. Доставка по России.

                return $"{Product.Model} Anritsu {Product.ProductTypeFull} от официального дилера. Доставка по России.";
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        protected override string GetPhrase(int lineNumber)
        {
            if (Product.Manufacturer.Equals("AnaPico"))
            {
                //PNA20 -anapico
                //AnaPico PNA20
                if(lineNumber == 1)
                {
                    return $"{Product.Model} -anapico";
                }
                if(lineNumber == 2)
                {
                    return $"AnaPico {Product.Model}";
                }

            }
            else if (Product.Manufacturer.Equals("LeCroy"))
            {
                if (lineNumber == 1)
                {
                    return Product.Model;
                }
                if (lineNumber == 2)
                {
                    return $"{Product.Model} LeCroy";
                }
            }
            else if (Product.Manufacturer.Equals("Anritsu"))
            {
                //MS2037С Anritsu
                //MS2037С -anritsu

                if (lineNumber == 1)
                {
                    return $"{Product.Model} Anritsu";
                }
                if (lineNumber == 2)
                {
                    return $"{Product.Model} -anritsu";
                }
            }

            throw new NotSupportedException();
        }

        protected override string GetViewedUrl()
        {
            string url = Product.Model
                .Replace(" ", "-")
                .Replace(",", "-")
                .Trim();

            return url;
        }
    }
}
