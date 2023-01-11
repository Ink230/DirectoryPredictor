namespace DirectoryPredictor;
static class ExceptionExtensions
{
    public static IEnumerable<TIn> Catch<TIn>(
                this IEnumerable<TIn> source,
                Type exceptionType)
    {
        using (var e = source.GetEnumerator())
            while (true)
            {
                var ok = false;

                try
                {
                    ok = e.MoveNext();
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != exceptionType)
                        throw;
                    continue;
                }

                if (!ok)
                    yield break;

                yield return e.Current;
            }
    }
}