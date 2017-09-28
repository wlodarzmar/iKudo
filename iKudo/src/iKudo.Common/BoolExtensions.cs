namespace iKudo.Common
{
    public static class BoolExtensions
    {
        public static bool? ToNullableBool(this string boolText)
        {
            bool result;
            if (bool.TryParse(boolText, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
