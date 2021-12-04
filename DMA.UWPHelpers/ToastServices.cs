using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA.UWPHelpers
{
    public class ToastServices
    {
        public static void PopToast(string message)
        {
            // Generate the toast notification content
            ToastContentBuilder builder = new ToastContentBuilder();

            // Include launch string so we know what to open when user clicks toast        
            builder.AddArgument("action", "viewForecast");
            builder.AddArgument("zip", 98008);

            // We'll always have this summary text on our toast notification        
            // (it is required that your toast starts with a text element)        
            builder.AddText(message);

            var paramsBtn = new ToastButton()
            .SetSnoozeActivation("Paramètres");


            builder.AddButton(new ToastButton()
            .SetSnoozeActivation("Paramètres"));
            builder.Show();

        }
    }
}
