using System.Windows;

namespace AstroNET.View.Customs
{
    public class Helper
    {
        public static void DisplayTaskCancelled()
        {
            MessageBox.Show("Task Cancelled", "Action Aborted", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK);
        }
    }
}