using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Infrastructure
{
    public static class RealmObservableExtensions
    {
        public static IObservable<IRealmCollection<T>> ToObservable<T>(this IRealmCollection<T> collection)
            where T : RealmObjectBase
        {
            return Observable.Create<IRealmCollection<T>>(observer =>
            {
                var token = collection.SubscribeForNotifications((sender, changes) =>
                {
                    observer.OnNext(sender);
                });

                return () => token.Dispose();
            });
        }
    }
}
