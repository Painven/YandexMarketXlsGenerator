using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator.Templates
{
    [Description("Шаблон для Aktakom")]
    public class AktakomYandexDirectTemplate : IYandexDirectTemplate
    {
        public string Host { get; } = "https://etk-komplekt.ru";
        public string Manufacturer { get; } = "Актаком";
        public string TranslatedManufacturer { get; } = "Актаком";
        public List<string> InvalidWords => throw new NotImplementedException();

        public Dictionary<string, string> ColumnStaticValues { get; } = new Dictionary<string, string>()
        {
            ["A"] = "-",
            ["B"] = "Текстово-графическое",
            ["C"] = "-",
            ["G"] = "-",
            ["S"] = "Россия",
            ["V"] = "-",
            ["X"] = "Работает везде",
            ["Y"] = "Актаком||Осцилографы Актаком||Мультиметры Актаком||Микроомметры Актаком",
            ["AA"] = "https://etk-komplekt.ru/aktakom||https://etk-komplekt.ru/izmeritelnye-pribory/oscillografy/oscillografy-aktakom/||https://etk-komplekt.ru/izmeritelnye-pribory/multimetry-cifrovye/multimetry-aktakom/||https://etk-komplekt.ru/izmeritelnye-pribory/izmeriteli-soprotivleniya/mikroommetry/mikroommetry-aktakom/",
            ["AJ"] = "официальный дилер||Генераторы AnaPico",
            ["AK"] = "-поверка -сертификат -скачать"
        };

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            var sb = new StringBuilder();

            foreach (var line in productsInfo)
            {
                sb.Append(CreateSection(line, startGroupSectionNumber++, 5));
            }

            return sb.ToString();
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount)
        {
            var data = new YandexMarketSection(this, typeof(AktakomYandexMarketSectionLine), productInfo, groupIndex);
            return data.BuildSection(5);
        }
    }

    internal class AktakomYandexMarketSectionLine : YandexMarketSectionLineBase
    {
        private Dictionary<string, string> MODEL_MAP = new Dictionary<string, string>()
        {
            ["ACK"] = "аск",
            ["ADG"] = "адг",
            ["ADLM-W"] = "адлм",
            ["ADS"] = "адс",
            ["ADSD"] = "адсд",
            ["ADSH"] = "адсш",
            ["ADSL"] = "адсл",
            ["ADSM"] = "адсм",
            ["ADSMV"] = "адсмв",
            ["ADST"] = "адст",
            ["ADSV"] = "адсв",
            ["AEL"] = "аел",
            ["AELL"] = "аелл",
            ["AELP"] = "аелп",
            ["AFC"] = "афс",
            ["AKC"] = "akc",
            ["AOP"] = "аоп",
            ["APC"] = "апс",
            ["APM"] = "апм",
            ["APQT"] = "апкт",
            ["APS"] = "апс",
            ["APSL"] = "апсл",
            ["APT"] = "апт",
            ["ASA"] = "аса",
            ["ASE"] = "асе",
            ["ATE"] = "ate",
            ["ATH"] = "атш",
            ["ATP"] = "атп",
            ["ATT"] = "атт",
            ["AVA"] = "ава",
            ["AVS"] = "авс",
            ["AWG"] = "авг",
            ["XDS"] = "хдс",
            ["ААЕ"] = "aae",
            ["АВМ"] = "abm",
            ["АЕЕ"] = "aee",
            ["АКС"] = "akc",
            ["АМ"] = "ama",
            ["АМB"] = "амв",
            ["АМЕ"] = "ame",
            ["АММ"] = "amm",
            ["АНА"] = "аха",
            ["АНР"] = "ахп",
            ["АНТ"] = "ахт",
            ["АОЕ"] = "aoe",
            ["АОС"] = "аок",
            ["АРМ"] = "апм",
            ["АРМБ"] = "апмб",
            ["АРМС"] = "апмс",
            ["АРР"] = "апп",
            ["АРС"] = "апс",
            ["АРТ"] = "апт",
            ["АСА"] = "aca",
            ["АСЕ"] = "ace",
            ["АСК"] = "ack",
            ["АСКB"] = "аскв",
            ["АСМ"] = "acm",
            ["АСН"] = "асш",
            ["АСТ"] = "act",
            ["АТА"] = "ata",
            ["АТЕ"] = "ате",
            ["АТЕBT"] = "атебт",
            ["АТЕВТ"] = "атебт",
            ["АТК"] = "atk",
            ["АТКB"] = "аткб",
            ["АТН"] = "атш",
            ["АТР"] = "атп",
            ["АТT"] = "атт"
        };

        public AktakomYandexMarketSectionLine(YandexMarketSection parentSection) : base(parentSection)
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
        }

        protected override string GetViewedUrl()
        {
            string url = Product.Model.Replace(" ", "-").Replace("/", "-").Replace("_", "-");
            if(url.Length + "-Актаком".Length < VIEWED_URL_MAX_LENGTH)
            {
                url += "-Актаком";
            }

            if(url.Length >= VIEWED_URL_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + url);
            }

            return url;
        }

        protected override string GetTitle1()
        {
            var title = $"{Product.Model} {Product.ProductTypeShort} Актаком";          
            if (title.Length >= TITLE1_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetTitle2()
        {

            var title = $"{Product.Model} Актаком";
            if (title.Length >= TITLE2_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }
            return title;
        }

        protected override string GetTitle3()
        {
            var title = $"{Product.Model} Актаком {Product.ProductTypeFull}";

            if((title.Length + 20) < TITLE3_MAX_LENGTH)
            {
                title += ". Доставка по Росиии";
            }

            if(title.Length >= TITLE3_MAX_LENGTH)
            {
                //throw new FormatException("Превышена допустимая длина: " + title);
            }

            return title;
        }

        protected override string GetPhrase(int lineNumber)
        {
            string phrase = null;

            //АМ-1038 -aktakom -актак
            //1038 Aktakom -ама
            //1038 актаком -ама
            //АМ-1038 Aktakom
            //АМ-1038 актаком

            var modelNumbers = Product.Model.Split('-').ToList();
            bool hasModelNumbers = modelNumbers.Count >=2 && Regex.IsMatch(modelNumbers[1], @"\d+");

            var tranlatedModel = MODEL_MAP.ContainsKey(modelNumbers[0]) ? MODEL_MAP[modelNumbers[0]] : string.Empty;
            bool hasModelPrefix = !string.IsNullOrEmpty(tranlatedModel);


            if (lineNumber == 1)
            {
                phrase = $"{Product.Model} -aktakom -актак";
            }
            else if (lineNumber == 2)
            {
                if (!hasModelNumbers || !hasModelPrefix)
                {
                    return "___INVALID";
                }

                phrase = $"{Product.Model} Aktakom -{tranlatedModel}";
            }
            else if (lineNumber == 3)
            {
                if(!hasModelNumbers || !hasModelPrefix)
                {
                    return "___INVALID";
                }
                phrase = $"{modelNumbers[1]} актаком -{tranlatedModel}";
            }
            else if (lineNumber == 4)
            {
                if (!hasModelNumbers)
                {
                    return "___INVALID";
                }

                phrase = $"{modelNumbers[1]} Aktakom";
            }
            else if (lineNumber == 5)
            {
                phrase = $"{Product.Model} актаком";
            }

            return phrase;
        }
    }
}
