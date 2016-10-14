namespace MienDev.AspNetCore.Identity.MongoDB.Utils
{
    public static class StringExtentsions
    {
        public static bool IsEmpty(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }
        public static bool IsNotEmpty(this string source)
        {
            return !source.IsEmpty();
        }
    }
}