using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using SimpleWPFReporting;
using WeigthIndicator.Views;

namespace WeigthIndicator.Core.Print
{
    public static class PrintHelper
    {

        public static void Prints(FlowDocument flowDocument)
        {
           
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
            flowDocument.PageHeight = printDialog.PrintableAreaHeight;
            flowDocument.PageWidth = printDialog.PrintableAreaWidth;
            var fd = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
            printDialog.PrintDocument(fd, "ss");
        }


        public static Thickness DefaultThickness = new Thickness(0, 20, 0, 0);

        public static void PrintReport(
              StackPanel reportContainer,
              object dataContext,
              ResourceDictionary resourceDictionary = null,
              Brush backgroundBrush = null,
              DataTemplate reportHeaderDataTemplate = null,
              bool headerOnlyOnTheFirstPage = false,
              DataTemplate reportFooterDataTemplate = null,
              bool footerStartsFromTheSecondPage = false)
        {
            PrintDialog printDialog = new PrintDialog();

            Size reportSize = GetReportSize(reportContainer, DefaultThickness, printDialog);
            List<FrameworkElement> ReportElements = new List<FrameworkElement>(reportContainer.Children.Cast<FrameworkElement>());
            reportContainer.Children.Clear(); //to avoid exception "Specified element is already the logical child of another element."

            List<ReportPage> ReportPages =
                GetReportPages(
                    resourceDictionary,
                    backgroundBrush,
                    ReportElements,
                    dataContext,
                    DefaultThickness,
                    reportSize,
                    reportHeaderDataTemplate,
                    headerOnlyOnTheFirstPage,
                    reportFooterDataTemplate,
                    footerStartsFromTheSecondPage);

            try
            {
                ReportPages.ForEach(reportPage => reportPage.Scale(reportSize, printDialog));
                ReportPages.ForEach((reportPage, index) => printDialog.PrintVisual(reportPage.LayoutRoot, "asd"));
            }
            finally
            {
                ReportPages.ForEach(reportPage => reportPage.ClearChildren());
                ReportElements.ForEach(elm => reportContainer.Children.Add(elm));
                reportContainer.UpdateLayout();
            }
        }

        private static List<ReportPage> GetReportPages(
           ResourceDictionary resourceDictionary,
           Brush backgroundBrush,
           List<FrameworkElement> ReportElements,
           object dataContext,
           Thickness margin,
           Size reportSize,
           DataTemplate reportHeaderDataTemplate,
           bool headerOnlyOnTheFirstPage,
           DataTemplate reportFooterDataTemplate,
           bool footerStartsFromTheSecondPage)
        {
            int pageNumber = 1;

            List<ReportPage> ReportPages =
                new List<ReportPage>
                {
                    new ReportPage(
                        reportSize,
                        backgroundBrush,
                        margin,
                        dataContext,
                        resourceDictionary,
                        reportHeaderDataTemplate,
                        (footerStartsFromTheSecondPage) ? null : reportFooterDataTemplate,
                        pageNumber)
                };

            foreach (FrameworkElement reportVisualElement in ReportElements)
            {
                if (ReportPages.Last().GetChildrenActualHeight() + GetActualHeightPlusMargin(reportVisualElement) > reportSize.Height - margin.Top - margin.Bottom)
                {
                    pageNumber++;

                    ReportPages.Add(
                        new ReportPage(
                            reportSize,
                            backgroundBrush,
                            margin,
                            dataContext,
                            resourceDictionary,
                            (headerOnlyOnTheFirstPage) ? null : reportHeaderDataTemplate,
                            reportFooterDataTemplate,
                            pageNumber));
                }

                ReportPages.Last().AddElement(reportVisualElement);
            }

            foreach (ReportPage reportPage in ReportPages)
            {
                reportPage.LayoutRoot.Measure(reportSize);
                reportPage.LayoutRoot.Arrange(new Rect(reportSize));
                reportPage.LayoutRoot.UpdateLayout();
            }

            return ReportPages;
        }

        private static double GetActualHeightPlusMargin(FrameworkElement elm)
        {
            return elm.ActualHeight + elm.Margin.Top + elm.Margin.Bottom;
        }


        private static Size GetReportSize(StackPanel reportContainer, Thickness margin, PrintDialog printDialog = null)
        {
            if (printDialog == null)
                printDialog = new PrintDialog();

            printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;

            double reportWidth = reportContainer.ActualWidth + margin.Left + margin.Right;

            double reportHeight = (reportWidth / printDialog.PrintableAreaWidth) * printDialog.PrintableAreaHeight;

            return new Size(reportWidth, reportHeight);
        }

        private static void ForEach<T>(this IEnumerable<T> enumeration, Action<T, int> action)
        {
            if (enumeration == null)
                return;

            int index = 0;
            foreach (T item in enumeration)
            {
                action(item, index);
                index++;
            }
        }
    }
}
