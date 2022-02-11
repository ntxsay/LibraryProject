using ClosedXML.Excel;
using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace LibraryProjectUWP.Code.Services.Excel
{
    public class ExcelServices 
    {
        readonly StorageFile _file;
        public ExcelServices()
        {

        }

        public ExcelServices(StorageFile file)
        {
            _file = file;
        }

        public async Task<IEnumerable<string>> GetExcelSheetsName(StorageFile file = null)
        {
            try
            {
                StorageFile excelFile = file ?? _file;
                using (IRandomAccessStream stream = await excelFile.OpenAsync(FileAccessMode.Read))
                {
                    using (XLWorkbook workBook = new XLWorkbook(stream.AsStream()))
                    {
                        return workBook.Worksheets.Select(s => s.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return Enumerable.Empty<string>();
            }
        }

        public async Task<DataTable> ImportExcelToDatatable(string sheetName, string range, bool isContainsHeader = true, StorageFile file = null)
        {
            try
            {
                if (sheetName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                if (range.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                StorageFile excelFile = file ?? _file;
                using (IRandomAccessStream stream = await excelFile.OpenAsync(FileAccessMode.Read))
                {
                    using (XLWorkbook workBook = new XLWorkbook(stream.AsStream()))
                    {
                        //Read the first Sheet from Excel file.
                        IXLWorksheet workSheet = workBook.Worksheet(sheetName);

                        //Create a new DataTable.
                        DataTable dt = new DataTable();

                        //Loop through the Worksheet rows.
                        bool firstRow = true;
                        foreach (var row in workSheet.Range(range).Rows())
                        {
                            
                            //Use the first row to add columns to DataTable.
                            if (firstRow)
                            {
                                if (isContainsHeader)
                                {
                                    foreach (IXLCell cell in row.Cells())
                                    {
                                        dt.Columns.Add(cell.Value.ToString());
                                    }

                                    firstRow = false;
                                    continue;
                                }
                                else
                                {
                                    for (int j = 0; j < row.CellCount(); j++)
                                    {
                                        dt.Columns.Add($"Colonne {j + 1}");
                                    }
                                }
                            }

                            //Add rows to DataTable.
                            dt.Rows.Add();

                            int i = 0;
                            int firstColumn = row.FirstCellUsed().Address.ColumnNumber;
                            int lastColumn = row.LastCellUsed().Address.ColumnNumber;
                            //var cellf = row.Cells($"{firstColumn}:{lastColumn}");
                            var cellf = row.Cells();

                            foreach (IXLCell cell in cellf)
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                i++;
                            }

                            firstRow = false;
                        }

                        return dt;
                    }
                }
                //// Open the Excel file using ClosedXML.
                //// Keep in mind the Excel file cannot be open when trying to read it
                //using (XLWorkbook workBook = new XLWorkbook(filePath))
                //{
                //    //Read the first Sheet from Excel file.
                //    IXLWorksheet workSheet = workBook.Worksheet(1);

                //    //Create a new DataTable.
                //    DataTable dt = new DataTable();

                //    //Loop through the Worksheet rows.
                //    bool firstRow = true;
                //    foreach (IXLRow row in workSheet.Rows())
                //    {
                //        //Use the first row to add columns to DataTable.
                //        if (firstRow)
                //        {
                //            foreach (IXLCell cell in row.Cells())
                //            {
                //                dt.Columns.Add(cell.Value.ToString());
                //            }
                //            firstRow = false;
                //        }
                //        else
                //        {
                //            //Add rows to DataTable.
                //            dt.Rows.Add();
                //            int i = 0;

                //            foreach (IXLCell cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                //            {
                //                dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                //                i++;
                //            }
                //        }
                //    }

                //    return dt;
                //}
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }

        public IXLWorksheet ExcelAddValue(IXLWorksheet worksheet, int row, int currentColumnTitle, int currentColumnValue, string title, object value, IXLStyle rangeStyle)
        {
            worksheet.Cell(row, currentColumnTitle).Value = title;
            worksheet.Cell(row, currentColumnValue).Value = value;
            worksheet.Range(row, currentColumnTitle, row, currentColumnValue).Style = rangeStyle;
            return worksheet;
        }

        public IXLStyle ReportGrayLine(IXLWorkbook workbook)
        {
            var itemValueRowStyle1 = workbook.Style;
            itemValueRowStyle1.Font.FontSize = 12d;
            itemValueRowStyle1.Font.Bold = false;
            itemValueRowStyle1.Font.FontColor = XLColor.Black;
            itemValueRowStyle1.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            itemValueRowStyle1.Fill.BackgroundColor = XLColor.LightGray;
            return itemValueRowStyle1;
        }

        public IXLStyle ReportWhiteLine(IXLWorkbook workbook)
        {
            var itemValueRowStyle1 = workbook.Style;
            itemValueRowStyle1.Font.FontSize = 12d;
            itemValueRowStyle1.Font.Bold = false;
            itemValueRowStyle1.Font.FontColor = XLColor.Black;
            itemValueRowStyle1.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            itemValueRowStyle1.Fill.BackgroundColor = XLColor.White;
            return itemValueRowStyle1;
        }

        public IXLStyle ReportSectionTitleStyle(IXLWorkbook workbook)
        {
            var value = workbook.Style;
            value.Font.FontSize = 18d;
            value.Font.Bold = true;
            value.Font.FontColor = XLColor.White;
            value.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            value.Fill.BackgroundColor = XLColor.DeepSkyBlue;
            return value;
        }

        public IXLStyle ReportSectionDesignationStyle(IXLWorkbook workbook)
        {
            var value = workbook.Style;
            value.Font.FontSize = 12d;
            value.Font.Bold = false;
            value.Font.FontColor = XLColor.White;
            value.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            value.Fill.BackgroundColor = XLColor.DeepSkyBlue;
            return value;
        }

        public IXLStyle ReportSectionValueStyle(IXLWorkbook workbook)
        {
            var value = workbook.Style;
            value.Font.FontSize = 12d;
            value.Font.Bold = false;
            value.Font.FontColor = XLColor.White;
            value.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            value.Fill.BackgroundColor = XLColor.DeepSkyBlue;
            return value;
        }

        //public XLWorkbook ExportToExcelAsync(LivreVM viewModel)
        //{
        //    string Title = $"Exporter vers Microsoft Excel";
        //    try
        //    {
        //        var workbook = new XLWorkbook();
        //        var worksheet = workbook.Worksheets.Add("Rapport");

        //        worksheet.ShowGridLines = false;
        //        worksheet.Column(1).Width = 45d;
        //        worksheet.Column(2).Width = 37.54d;

        //        worksheet.Cell(1, 1).Value = "Rapport de préinscription à une formation";
        //        worksheet.Range(1, 1, 1, 2).Merge().AddToNamed("Title");
        //        worksheet.Row(1).Height = 80d;
        //        var titlesStyle = workbook.Style;
        //        titlesStyle.Font.FontSize = 22d;
        //        titlesStyle.Font.Bold = true;
        //        titlesStyle.Font.FontColor = XLColor.DeepSkyBlue;
        //        titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        titlesStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //        titlesStyle.Fill.BackgroundColor = XLColor.NoColor;
        //        workbook.NamedRanges.NamedRange("Title").Ranges.Style = titlesStyle;

        //        worksheet.Cell(2, 1).Value = "Ce document a été généré automatiquement sur le site https://ateliercoupdepouce.fr sur la demande de l'utilisateur.";
        //        worksheet.Range(2, 1, 2, 2).Merge().AddToNamed("Info1");
        //        worksheet.Row(2).Height = 40d;

        //        var info1Style = workbook.Style;
        //        info1Style.Font.FontSize = 12d;
        //        info1Style.Font.Bold = false;
        //        info1Style.Font.FontColor = XLColor.Black;
        //        info1Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //        info1Style.Alignment.WrapText = true;
        //        workbook.NamedRanges.NamedRange("Info1").Ranges.Style = info1Style;

        //        worksheet.Cell(3, 1).Value = "Données du serveur";
        //        worksheet.Range(3, 1, 3, 2).Merge().Style = this.ReportSectionTitleStyle(workbook);
        //        worksheet.Cell(4, 1).Value = "Libellé";
        //        worksheet.Range(4, 1, 4, 1).Style = this.ReportSectionDesignationStyle(workbook);
        //        worksheet.Cell(4, 2).Value = "Valeur";
        //        worksheet.Range(4, 2, 4, 2).Style = this.ReportSectionValueStyle(workbook);

        //        var currentColumnTitle = 1;
        //        var currentColumnValue = 2;

        //        for (var i = 5; i < 8; i++)
        //        {
        //            if (viewModel == null) break;

        //            var row = i;

        //            if (row == 5)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue, "Id",
        //                    viewModel.Id,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (row == 6)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Numéro de candidature", viewModel.CandidatureNumber,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else if (row == 7)
        //            {
        //                var converterValue = viewModel.DateCreation.ToString(DateHelpers.DateStringFormat
        //                    .FrenchFullDateTimeStringFormat);

        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Date de création", converterValue,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }

        //        //Civilité
        //        const int startCivilityRow = 12;
        //        for (var i = 0; i < 12; i++)
        //        {
        //            if (viewModel?.Civility == null) break;

        //            var row = startCivilityRow + i;

        //            if (i == 0)
        //            {
        //                worksheet.Cell(row, currentColumnTitle).Value = "Civilité";
        //                worksheet.Range(row, currentColumnTitle, row, currentColumnValue).Merge().Style =
        //                    this.ReportSectionTitleStyle(workbook);
        //            }
        //            else if (i == 1)
        //            {
        //                worksheet.Cell(row, currentColumnTitle).Value = "Désignation";
        //                worksheet.Range(row, currentColumnTitle, row, currentColumnTitle).Style =
        //                    this.ReportSectionDesignationStyle(workbook);
        //                worksheet.Cell(row, currentColumnValue).Value = "Valeur";
        //                worksheet.Range(row, currentColumnValue, row, currentColumnValue).Style =
        //                    this.ReportSectionValueStyle(workbook);
        //            }
        //            else if (i == 2)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Titre de civilité", viewModel.Civility.TitreCivilite,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 3)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Nom de naissance", viewModel.Civility.NomNaissance,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else if (i == 4)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Prénom", viewModel.Civility.Prenom,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 5)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Autre(s) prénom(s)", viewModel.Civility.AutresPrenoms,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else if (i == 6)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Nom d'usage", viewModel.Civility.NomUsage,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 7)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Date de naissance", viewModel.Civility.DateNaissance.ToString("dd/MM/yyyy"),
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else if (i == 8)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Lieu de naissance", viewModel.Civility.LieuNaissance,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 9)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Nationalité", viewModel.Civility.Nationalite,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else if (i == 10)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Situation familiale", viewModel.Civility.SituationFamiliale,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 11)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Nombre d'enfant à charge", viewModel.Civility.NbreEnfantsCharge,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }

        //        //Adress
        //        const int startAdressRow = 24;
        //        for (var i = 0; i < 8; i++)
        //        {
        //            if (viewModel?.Adresse == null) break;

        //            var row = startAdressRow + i;

        //            if (i == 0)
        //            {
        //                worksheet.Cell(row, currentColumnTitle).Value = "Adresse";
        //                worksheet.Range(row, currentColumnTitle, row, currentColumnValue).Merge().Style =
        //                    this.ReportSectionTitleStyle(workbook);
        //            }
        //            else if (i == 1)
        //            {
        //                worksheet.Cell(row, currentColumnTitle).Value = "Libellé";
        //                worksheet.Range(row, currentColumnTitle, row, currentColumnTitle).Style =
        //                    this.ReportSectionDesignationStyle(workbook);
        //                worksheet.Cell(row, currentColumnValue).Value = "Valeur";
        //                worksheet.Range(row, currentColumnValue, row, currentColumnValue).Style =
        //                    this.ReportSectionValueStyle(workbook);
        //            }
        //            else if (i == 2)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Adresse", viewModel.Adresse.Adresse,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 3)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Code postal", viewModel.Adresse.CodePostal,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else if (i == 4)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Ville", viewModel.Adresse.Ville,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 5)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "N° Portable", viewModel.Adresse.NoPortable,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else if (i == 6)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "N° Fixe", viewModel.Adresse.NoTelephone,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 7)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Adresse mail", viewModel.Adresse.MailAdress,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }

        //        //Situation administrative
        //        const int startSituationARow = 32;
        //        for (var i = 0; i < 8; i++)
        //        {
        //            if (viewModel?.SituationA == null) break;

        //            var row = startSituationARow + i;

        //            if (i == 0)
        //            {
        //                worksheet.Cell(row, currentColumnTitle).Value = "Situation administrative";
        //                worksheet.Range(row, currentColumnTitle, row, currentColumnValue).Merge().Style =
        //                    this.ReportSectionTitleStyle(workbook);
        //            }
        //            else if (i == 1)
        //            {
        //                worksheet.Cell(row, currentColumnTitle).Value = "Libellé";
        //                worksheet.Range(row, currentColumnTitle, row, currentColumnTitle).Style =
        //                    this.ReportSectionDesignationStyle(workbook);
        //                worksheet.Cell(row, currentColumnValue).Value = "Valeur";
        //                worksheet.Range(row, currentColumnValue, row, currentColumnValue).Style =
        //                    this.ReportSectionValueStyle(workbook);
        //            }
        //            else if (i == 2)
        //            {
        //                var converterValue = Convert.ToBoolean(viewModel.SituationA.IsDemandeurEmploi) ? "Oui" : "Non";

        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Êtes-vous demandeur d'emploi", converterValue,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 3)
        //            {
        //                var converterValue = Convert.ToBoolean(viewModel.SituationA.IsBeneficiaireAre) ? "Oui" : "Non";

        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Êtes-vous bénéficiaire de l'ARE", converterValue,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else if (i == 4)
        //            {
        //                var converterValue = Convert.ToBoolean(viewModel.SituationA.IsBeneficiaireRsa) ? "Oui" : "Non";

        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Êtes-vous bénéficiaire du RSA", converterValue,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 5)
        //            {
        //                var converterValue = Convert.ToBoolean(viewModel.SituationA.IsSalarie) ? "Oui" : "Non";

        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Êtes-vous salarié", converterValue,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else if (i == 6)
        //            {
        //                var converterValue = Convert.ToBoolean(viewModel.SituationA.IsAutre) ? "Oui" : "Non";

        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Êtes-vous dans une autre situation", converterValue,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 7)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Précisez (si autre situation)", "",
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }

        //        //Parcours de formation
        //        const int startParcoursRow = 40;
        //        for (var i = 0; i < 8; i++)
        //        {
        //            if (viewModel?.Parcours == null) break;

        //            var row = startParcoursRow + i;

        //            if (i == 0)
        //            {
        //                worksheet.Cell(row, currentColumnTitle).Value = "Parcours de formation";
        //                worksheet.Range(row, currentColumnTitle, row, currentColumnValue).Merge().Style =
        //                    this.ReportSectionTitleStyle(workbook);
        //            }
        //            else if (i == 1)
        //            {
        //                worksheet.Cell(row, currentColumnTitle).Value = "Libellé";
        //                worksheet.Range(row, currentColumnTitle, row, currentColumnTitle).Style =
        //                    this.ReportSectionDesignationStyle(workbook);
        //                worksheet.Cell(row, currentColumnValue).Value = "Valeur";
        //                worksheet.Range(row, currentColumnValue, row, currentColumnValue).Style =
        //                    this.ReportSectionValueStyle(workbook);
        //            }
        //            else if (i == 2)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Niveau d'étude", viewModel.Parcours.NiveauEtude,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 3)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Dernière classe fréquentée", viewModel.Parcours.DerniereClassFrequente,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else if (i == 4)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Diplôme préparé", viewModel.Parcours.DiplomePreparer,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 5)
        //            {
        //                var converterValue = StringHelpers.JoinStringArray(
        //                    viewModel.Parcours.DiplomesObtenus?.ToArray() ?? Array.Empty<string>(), " ; ", out _);

        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Diplôme(s) obtenu(s)", converterValue,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else if (i == 6)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Expérience(s) professionnelle(s) ou stage(s) réalisé(s) dans le cadre des services à la personne (salarié ou bénévole)",
        //                    viewModel.Parcours.XprienceServPersonne,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 7)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Autre(s) expérience(s) professionnelle(s) (salarié ou bénévole)",
        //                    viewModel.Parcours.Xprience,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }

        //        //Choix/Raison de la formation
        //        const int startChoixFormationRow = 48;
        //        for (var i = 0; i < 5; i++)
        //        {
        //            if (viewModel?.Formation == null) break;

        //            var row = startChoixFormationRow + i;

        //            if (i == 0)
        //            {
        //                worksheet.Cell(row, currentColumnTitle).Value = "Choix de la formation";
        //                worksheet.Range(row, currentColumnTitle, row, currentColumnValue).Merge().Style = this.ReportSectionTitleStyle(workbook);
        //            }
        //            else if (i == 1)
        //            {
        //                worksheet.Cell(row, currentColumnTitle).Value = "Libellé";
        //                worksheet.Range(row, currentColumnTitle, row, currentColumnTitle).Style = this.ReportSectionDesignationStyle(workbook);
        //                worksheet.Cell(row, currentColumnValue).Value = "Valeur";
        //                worksheet.Range(row, currentColumnValue, row, currentColumnValue).Style = this.ReportSectionValueStyle(workbook);
        //            }
        //            else if (i == 2)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Intitulé de la formation", viewModel.Formation.FormationName,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else if (i == 3)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Type de cursus", viewModel.Formation.TypeCursus,
        //                    this.ReportGrayLine(workbook));
        //            }
        //            else if (i == 4)
        //            {
        //                this.ExcelAddValue(worksheet, row, currentColumnTitle, currentColumnValue,
        //                    "Les raisons qui vous conduisent à préparer ce diplôme", viewModel.Formation.RaisonsFormation,
        //                    this.ReportWhiteLine(workbook));
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }

        //        //Fin
        //        const int startFinRow = 53;
        //        worksheet.Cell(startFinRow, currentColumnTitle).Value = "Fin";
        //        worksheet.Range(startFinRow, currentColumnTitle, startFinRow, currentColumnValue).Merge().Style = this.ReportSectionTitleStyle(workbook);

        //        return workbook;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        return null;
        //    }
        //}

    }
}
