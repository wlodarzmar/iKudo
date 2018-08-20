namespace iKudo.Dtos
{
    internal class Projection
    {
        private string fields;

        public Projection(string fields)
        {
            this.fields = fields;
        }

        internal string ToProjectionString()
        {
            return $"new({string.Join(',', fields)})";
        }
    }
}