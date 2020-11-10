using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Wera")]
    public class WeraYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Wera";
        public string TranslatedManufacturer { get; } = "Вера";

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
            ["Z"] = "WERA инструмент||Отвертки||Наборы отверток||Биты||Трещотки||Автоинструмент||Головки||Наборы инструмента WERA",
            ["AA"] = "Весь ассортимент||Шлицевые, регулируемые, динамометрические||||TORX, TRI-Wing, наборы и штучно||Наборы и штучно||Отвертки||Магнитныне||Профессиональные",
            ["AB"] = "https://etk-komplekt.ru/wera||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/otvertka/otvertki-wera/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabor-otvertok/nabory-otvertok-wera/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/otvertka/bity-dlya-otvertok/bity-dlya-otvertok-wera/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/instrumentalnye-klyuchi/treshhotki/treshhotki-wera/||https://etk-komplekt.ru/instrumenty/instrument-dlya-avtoservisa/avtoinstrument-wera/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/golovki/golovki-wera/||https://etk-komplekt.ru/instrumenty/ruchnoy-instrument/nabory-instrumentov/nabory-instrumentov-wera/",
            ["AK"] = "Немецкое качество||в наличии||Доставка по России||Выставочный зал в СПб"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            var source = productsInfo.ToList();

            foreach (var line in source)
            {
                int lines = 4;

                var section = CreateSection(line, startGroupSectionNumber++, lines);

                sb.Append(section);               
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(WeraYandexMarketSectionLine), productInfo, groupIndex);

            return data.BuildSection(linesCount);
        }
    }

    internal class WeraYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        public WeraYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection) { }

        protected override string GetViewedUrl() => $"{Manufacturer} {Product.Model}".ToViewedUrl();

        protected override string GetGroupName() => $"{Manufacturer} {Product.Model}";

        //Wera WE-074716 отвертка
        protected override string GetTitle1() => $"{Manufacturer} {Product.Model} {Product.ProductTypeShort}".ToTitle();

        //Wera WE-074716 7461 Kraftform
        protected override string GetTitle2() => $"{Manufacturer} {Product.Model} {Product.CustomField}".ToTitle();

        //Wera WE-074716 7461 Kraftform Регулируемая динамометрическая отвертка от дилера!
        protected override string GetTitle3() => $"{Manufacturer} {Product.Model} {Product.CustomField} {Product.ProductTypeFull} от дилера!".ToTitle();
        
        protected override string GetPhrase(int lineNumber)
        {
            var modelRangeNames = new List<string>()
                        {
                             "BiTorsion"
                            ,"BlackLaser"
                            ,"Bull"
                            ,"Cellidor"
                            ,"Classic"
                            ,"Comfort"
                            ,"Diamond"
                            ,"Drywall"
                            ,"Harpoon"
                            ,"Hybrid"
                            ,"Impaktor"
                            ,"Imperial"
                            ,"Inclusive"
                            ,"Joker"
                            ,"Koloss"
                            ,"Kompakt"
                            ,"Kraftform"
                            ,"Lasertip"
                            ,"Multicolour"
                            ,"Pozidriv"
                            ,"Racing"
                            ,"Rapidaptor"
                            ,"Security"
                            ,"Tool-Check"
                            ,"Torque"
                            ,"Universal"
                            ,"Vario"
                            ,"Wood"
                            ,"Zyklop"
                        };

            string result = null;

            //WE-074716 Kraftform
            //Wera WE-074716
            //WE-074716 отвертка
            //WE-074716 -wera -kraftform -отвертка


            var customFieldModel = Product.CustomField.ContainsAny(modelRangeNames) ? Product.CustomField.Split().Last() : string.Empty;
            var minusSource = new[] { "wera", customFieldModel, Product.ProductTypeShort }.Where(w => !string.IsNullOrWhiteSpace(w));
            var minusWordString = string.Join(" ", minusSource.Select(w => $"-{w}"));          

            if (lineNumber == 1)
            {
                result = $"{Product.Model} {customFieldModel}".ToTitle().ToKeyPhrase();
            }
            else if (lineNumber == 2)
            {
                result = $"{Manufacturer} {Product.Model}".ToKeyPhrase();
            }
            else if (lineNumber == 3)
            {
                result = $"{Product.Model} {Product.ProductTypeShort}".ToKeyPhrase();
            }
            else if (lineNumber == 4)
            {
                result = $"{Product.Model} {minusWordString}".ToKeyPhrase();
            }
            else
            {
                throw new FormatException();
            }

            return result;
        }
    }

    #region код для получения разметки модельного ряда
//    var modelRangeNames = new List<string>()
//            {
//                 "BiTorsion"
//                ,"BlackLaser"
//                ,"Bull"
//                ,"Cellidor"
//                ,"Classic"
//                ,"Comfort"
//                ,"Diamond"
//                ,"Drywall"
//                ,"Harpoon"
//                ,"Hybrid"
//                ,"Impaktor"
//                ,"Imperial"
//                ,"Inclusive"
//                ,"Joker"
//                ,"Koloss"
//                ,"Kompakt"
//                ,"Kraftform"
//                ,"Lasertip"
//                ,"Multicolour"
//                ,"Pozidriv"
//                ,"Racing"
//                ,"Rapidaptor"
//                ,"Security"
//                ,"Tool-Check"
//                ,"Torque"
//                ,"Universal"
//                ,"Vario"
//                ,"Wood"
//                ,"Zyklop"
//            };

//    var data = Clipboard.GetText().Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
//    var resultList = Enumerable.Repeat(string.Empty, data.Count).ToArray();

//    int i = 0;
//            foreach (var name in data)
//            {
//                var regex = Regex.Match(name, @"^\d{2,5}\/?\d? [a-zA-Z]+");

//                if (regex.Success)
//                {
//                    resultList[i] += regex.Value;
//                }


//var findedModel = modelRangeNames.FirstOrDefault(m => name.IndexOf(m) >= 0);
//if (findedModel != null)
//{
//    if (regex.Success)
//    {
//        resultList[i] += " ";
//    }
//    resultList[i] += findedModel;
//}

//i++;
//            }


//            var result = string.Join(Environment.NewLine, resultList);
//Clipboard.SetText(result);
    #endregion
}
