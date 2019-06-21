using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFWriterTest
{
    class WindowManager
    {
        private const int DOCUMENTHEIGHT = 842;
        List<ProductWindow> productWindows = new List<ProductWindow>();

        public void Add(ProductWindow window)
        {
            productWindows.Add(window);
        }

        public void Draw(PdfContentByte cb, Document doc)
        {
            int currentlyDrawingYPosition = DOCUMENTHEIGHT;
            for(int i = 0; i < productWindows.Count; i++)
            {
                ProductWindow prodWindow = productWindows[i];
                currentlyDrawingYPosition -= prodWindow.GetHeight();
                if(currentlyDrawingYPosition <= 0)
                {
                    doc.NewPage();
                    currentlyDrawingYPosition = DOCUMENTHEIGHT - prodWindow.GetHeight();
                }
                prodWindow.Draw(cb, currentlyDrawingYPosition);
            }
        }
    }

    class ProductWindow
    {
        private const int TOPBARHEIGHT = 40;
        private const int MINIMUMHEIGHT = 200 + TOPBARHEIGHT;
        private const int WINDOWWIDTH = 595;
        private const int MARGIN = 15;
        private const int SUMMARYHEIGHT = 50;

        private string summaryText;
        private string headerText;
        private List<string[]> specifications = new List<string[]>();

        public ProductWindow(string name)
        {
            headerText = name;
        }

        public void AddSpecification(string key, string value)
        {
            specifications.Add(new string[] { key, value});
        }

        public void AddSummary(string summary)
        {
            summaryText = summary;
        }

        public int GetHeight()
        {
            return CalculateHeight();
        }

        private int CalculateSpecificationElementHeight()
        {
            int height = 0;

            //Space between top and first letters
            height += 5;
            //Space for element header
            height += 25;
            //Space for each element
            height += specifications.Count * 25;
            //Space for room between text and bottom
            //height += 5;

            return height;
        }

        private int CalculateHeight()
        {
            int currentHeight = TOPBARHEIGHT;
            if(!String.IsNullOrEmpty(summaryText))
            {
                currentHeight += MARGIN + SUMMARYHEIGHT;
            }

            if(specifications.Count > 0)
            {
                currentHeight += MARGIN;
                currentHeight += CalculateSpecificationElementHeight();
                currentHeight += MARGIN;
            }
            return currentHeight;
        }

        public void Draw(PdfContentByte cb, int y)
        {
            int yPosition = y;
            int currentHeight = CalculateHeight();
            int HEIGHT = currentHeight >= MINIMUMHEIGHT ? currentHeight : MINIMUMHEIGHT;
            int TOP = yPosition + HEIGHT;
            int BOTTOM = yPosition;

            int SPECIFICATIONHEIGHT = CalculateSpecificationElementHeight();


            BaseFont f_cb = BaseFont.CreateFont("c:\\windows\\fonts\\calibrib.ttf", BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            //Background of element
            cb.SetColorFill(new BaseColor(245, 245, 245));
            cb.Rectangle(0, BOTTOM, WINDOWWIDTH, HEIGHT);
            cb.Fill();

            //Top bar background
            cb.SetColorFill(new BaseColor(22, 137, 206));
            cb.Rectangle(0, TOP - TOPBARHEIGHT, WINDOWWIDTH, TOPBARHEIGHT);
            cb.Fill();

            //Top bar text
            cb.BeginText();
            cb.SetFontAndSize(f_cb, 20);
            cb.SetColorFill(BaseColor.WHITE);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, headerText, 20, TOP - 26, 0);
            cb.EndText();

            //Shadow line under top bar
            cb.SetColorStroke(new BaseColor(222, 222, 222));
            cb.SetLineWidth(2);
            cb.MoveTo(0, TOP - TOPBARHEIGHT - 1);
            cb.LineTo(WINDOWWIDTH, TOP - TOPBARHEIGHT - 1);
            cb.ClosePathFillStroke();

            //Image of product
            Image img = Image.GetInstance("mokgroen.jpg");
            img.SetAbsolutePosition(MARGIN, TOP - TOPBARHEIGHT - 200 + MARGIN);
            img.ScaleAbsolute(200 - MARGIN - MARGIN, 200 - MARGIN - MARGIN);
            cb.AddImage(img);

            //Shadow line under image
            cb.SetColorStroke(new BaseColor(222, 222, 222));
            cb.SetLineWidth(2);
            cb.MoveTo(MARGIN, TOP - TOPBARHEIGHT - 200 + MARGIN);
            cb.LineTo(200-MARGIN, TOP - TOPBARHEIGHT - 200 + MARGIN);
            cb.ClosePathFillStroke();

            //Background of summary element
            cb.SetColorFill(new BaseColor(255, 255, 255));
            cb.Rectangle(200, TOP - TOPBARHEIGHT - MARGIN - SUMMARYHEIGHT, WINDOWWIDTH - 200 - MARGIN, SUMMARYHEIGHT);
            cb.Fill();

            //Header text for summary
            cb.BeginText();
            cb.SetFontAndSize(f_cb, 12);
            cb.SetColorFill(new BaseColor(22, 137, 206));
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Description", 210, TOP - TOPBARHEIGHT - MARGIN - 15, 0);

            //Summary text inside element
            cb.SetFontAndSize(f_cb, 12);
            cb.SetColorFill(new BaseColor(120, 120, 120));
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, summaryText, 210, TOP - TOPBARHEIGHT - MARGIN - 35, 0);
            cb.EndText();

            //Shadow line under summary
            cb.SetColorStroke(new BaseColor(222, 222, 222));
            cb.SetLineWidth(2);
            cb.MoveTo(200, TOP - TOPBARHEIGHT - MARGIN - SUMMARYHEIGHT - 1);
            cb.LineTo(200 + WINDOWWIDTH - 200- MARGIN, TOP - TOPBARHEIGHT - MARGIN - SUMMARYHEIGHT - 1);
            cb.ClosePathFillStroke();

            if(specifications.Count > 0)
            {
                //Background of specifications
                cb.SetColorFill(new BaseColor(255, 255, 255));
                cb.Rectangle(200, TOP - TOPBARHEIGHT - MARGIN - SUMMARYHEIGHT - MARGIN - SPECIFICATIONHEIGHT, WINDOWWIDTH - 200 - MARGIN, SPECIFICATIONHEIGHT);
                cb.Fill();

                //Header text for specifications
                cb.BeginText();
                cb.SetFontAndSize(f_cb, 12);
                cb.SetColorFill(new BaseColor(22, 137, 206));
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Specifications", 210, TOP - TOPBARHEIGHT - MARGIN - SUMMARYHEIGHT - MARGIN - 15, 0);

                //Summary text inside specifications
                cb.SetFontAndSize(f_cb, 12);
                cb.SetColorFill(new BaseColor(120, 120, 120));
                for (int i = 0; i < specifications.Count; i++)
                {
                    string[] specValuePairs = specifications[i];

                    int textYPos = TOP - TOPBARHEIGHT - MARGIN - SUMMARYHEIGHT - MARGIN - 25 - 15 - (25 * i);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, specValuePairs[0], 210, textYPos, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, specValuePairs[1], 320, textYPos, 0);

                }
                cb.EndText();

                //Shadow line under specifications
                cb.SetColorStroke(new BaseColor(222, 222, 222));
                cb.SetLineWidth(2);
                cb.MoveTo(200, TOP - TOPBARHEIGHT - MARGIN - SUMMARYHEIGHT - MARGIN - SPECIFICATIONHEIGHT - 1);
                cb.LineTo(200 + WINDOWWIDTH - 200 - MARGIN, TOP - TOPBARHEIGHT - MARGIN - SUMMARYHEIGHT - MARGIN - SPECIFICATIONHEIGHT - 1);
                cb.ClosePathFillStroke();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            System.IO.FileStream fs = new FileStream("First PDF document.pdf", FileMode.Create);

            //Create pdf structure and content
            ProductWindow prodWindow = new ProductWindow("MOK_ALLES_GROEN");
            prodWindow.AddSpecification("Resolution", "300");
            prodWindow.AddSpecification("Width", "400");
            prodWindow.AddSpecification("Height", "200");
            prodWindow.AddSpecification("Glans", "Inderdaad");
            prodWindow.AddSummary("Green mug for a nice cuppa tea");

            ProductWindow prodWindow1 = new ProductWindow("WOOP");
            prodWindow1.AddSpecification("Resolution", "300");
            prodWindow1.AddSpecification("Width", "400");
            prodWindow1.AddSpecification("Height", "200");
            prodWindow1.AddSpecification("Glans", "Inderdaad");
            prodWindow1.AddSpecification("Width", "400");
            prodWindow1.AddSpecification("Height", "200");
            prodWindow1.AddSpecification("Glans", "Inderdaad");
            prodWindow1.AddSummary("Green mug for a nice cuppa tea");

            ProductWindow prodWindow2 = new ProductWindow("GELE_BANAAN");
            prodWindow2.AddSpecification("Resolution", "300");
            prodWindow2.AddSpecification("Width", "400");
            prodWindow2.AddSpecification("Height", "200");
            prodWindow2.AddSpecification("Glans", "Inderdaad");
            prodWindow2.AddSummary("Green mug for a nice cuppa tea");

            ProductWindow prodWindow3 = new ProductWindow("MOK_ALLES_GROEN");
            prodWindow3.AddSpecification("Resolution", "300");
            prodWindow3.AddSpecification("Width", "400");
            prodWindow3.AddSpecification("Height", "200");
            prodWindow3.AddSpecification("Glans", "Inderdaad");
            prodWindow3.AddSummary("Green mug for a nice cuppa tea");

            //Window manager manages the layout of all the products
            WindowManager pageManager = new WindowManager();
            pageManager.Add(prodWindow);
            pageManager.Add(prodWindow1);
            pageManager.Add(prodWindow2);
            pageManager.Add(prodWindow3);

            Document document = new Document(PageSize.A4, 0, 0, 0, 0);
            PdfWriter writer = PdfWriter.GetInstance(document, fs); 
            document.Open();

            PdfContentByte cb = writer.DirectContent;
            
            //Purple background for layout testing
            //cb.SetColorFill(new BaseColor(255, 0, 255));
            //cb.Rectangle(0, 0, 595, 842);
            //cb.Fill();

            pageManager.Draw(cb, document);

            //Draw debug info
            //BaseFont f_cb = BaseFont.CreateFont("c:\\windows\\fonts\\calibrib.ttf", BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            //cb.SetFontAndSize(f_cb, 12);
            //DrawPDFDebugInfo(cb);

            document.Close();
            // Close the writer instance  
            writer.Close();
            // Always close open filehandles explicity  
            fs.Close();
        }

        private static void DrawPDFDebugInfo(PdfContentByte cb)
        {
            int row = 1;
            for (int y = 0; y != 70; y++)
            {
                cb.SetTextMatrix(10, row);
                cb.ShowText("Y: " + row.ToString());
                row += 12; // The spacing between the rows is set to 12 "points"  
            }
            int col = 35;
            for (int x = 0; x != 22; x++)
            {
                cb.SetTextMatrix(col, 829);
                cb.ShowText("X: " + col.ToString());
                col += 25; // The spacing between the columns is set to 25 "points"  
            }
        }
    }
}
