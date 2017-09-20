namespace iKudo.Common
{
    public static class NumberExtensions
    {
        public static int? ToNullableInt(this string numberText)
        {
            int number;
            if (int.TryParse(numberText, out number))
            {
                return number;
            }
            else
            {
                return null;
            }
        }
    }
}
