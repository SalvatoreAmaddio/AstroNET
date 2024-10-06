using AstroNET.model;
using Backend.Database;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace AstroNET.View
{
    public partial class TransitInfo : Window
    {
        private readonly StringBuilder sb = new();
        private readonly Star? _star1 = null;
        private readonly House? _house1 = null;
        private readonly FlowDocument _flowDoc = new();
        private readonly Paragraph _paragraph = new();
        private readonly List<Star>? Stars = DatabaseManager.Find<Star>()?.MasterSource.Cast<Star>().ToList();
        private readonly List<House>? Houses = DatabaseManager.Find<House>()?.MasterSource.Cast<House>().ToList();
        public TransitInfo()
        {
            InitializeComponent();
        }

        public TransitInfo(int transitId, Aspect aspect) : this()
        {
            TransitTypeInfo? info = App.TransitTypeDescriptions?.TransitTypeInfo.FirstOrDefault(s => s.TransitTypeId == transitId && s.StarId == aspect.PointA.PointId);
            Star? star2 = null;
            House? house2 = null;

            if (aspect.PointA is Star star)
            {
                _star1 = Stars?.FirstOrDefault(s => s.PointId == star.PointId);
                if (star.House != null)
                    _house1 = Houses?.FirstOrDefault(s => s.PointId == star.House.PointId);
            }

            if (aspect.PointB is Star pointB)
            {
                star2 = Stars?.FirstOrDefault(s => s.PointId == pointB.PointId);
                if (pointB.House != null)
                    house2 = Houses?.FirstOrDefault(s => s.PointId == pointB.House.PointId);

            }
            else if (aspect.PointB is House house)
            {
                house2 = Houses?.FirstOrDefault(s => s.PointId == house.PointId);
            }

            AddImg(Utils.Img(_star1?.URI, _star1?.ToString()));
            sb.Append(_star1?.Description);
            sb.Append("\n\n");
            sb.Append(info?.TransitDescription);

            if (_house1 != null)
            {
                sb.Append("\n\n");
                sb.Append($"{_house1}:\n\n");
                sb.Append(_house1.Description);
            }

            AddParagraph(sb.ToString());

            sb.Clear();

            if (star2 != null)
            {
                AddParagraph("\n\n");
                AddImg(Utils.Img(star2?.URI, star2?.ToString()));
                sb.Append(star2?.Description);
            }

            if (house2 != null)
            {
                sb.Append("\n\n");
                sb.Append($"{house2}:\n\n");
                sb.Append(house2.Description);
            }

            AddParagraph(sb.ToString());

            documentViewer.Document = _flowDoc;
        }

        public TransitInfo(int transitId, Star star) : this()
        {
            TransitTypeInfo? info = App.TransitTypeDescriptions?.TransitTypeInfo.FirstOrDefault(s => s.TransitTypeId == transitId && s.StarId == star.PointId);
            _star1 = Stars?.FirstOrDefault(s => s.PointId == star.PointId);
            if (star.House != null)
                _house1 = Houses?.FirstOrDefault(s => s.PointId == star.House.PointId);


            AddImg(Utils.Img(_star1?.URI, _star1?.ToString()));
            sb.Append(_star1?.Description);
            sb.Append("\n\n");
            sb.Append(info?.TransitDescription);

            if (_house1 != null)
            {
                sb.Append("\n\n");
                sb.Append($"{_house1}:\n\n");
                sb.Append(_house1.Description);
            }

            AddParagraph(sb.ToString());

            documentViewer.Document = _flowDoc;
        }

        private void AddParagraph(string? text)
        {
            _paragraph.Inlines.Add(new Run(text));
            _flowDoc.Blocks.Add(_paragraph);
        }

        private void AddImg(InlineUIContainer? image)
        {
            _paragraph.Inlines.Add(image);
            _paragraph.Inlines.Add(" ");
        }

    }
}