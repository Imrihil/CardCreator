using System;
using System.Threading.Tasks;

namespace CardCreator.Features.SafeCaller
{
    public static class Safe
    {
        public static void Call(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static async Task CallAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public static class Safe<T>
    {
        public static T Call(Func<T> action, T @default)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return @default;
            }
        }

        public static async Task<T> CallAsync(Func<Task<T>> action, T @default)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return @default;
            }
        }

        public static async Task<T> CallAsync(Func<Task<T>> action, Func<Exception, Task<T>> catchAction, T @default)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                try
                {
                    return await catchAction(ex);
                }
                catch
                {
                    return @default;
                }
            }
        }
    }
}
