using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace YandexMarketFileGenerator
{
    public class YandexGenerator
    {
        readonly List<YandexMarketSection> sections;
        readonly IYandexDirectTemplate template;

        public YandexGenerator(IYandexDirectTemplate template)
        {
            sections = new List<YandexMarketSection>();
            this.template = template;
            if(template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }
        }

        public string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber)
        {
            return template.BuildExportInformation(productsInfo, startGroupSectionNumber);
        }

        public string CreateSection(OpenCartProductLine productInfo, int groupIndex)
        {
            return template.CreateSection(productInfo, groupIndex, 3);
        }

        public static List<TemplateData> GetAvailabledTemplateList()
        {
            var list = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => p.IsClass && typeof(IYandexDirectTemplate).IsAssignableFrom(p))
                    .Select(cl => new
                    {
                        Type = cl,
                        Description = ((DescriptionAttribute)Attribute.GetCustomAttribute(cl, typeof(DescriptionAttribute))).Description.Replace("Шаблон для ", String.Empty)
                    })
                    .OrderBy(td => td.Description)
                    .Select(td => new TemplateData(td.Type, td.Description))
                    .ToList();

            return list;
        }

        public class TemplateData
        {
            public Type Type { get; }
            public string Description { get; }

            public TemplateData(Type type, string description)
            {
                Type = type;
                Description = description;
            }
        }
    }

}
