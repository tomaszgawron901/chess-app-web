using System;
using System.Threading.Tasks;

namespace ChessApp.Web.Exstentions
{
    public static class TaskExtensions
    {
        public static Task<T2> Then<T1, T2>(this Task<T1> first, Func<T1, Task<T2>> next)
        {
            return first.ContinueWith(t => next(t.Result)).Unwrap();
        }

        public static Task Then<T>(this Task<T> first, Func<T, Task> next)
        {
            return first.ContinueWith(t => next(t.Result)).Unwrap();
        }

        public static Task Then(this Task first, Func<Task> next)
        {
            return first.ContinueWith(_ => next()).Unwrap();
        }

        public static Task<T> Then<T>(this Task first, Func<Task<T>> next)
        {
            return first.ContinueWith(_ => next()).Unwrap();
        }
    }
}
