namespace MienDev.AspNetCore.Identity.MongoDB.Models
{
    /// <summary>
    /// Indicate the contact type.
    /// ! Be Caution: 
    /// Be carefully Change this value (even order without value)
    /// change maybe lead to disorder in production envirment,
    /// </summary>
    public enum ContactType
    {
        None,
        Email =1,
        Mobile =2,
        Telephone =4
    };
}