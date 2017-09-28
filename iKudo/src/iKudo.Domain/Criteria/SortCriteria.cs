using System.Linq;

namespace iKudo.Domain.Criteria
{
    public class SortCriteria
    {
        public SortCriteria()
        {
        }

        public SortCriteria(string rawCriteria)
        {
            RawCriteria = rawCriteria;
        }

        public string RawCriteria { get; set; }

        public string Direction => RawCriteria.StartsWith("-") ? "DESC" : "ASC";

        public string Column => RawCriteria.TrimStart('-').FirstLetterToUpper();

        public string Criteria => $"{Column} {Direction}";
    }

    static class SortCriteriaExtensions
    {
        public static string FirstLetterToUpper(this string text)
        {
            string first = text.First().ToString().ToUpper();
            return $"{first}{text.Substring(1)}";
        }
    }
}
