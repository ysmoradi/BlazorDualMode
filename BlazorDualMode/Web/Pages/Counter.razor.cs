using System;

namespace BlazorDualMode.Web.Pages
{
    public partial class Counter
    {
        int currentCount = 0;

        DateTime datePickerValue = DateTime.Now;

        void IncrementCount()
        {
            currentCount++;
        }
    }
}
