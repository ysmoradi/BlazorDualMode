namespace BlazorDualMode.Web.Pages
{
    public partial class Counter
    {
        public int CurrentCount { get; set; }

        public void IncrementCount()
        {
            CurrentCount++;
        }
    }
}
