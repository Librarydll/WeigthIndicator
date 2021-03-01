using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Core.Excel
{
    public static class ExcelHelper
    {
        public static string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Excel files");
        static ExcelHelper()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        }

        public static void CreateDirectory()
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }
        public static void ExportExcel(IEnumerable<Reestr> reestrs, string fileName)
        {
            CreateDirectory();
            fileName = Path.Combine(_path, fileName);
            using (var file = File.Create(fileName))
            {
                using (ExcelPackage excelPackage = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Реестр");
                    worksheet.WriteHeaderNames();
                    worksheet.Row(1).Style.Font.Size = 14;
                    worksheet.Row(1).Style.Font.Bold = true;

                    int row = 2;
                    foreach (var reestr in reestrs)
                    {
                        worksheet.WriteSingleRow(reestr, row);
                        worksheet.StyleSingleRow(row);
                        row++;
                    }

                    excelPackage.Save();
                }
            }

        }

        public static void WriteSingleRow(this ExcelWorksheet worksheet, Reestr reestr, int row)
        {
            worksheet.Cells[row, 1].Value = reestr.Recipe.LongNameRu;
            worksheet.Cells[row, 2].Value = reestr.BarrelNumber;
            worksheet.Cells[row, 3].Value = reestr.BatchNumber;
            worksheet.Cells[row, 4].Style.Numberformat.Format = "dd.MM.yyyy";
            worksheet.Cells[row, 4].Value = reestr.BarrelStorage.ProductionDate;
            worksheet.Cells[row, 5].Value = reestr.Customer.ShortName;
            worksheet.Cells[row, 6].Value = reestr.Net;
            worksheet.Cells[row, 7].Value = reestr.TareBarrelWithLid + reestr.Net;
            worksheet.Cells[row, 8].Value = reestr.Note;

            if (!reestr.ReestrState)
            {
                worksheet.Row(row).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Row(row).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
            }

        }

        public static void StyleSingleRow(this ExcelWorksheet worksheet, int row)
        {
            worksheet.Row(row).Style.Font.Size = 14;
            worksheet.Column(1).AutoFit();
            worksheet.Column(4).AutoFit();

        }

        public static void WriteHeaderNames(this ExcelWorksheet worksheet)
        {
            worksheet.Cells["A1"].Value = "Наименование продукта";
            worksheet.Cells["B1"].Value = "Номер бочки";
            worksheet.Cells["C1"].Value = "Номер партии";
            worksheet.Cells["D1"].Value = "Дата производства";
            worksheet.Cells["E1"].Value = "Покупатель";
            worksheet.Cells["F1"].Value = "Нетто";
            worksheet.Cells["G1"].Value = "Брутто";
            worksheet.Cells["H1"].Value = "Примечание";
        }
    }
}
