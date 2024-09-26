﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using WpfApp1.model;

namespace WpfApp1.View
{
    public partial class Interpretation : Window
    {
        public Interpretation()
        {
            InitializeComponent();
        }

        public Interpretation(List<IStarLibrary?> library) : this()
        {
            Write(library);
        }

        private void WriteStarInSign(ref Paragraph paragraph, LibrarySigns library)
        {
            paragraph.Inlines.Add(Img(library.Star.URI, library.Star.ToString()));
            paragraph.Inlines.Add(" in ");
            paragraph.Inlines.Add(Img(library.Sign.URI, library.Sign.ToString()));
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(library.Description);
        }

        private void WriteAspectBetweenStars(ref Paragraph paragraph, LibraryAspects library) 
        {
            paragraph.Inlines.Add(Img(library.Star.URI, library.Star.ToString()));
            paragraph.Inlines.Add(" ");
            paragraph.Inlines.Add(Img(library.Aspect.URI, library.Aspect.ToString()));
            paragraph.Inlines.Add(" ");
            paragraph.Inlines.Add(Img(library.Star2.URI, library.Star.ToString()));

            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(library.Energy.EnergyId == 1 ? "Positive" : "Negative");
            paragraph.Inlines.Add(":");
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(library.Description);
        }

        private void WriteStarInHouse(ref Paragraph paragraph, LibraryHouses library)
        {
            paragraph.Inlines.Add(Img(library.Star.URI, library.Star.ToString()));

            if (library.Aspect != null) 
            {
                paragraph.Inlines.Add(" ");
                paragraph.Inlines.Add(Img(library.Aspect.URI, library.Aspect.ToString()));
                paragraph.Inlines.Add(" ");
            }
            else 
            {
                paragraph.Inlines.Add(" in ");
            }

            paragraph.Inlines.Add(library.House.ToString());
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(library.Description);
        }

        private InlineUIContainer Img(string uri, string toolTip = "") 
        {
            Image image = new()
            {
                Source = new BitmapImage(new Uri(uri)),
                Width = 20, // Set image size
                Height = 20,
                VerticalAlignment = VerticalAlignment.Bottom,
                ToolTip = toolTip
            };
            return new InlineUIContainer(image);
        }

        private void Write(List<IStarLibrary?> library) 
        {
            FlowDocument flowDoc = new FlowDocument();
            Paragraph paragraph = new Paragraph();

            foreach (IStarLibrary? lib in library) 
            {
                lib?.Build();

                if (lib is LibraryAspects libAsp) 
                {
                    WriteAspectBetweenStars(ref paragraph, libAsp);
                    paragraph.Inlines.Add(new LineBreak());
                    paragraph.Inlines.Add(new LineBreak());
                }

                if (lib is LibrarySigns libSigns)
                {
                    WriteStarInSign(ref paragraph, libSigns);
                    paragraph.Inlines.Add(new LineBreak());
                    paragraph.Inlines.Add(new LineBreak());
                }

                if (lib is LibraryHouses libHouse) 
                {
                    WriteStarInHouse(ref paragraph, libHouse);
                    paragraph.Inlines.Add(new LineBreak());
                    paragraph.Inlines.Add(new LineBreak());
                }

            }
            
            flowDoc.Blocks.Add(paragraph);
            documentViewer.Document = flowDoc;
        }
    }
}
