using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespServer.Helpers
{
    static class ObservableHelper
    {
        public static IObservable<T> TakeWhileInclusive<T>(
    this IObservable<T> source, Func<T, bool> predicate)
        {
            return source.Publish(co => co.TakeWhile(predicate)
                .Merge(co.SkipWhile(predicate).Take(1)));
        }
    }
}
