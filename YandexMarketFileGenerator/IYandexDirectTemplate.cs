using System;
using System.Collections.Generic;

namespace YandexMarketFileGenerator
{
    public interface IYandexDirectTemplate
    {
        string Host { get; }
        string Manufacturer { get; }

        string BuildExportInformation(IEnumerable<OpenCartProductLine> productsInfo, int startGroupSectionNumber);
        string CreateSection(OpenCartProductLine productInfo, int groupIndex, int linesCount);

        Dictionary<string, string> ColumnStaticValues { get; }
    }
}