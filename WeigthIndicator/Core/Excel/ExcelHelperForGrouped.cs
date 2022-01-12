using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.DTO;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Core.Excel
{
    public static class ExcelHelperForGrouped
    {
        static ExcelHelperForGrouped()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }


        public static void ExportExcel(IEnumerable<GroupedReestr> reestrs, string fileName)
        {
            ExcelHelper.CreateDirectory();
            fileName = Path.Combine(ExcelHelper._path, fileName);
            using (var file = File.Create(fileName))
            {
                using (ExcelPackage excelPackage = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Реестр");
                    worksheet.WriteHeaderNamesForGrouping();
                    worksheet.Row(1).Style.Font.Size = 14;
                    worksheet.Row(1).Style.Font.Bold = true;

                    int row = 2;
                    foreach (var reestr in reestrs)
                    {
                        worksheet.WriteSingleRowForGrouping(reestr, row);
                        worksheet.StyleSingleRowForGrouping(row);
                        row++;
                    }

                    excelPackage.Save();
                }
            }

        }

        public static void WriteSingleRowForGrouping(this ExcelWorksheet worksheet, GroupedReestr reestr, int row)
        {
            worksheet.Cells[row, 1].Value = reestr.BatchNumber;
            worksheet.Cells[row, 2].Value = reestr.RecipeName;
            worksheet.Cells[row, 3].Value = reestr.TotalNet;
            worksheet.Cells[row, 4].Value = reestr.BarrelsCount;
        }

        public static void StyleSingleRowForGrouping(this ExcelWorksheet worksheet, int row)
        {
            worksheet.Row(row).Style.Font.Size = 14;
            worksheet.Column(1).AutoFit();
            worksheet.Column(3).AutoFit();
            worksheet.Column(4).AutoFit();

        }

        public static void WriteHeaderNamesForGrouping(this ExcelWorksheet worksheet)
        {
            worksheet.Cells["A1"].Value = "Номер партии";
            worksheet.Cells["B1"].Value = "Наименование продукта";
            worksheet.Cells["C1"].Value = "Нетто всего";
            worksheet.Cells["D1"].Value = "Бочек всего";

        }
    }

}
