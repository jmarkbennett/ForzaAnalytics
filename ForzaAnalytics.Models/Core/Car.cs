namespace ForzaAnalytics.Models.Core
{
    public class Car
    {
        public string CarId { get; set; }
        public string YearMakeModel { get; set; }
        public string NickName { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Division { get; set; }
        public string Spec { get; set; }
        public string Pi { get; set; }
        public string Class { get; set; }
        public Car()
        {
            CarId = string.Empty;
            YearMakeModel = string.Empty;
            NickName = string.Empty;
            Year = string.Empty;
            Make = string.Empty;
            Model = string.Empty;
            Division = string.Empty;
            Spec = string.Empty;
            Pi = string.Empty;
            Class = string.Empty;
        }
    }
}