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

        public TransitInfo(int transitId, int pointId) : this()
        {
            TransitTypeInfo? info = App.TransitTypeDescriptions?.TransitTypeInfo.FirstOrDefault(s=>s.TransitTypeId == transitId && s.StarId == pointId);
            FlowDocument flowDoc = new();
            Paragraph paragraph = new();
            paragraph.Inlines.Add(info?.TransitDescription);
            flowDoc.Blocks.Add(paragraph);
            documentViewer.Document = flowDoc;
        }
    }
}