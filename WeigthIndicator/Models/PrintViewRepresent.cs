using System.Collections.Generic;
using WeigthIndicator.Factory;

namespace WeigthIndicator.Models
{
    public class PrintViewRepresent
    {
        public PrintViewRepresent(PrintViewType printViewType, string title)
        {
            PrintViewType = printViewType;
            Title = title;
        }

        public PrintViewType PrintViewType { get; set; }
        public string Title { get; set; }

        public static IEnumerable<PrintViewRepresent> GetPrintViewRepresents()
        {
            yield return new PrintViewRepresent(PrintViewType.WithAddress, "С адрессом");
            yield return new PrintViewRepresent(PrintViewType.WithNutritionValue, " С пищ ценностью");
            yield return new PrintViewRepresent(PrintViewType.PepsiCo, "Pepsi co");
            yield return new PrintViewRepresent(PrintViewType.NoPrint, "Не печатать");
        }
    }
}
