namespace iKudo.Dtos
{
    public interface IDtoFactory
    {
        TDestination Create<TDestination, TSource>(TSource source);
    }
}
