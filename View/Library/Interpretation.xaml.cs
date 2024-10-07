using System.Windows;
using System.Windows.Documents;
using AstroNET.model;

namespace AstroNET.View
{
    public partial class Interpretation : Window
    {
        private int skyTypeId = 1;
        private object _obj = null!;
        public Interpretation()
        {
            InitializeComponent();
        }

        public Interpretation(IEnumerable<IAbstractPointLibrary?> library) : this()
        {
            Write(library);
        }

        public Interpretation(IEnumerable<IAbstractPointLibrary?> library, object aspect) : this(library)
        {
            _obj = aspect;
            Write(library);
        }

        public Interpretation(IEnumerable<IAbstractPointLibrary?> library, object aspect, int transitType) : this(library, aspect)
        {
            this.skyTypeId = transitType;
        }

        private static void WriteHouseInSign(ref Paragraph paragraph, LibraryHouseSigns library)
        {
            paragraph.Inlines.Add($"{library.House.PointName} in ");
            paragraph.Inlines.Add(Utils.InLineImage(library.Sign.URI, library.Sign.ToString()));
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(library.Description);
        }

        private static void WriteStarInSign(ref Paragraph paragraph, LibraryStarSigns library)
        {
            paragraph.Inlines.Add(Utils.InLineImage(library.Star.URI, library.Star.ToString()));
            paragraph.Inlines.Add(" in ");
            paragraph.Inlines.Add(Utils.InLineImage(library.Sign.URI, library.Sign.ToString()));
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(library.Description);
        }

        private static void WriteAspectBetweenStars(ref Paragraph paragraph, LibraryStarAspects library)
        {
            paragraph.Inlines.Add(Utils.InLineImage(library.Star.URI, library.Star.ToString()));
            paragraph.Inlines.Add(" ");
            paragraph.Inlines.Add(Utils.InLineImage(library.Aspect.URI, library.Aspect.ToString()));
            paragraph.Inlines.Add(" ");
            paragraph.Inlines.Add(Utils.InLineImage(library.Star2.URI, library.Star.ToString()));

            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(library.Energy.EnergyId == 1 ? "Positive:" : "Negative:");
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(library.Description);
        }

        private static void WriteStarInHouse(ref Paragraph paragraph, LibraryStarHouses library)
        {
            paragraph.Inlines.Add(Utils.InLineImage(library.Star.URI, library.Star.ToString()));

            if (library.Aspect != null)
            {
                paragraph.Inlines.Add(" ");
                paragraph.Inlines.Add(Utils.InLineImage(library.Aspect.URI, library.Aspect.ToString()));
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

        private void Write(IEnumerable<IAbstractPointLibrary?> library)
        {
            FlowDocument flowDoc = new();
            Paragraph paragraph = new();

            foreach (IAbstractPointLibrary? lib in library)
            {
                lib?.Build();

                if (lib is LibraryStarAspects libAsp)
                {
                    WriteAspectBetweenStars(ref paragraph, libAsp);
                    paragraph.Inlines.Add(new LineBreak());
                    paragraph.Inlines.Add(new LineBreak());
                }

                if (lib is LibraryStarSigns libSigns)
                {
                    WriteStarInSign(ref paragraph, libSigns);
                    paragraph.Inlines.Add(new LineBreak());
                    paragraph.Inlines.Add(new LineBreak());
                }

                if (lib is LibraryStarHouses libHouse)
                {
                    WriteStarInHouse(ref paragraph, libHouse);
                    paragraph.Inlines.Add(new LineBreak());
                    paragraph.Inlines.Add(new LineBreak());
                }

                if (lib is LibraryHouseSigns libHouseSign)
                {
                    WriteHouseInSign(ref paragraph, libHouseSign);
                    paragraph.Inlines.Add(new LineBreak());
                    paragraph.Inlines.Add(new LineBreak());
                }

            }

            flowDoc.Blocks.Add(paragraph);
            documentViewer.Document = flowDoc;
        }

        private void OnQuestionMarkClicked(object sender, RoutedEventArgs e)
        {
            if (_obj is Aspect aspect)
            {
                new TransitInfo((int)aspect.TransitType.TransitTypeId, aspect).ShowDialog();
            }
            else if (_obj is Star star)
            {
                new TransitInfo(skyTypeId, star).ShowDialog();
            }
            else if (_obj is House house)
            {

            }
        }
    }
}