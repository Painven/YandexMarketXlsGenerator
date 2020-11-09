using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YandexMarketFileGenerator
{
    public class YandexMarketSection : List<YandexMarketSectionLineBase>
    {
        public OpenCartProductLine ProductLine { get; }
        public int GroupIndex { get; }
        public IYandexDirectTemplate ParentTemplate { get; }
        Type SectionType { get; }
        public string Manufacturer => ParentTemplate.Manufacturer;
        public string Host => ParentTemplate.Host;
        public string TranslatedManufacturer => ParentTemplate.TranslatedManufacturer;

        public YandexMarketSection(IYandexDirectTemplate parentTemplate, Type lineType, OpenCartProductLine productLine, int groupIndex)
        {
            ProductLine = productLine;
            GroupIndex = groupIndex;
            ParentTemplate = parentTemplate;
            SectionType = lineType;
        }

        public string BuildSection(int count)
        {
            AddRange(Enumerable.Repeat((YandexMarketSectionLineBase)Activator.CreateInstance(SectionType, this), count));

            var list = new List<string>();

            int i = 1;
            foreach (var line in this)
            {
                var data = line.BuildLine(i++);

                list.Add(data);
            }

            var distinctList = list.GroupBy(line => line).Select(g => g.Key).ToList();

            return string.Join(string.Empty, distinctList.Select(line => line + Environment.NewLine));
        }

    }
}
