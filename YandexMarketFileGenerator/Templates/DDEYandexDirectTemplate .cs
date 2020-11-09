using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для DDE")]
    public class DDEYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Manufacturer { get; } = "DDE";
        public string Host { get; } = "https://etk-komplekt.ru";
        public string TranslatedManufacturer { get; } = "ДДЕ";

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",       
            ["V"] = "+",
            ["W"] = "Активно",
            ["X"] = "Работает везде",
            ["Y"] = "Генераторы DDE||Культиваторы DDE||Газонокосилки DDE||Бензопилы DDE",
            ["AA"] = "https://etk-komplekt.ru/benzoinstrument/generatory-i-elektrostancii/generatory-benzinovye-dde/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/kultivatory/kultivatory-dde/||https://etk-komplekt.ru/oborudovanie-dlya-sada-i-dachi/gazonokosilki/gazonokosilki-dde/||https://etk-komplekt.ru/benzoinstrument/benzopily/benzopily-dde/",
            ["AJ"] = "Двигатели DDE||Триммера DDE||Запчасти DDE||Мотоблоки DDE||Мотопомпы DDE"
        };

        public List<string> InvalidWords => throw new NotImplementedException();


        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 4));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(DDEYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(4);
        }
    }

    internal class DDEYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public DDEYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
            resultDictionary["Q"] = FullUrlPath;
            resultDictionary["R"] = GetViewedUrl();
            resultDictionary["S"] = parentSection.ParentTemplate.ColumnStaticValues["S"];
            resultDictionary["T"] = string.Empty;
            resultDictionary["U"] = string.Empty;
            resultDictionary["V"] = parentSection.ParentTemplate.ColumnStaticValues["V"];
            resultDictionary["W"] = parentSection.ParentTemplate.ColumnStaticValues["W"];
            resultDictionary["X"] = parentSection.ParentTemplate.ColumnStaticValues["X"];
            resultDictionary["Y"] = parentSection.ParentTemplate.ColumnStaticValues["Y"];
            resultDictionary["Z"] = string.Empty;
            resultDictionary["AA"] = parentSection.ParentTemplate.ColumnStaticValues["AA"];
            resultDictionary["AB"] = string.Empty;
            resultDictionary["AC"] = string.Empty;
            resultDictionary["AD"] = string.Empty;
            resultDictionary["AE"] = string.Empty;
            resultDictionary["AF"] = string.Empty;
            resultDictionary["AG"] = string.Empty;
            resultDictionary["AH"] = string.Empty;
            resultDictionary["AI"] = string.Empty;
            resultDictionary["AJ"] = parentSection.ParentTemplate.ColumnStaticValues["AJ"];
            resultDictionary["AK"] = string.Empty;
            resultDictionary["AL"] = string.Empty;
            resultDictionary["AM"] = string.Empty;
            resultDictionary["AN"] = string.Empty;
            resultDictionary["AO"] = string.Empty;
            resultDictionary["AP"] = string.Empty;
            resultDictionary["AQ"] = string.Empty;
            resultDictionary["AR"] = string.Empty;
            resultDictionary["AS"] = string.Empty;
            resultDictionary["AT"] = string.Empty;
            resultDictionary["AU"] = parentSection.ProductLine.Price.ToString("F0");
        }

        protected override string GetGroupName()
        {
            return $"{Manufacturer} {Product.Model}".Trim().ToLower().Replace("-dde", "dde");
        }

        protected override string GetTitle1()
        {
            //DDE ET1200 культиватор в наличии! 
            var title = $"{Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeShort}".Trim();
            title = Regex.Replace(title, " +", " ");

            if (title.Length < (TITLE1_MAX_LENGTH - " в наличии!".Length))
            {
                title = title + " в наличии!";
            }

            return title;
        }

        protected override string GetTitle2()
        {
            //DDE ET1200 культиватор!
            var title = $"{Manufacturer} {ModelWithoutManufacturerName} {Product.ProductTypeShort}!".Trim();

            title = Regex.Replace(title, " +", " ");

            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                
            }


            return title;
        }

        protected override string GetTitle3()
        {
            //культиватор dde et1200
            var title = $"{Product.ProductTypeFull} {Manufacturer.ToLower()} {ModelWithoutManufacturerName.ToLower()}".Trim();
            title = Regex.Replace(title, " +", " ");

            if (title.Length >= TITLE3_MAX_LENGTH)
            {
                
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {            
            var keyPhrase = string.Empty;

            //dde et1200 40
            //et1200 40 -dde
            //культиватор dde et1200
            //dde et1200 -40 -культиватор

            var replacedModel = Product.Model.Replace("-", " ");

            if (lineNumber == 1)
            {

                keyPhrase = $"{Manufacturer} {replacedModel}";
            }
            else if (lineNumber == 2)
            {

                keyPhrase = $"{replacedModel} -{Manufacturer}";
            }
            else if (lineNumber == 3)
            {

                keyPhrase = $"{Product.ProductTypeShort} {Manufacturer} {replacedModel}";
            }
            else if (lineNumber == 4)
            {

                var prefix = Product.ProductTypeShort.Split().Length == 1;
                keyPhrase = $"{Manufacturer} {replacedModel} {(prefix ? "-" : string.Empty)}{Product.ProductTypeShort}";
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            keyPhrase = keyPhrase.Replace("/", " ");
            keyPhrase = keyPhrase.Replace(",", ".");
            keyPhrase = keyPhrase.Replace("(", string.Empty);
            keyPhrase = keyPhrase.Replace(")", string.Empty);
            keyPhrase = keyPhrase.Replace("\"", string.Empty);
            keyPhrase = keyPhrase.Replace("'", string.Empty);


            if(keyPhrase.Split().Length >= 7)
            {

            }

            return keyPhrase.ToLower();
        }

        protected override string GetViewedUrl()
        {
            return $"{Manufacturer}";
        }
    }
}
