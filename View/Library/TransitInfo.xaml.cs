using AstroNET.model;
using Backend.Database;
using System.Windows;
using System.Windows.Documents;

namespace AstroNET.View
{
    public partial class TransitInfo : Window
    {
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

            AddStar(_star1);

            if (aspect.TransitType.TransitTypeId == 1)
            {
                _flowDoc.Blocks.Add(Utils.DescriptionParagraph("Nelle Case:", FontWeights.Bold));
                _flowDoc.Blocks.Add(Utils.DescriptionParagraph(_star1?.InHouseDescription));
            }
            else if (aspect.TransitType.TransitTypeId == 2)
            {
                _flowDoc.Blocks.Add(Utils.DescriptionParagraph("I Transiti:", FontWeights.Bold));
                _flowDoc.Blocks.Add(Utils.DescriptionParagraph(_star1?.TransitDescription));
            }


            if (_house1 != null)
            {
                AddHouse(_house1);
            }

            if (star2 != null)
            {
                AddStar(star2);
            }

            if (house2 != null)
            {
                AddHouse(house2);
            }

            documentViewer.Document = _flowDoc;
        }

        public TransitInfo(int transitId, Star star) : this()
        {
            _star1 = Stars?.FirstOrDefault(s => s.PointId == star.PointId);
            if (star.House != null)
                _house1 = Houses?.FirstOrDefault(s => s.PointId == star.House.PointId);


            AddStar(_star1);

            if (transitId == 1)
            {
                _flowDoc.Blocks.Add(Utils.DescriptionParagraph("Nelle Case:", FontWeights.Bold));
                _flowDoc.Blocks.Add(Utils.DescriptionParagraph(_star1?.InHouseDescription));
            }
            else if (transitId == 2) 
            {
                _flowDoc.Blocks.Add(Utils.DescriptionParagraph("I Transiti:", FontWeights.Bold));
                _flowDoc.Blocks.Add(Utils.DescriptionParagraph(_star1?.TransitDescription));
            }

            if (_house1 != null)
            {
                AddHouse(_house1);
            }

            documentViewer.Document = _flowDoc;
        }

        private void AddStar(Star? star)
        {
            _flowDoc.Blocks.Add(Utils.ImageParagraph(star?.URI, star?.ToString()));
            _flowDoc.Blocks.Add(Utils.DescriptionParagraph(star?.Description));
        }

        private void AddHouse(House? house)
        {
            _flowDoc.Blocks.Add(Utils.DescriptionParagraph($"{house}:", FontWeights.Bold));
            _flowDoc.Blocks.Add(Utils.DescriptionParagraph($"{house?.Description}"));
        }
    }
}