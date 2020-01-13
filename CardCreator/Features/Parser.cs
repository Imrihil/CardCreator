using CardCreator.Features.Logging;
using System;

namespace CardCreator.Features
{
    public static class Parser<T>
    {
        public static T Parse(ILogger logger, string value, Func<string, T> parse, Func<T, bool> validate, string errorMessage)
        {
            try
            {
                var result = parse(value);
                if (!validate(result))
                {
                    logger?.LogMessage(errorMessage);
                    throw new ArgumentException(errorMessage);
                }

                return result;
            }
            catch
            {
                throw new ArgumentException(errorMessage);
            }
        }
    }
}
