using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System.Collections.Generic;
using System.Diagnostics;

namespace OrariDipendenti
{
    internal static class pdf_all
    {
        public static Document generaPdf(List<Report> tabella, string mese)
        {
            Document document = new Document();

            MigraDoc.DocumentObjectModel.Section section = document.AddSection();
            Image image = section.AddImage("images/sfondo-pdf2.jpg");
            image.Height = 80;
            image.Width = 600;

            DefineStyles(document);
            //DefineTables(document);

            document.LastSection.AddParagraph("Report tutti i dipendenti " + mese, "Heading1");

            Table table = new Table();
            table.Borders.Width = 0.75;
            table.Format.Font = new Font("Verdana");

            Column column = table.AddColumn(Unit.FromCentimeter(2.2));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(2));
            table.AddColumn(Unit.FromCentimeter(1.6));
            table.AddColumn(Unit.FromCentimeter(1.6));
            table.AddColumn(Unit.FromCentimeter(1.6));
            table.AddColumn(Unit.FromCentimeter(1.6));
            table.AddColumn(Unit.FromCentimeter(1.6));
            table.AddColumn(Unit.FromCentimeter(1.6));
            table.AddColumn(Unit.FromCentimeter(4.5));
            table.AddColumn(Unit.FromCentimeter(1.7));

            Row row = table.AddRow();
            row.Shading.Color = Colors.PaleGoldenrod;
            Cell cell = row.Cells[0];
            cell.AddParagraph("Nome");
            cell = row.Cells[1];
            cell.AddParagraph("Giorno");
            cell = row.Cells[2];
            cell.AddParagraph("Orario");
            cell = row.Cells[3];
            cell.AddParagraph("Entrata");
            cell = row.Cells[4];
            cell.AddParagraph("Uscita");
            cell = row.Cells[5];
            cell.AddParagraph("Pausa");
            cell = row.Cells[6];
            cell.AddParagraph("Ore a scuola");
            cell = row.Cells[7];
            cell.AddParagraph("Ore Lavorate");
            cell = row.Cells[8];
            cell.AddParagraph("Note");
            cell = row.Cells[9];
            cell.AddParagraph("Banca Ore");

            string heading = "";
            foreach (var riga in tabella)
            {
                if (riga.report_nome == "---")
                {
                    heading = "Heading2";
                }
                else
                {
                    heading = "Normal";
                }
                Debug.WriteLine("Amount " + riga.report_entrata + " " + riga.report_nome + " " + riga.report_bancaore);
                row = table.AddRow();
                row.VerticalAlignment = VerticalAlignment.Center;
                row.Style = heading;
                cell = row.Cells[0];
                cell.AddParagraph(riga.report_nome);
                cell = row.Cells[1];
                cell.AddParagraph(riga.report_giorno_dayofweek);
                cell = row.Cells[2];
                cell.AddParagraph(riga.report_orario);
                cell = row.Cells[3];
                cell.AddParagraph(riga.report_entrata);
                cell = row.Cells[4];
                cell.AddParagraph(riga.report_uscita);
                cell = row.Cells[5];
                cell.AddParagraph(riga.report_pausa);
                cell = row.Cells[6];
                cell.AddParagraph(riga.report_ore_dentro);
                cell = row.Cells[7];
                cell.AddParagraph(riga.report_ore_lavorate);
                cell = row.Cells[8];
                cell.AddParagraph(riga.report_note);
                cell = row.Cells[9];
                cell.AddParagraph(riga.report_bancaore);
            }

            table.SetEdge(0, 0, 1, 1, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);

            document.DefaultPageSetup.RightMargin = 13;
            document.DefaultPageSetup.LeftMargin = 15;
            document.DefaultPageSetup.TopMargin = 10;

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false, PdfSharp.Pdf.PdfFontEmbedding.Always);
            pdfRenderer.Document = document;

            pdfRenderer.RenderDocument();
            string filename = initTable.pathPdf() + "/" + "tutti_i_dipendenti_" + mese + ".pdf";
            pdfRenderer.PdfDocument.Save(filename);
            //Process.Start(filename);
            return document;
        }

        public static void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";
            style.Font.Size = 8;

            // Heading1 to Heading9 are predefined styles with an outline level. An outline level
            // other than OutlineLevel.BodyText automatically creates the outline (or bookmarks)
            // in PDF.

            style = document.Styles["Heading1"];
            style.Font.Name = "Verdana";
            style.Font.Size = 18;
            style.Font.Bold = true;
            style.Font.Color = Colors.Black;
            //style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceAfter = 4;

            style = document.Styles["Heading2"];
            style.Font.Name = "Verdana";
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 4;
            style.ParagraphFormat.SpaceAfter = 2;

            style = document.Styles["Heading3"];
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called TextBox based on style Normal
            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Create a new style called TOC based on style Normal
            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.Blue;
        }

        public static void DefineTables(Document document)
        {
            Paragraph paragraph = document.LastSection.AddParagraph("Table Overview", "Heading1");
            paragraph.AddBookmark("Tables");

            // DemonstrateSimpleTable(document);
            DemonstrateAlignment(document);
            DemonstrateCellMerge(document);
        }

        public static void DemonstrateSimpleTable(Document document)
        {
            document.LastSection.AddParagraph("Simple Tables", "Heading2");

            Table table = new Table();
            table.Borders.Width = 0.75;

            Column column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(5));

            Row row = table.AddRow();
            row.Shading.Color = Colors.PaleGoldenrod;
            Cell cell = row.Cells[0];
            cell.AddParagraph("Itemus");
            cell = row.Cells[1];
            cell.AddParagraph("Descriptum");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("1");
            cell = row.Cells[1];
            cell.AddParagraph(" ");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("2");
            cell = row.Cells[1];
            cell.AddParagraph(" ");

            table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);
        }

        public static void DemonstrateAlignment(Document document)
        {
            document.LastSection.AddParagraph("Cell Alignment", "Heading2");

            Table table = document.LastSection.AddTable();
            table.Borders.Visible = true;
            table.Format.Shading.Color = Colors.LavenderBlush;
            table.Shading.Color = Colors.Salmon;
            table.TopPadding = 5;
            table.BottomPadding = 5;

            Column column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Right;

            table.Rows.Height = 35;

            Row row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Top;
            row.Cells[0].AddParagraph("Text");
            row.Cells[1].AddParagraph("Text");
            row.Cells[2].AddParagraph("Text");

            row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Center;
            row.Cells[0].AddParagraph("Text");
            row.Cells[1].AddParagraph("Text");
            row.Cells[2].AddParagraph("Text");

            row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[0].AddParagraph("Text");
            row.Cells[1].AddParagraph("Text");
            row.Cells[2].AddParagraph("Text");
        }

        public static void DemonstrateCellMerge(Document document)
        {
            document.LastSection.AddParagraph("Cell Merge", "Heading2");

            Table table = document.LastSection.AddTable();
            table.Borders.Visible = true;
            table.TopPadding = 5;
            table.BottomPadding = 5;

            Column column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Right;

            table.Rows.Height = 35;

            Row row = table.AddRow();
            row.Cells[0].AddParagraph("Merge Right");
            row.Cells[0].MergeRight = 1;

            row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[0].MergeDown = 1;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[0].AddParagraph("Merge Down");

            table.AddRow();
        }
    }
}