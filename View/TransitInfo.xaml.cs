using AstroNET.model;
using System.Windows;
using System.Windows.Documents;

namespace AstroNET.View
{
    public partial class TransitInfo : Window
    {
        public TransitInfo()
        {
            InitializeComponent();
        }

        public TransitInfo(int id, int pointId, bool isHouse = false) : this()
        {
            TransitTypeInfo? info = App.TransitTypeDescriptions?.TransitTypeInfo.First(s=>s.TransitTypeId == id && s.StarId == pointId && s.IsHouse == isHouse);
            FlowDocument flowDoc = new();
            Paragraph paragraph = new();
            paragraph.Inlines.Add(info?.TransitDescription);
            flowDoc.Blocks.Add(paragraph);
            documentViewer.Document = flowDoc;
        }
    }
}