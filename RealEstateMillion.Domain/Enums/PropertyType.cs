namespace RealEstateMillion.Domain.Enums
{
    public enum PropertyType
    {
        House = 1,
        Apartment = 2,
        Condo = 3,
        Townhouse = 4,
        Villa = 5,
        Commercial = 6,
        Land = 7,
        Industrial = 8
    }

    public enum PropertyStatus
    {
        Available = 1,
        Sold = 2,
        UnderContract = 3,
        Rented = 4,
        OffMarket = 5
    }

    public enum ListingType
    {
        Sale = 1,
        Rent = 2,
        Both = 3
    }

    public enum PropertyCondition
    {
        New = 1,
        Excellent = 2,
        Good = 3,
        Fair = 4,
        Poor = 5,
        NeedsRenovation = 6
    }
}
