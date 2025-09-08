namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class LocationDto
    {
        public class StateDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class CityDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int StateId { get; set; }
        }
    }
}
