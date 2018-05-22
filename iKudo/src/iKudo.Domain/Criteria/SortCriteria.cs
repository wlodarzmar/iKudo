using System.Linq;

namespace iKudo.Domain.Criteria
{
    public class SortCriteria
    {
        public SortCriteria()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawCriteria">raw criteria string i.e -fieldName</param>
        public SortCriteria(string rawCriteria)
        {
            RawCriteria = rawCriteria;
        }

        public string RawCriteria { get; set; }

        /// <summary>
        /// Returns sort order: ASC or DESC
        /// </summary>
        public string Direction
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RawCriteria))
                {
                    return null;
                }

                return RawCriteria.StartsWith("-") ? "DESC" : "ASC";
            }
        }

        /// <summary>
        /// Returns field name to sort on
        /// </summary>
        public string Column => RawCriteria?.TrimStart('-')?.FirstLetterToUpper();

        /// <summary>
        /// Returns full criteria i.e FieldName DESC
        /// </summary>
        public string Criteria => $"{Column} {Direction}";
    }

    static class SortCriteriaExtensions
    {
        public static string FirstLetterToUpper(this string text)
        {
            if (text == null)
            {
                return text;
            }

            string first = text.First().ToString().ToUpper();
            return $"{first}{text.Substring(1)}";
        }
    }
}
