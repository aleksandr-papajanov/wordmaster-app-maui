using DynamicData;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Infrastructure
{
    internal static class RealmExtensions
    {
        public static IDisposable BindToSourceList<T>(this IRealmCollection<T> collection, ISourceList<T> targetSourceList)
            where T : RealmObject
        {
            return collection.SubscribeForNotifications((sender, changes) =>
            {
                if (changes == null)
                    return;

                targetSourceList.Edit(list =>
                {
                    foreach (var index in changes.DeletedIndices)
                    {
                        list.RemoveAt(index);
                    }

                    foreach (var index in changes.ModifiedIndices)
                    {
                        list[index] = sender[index];
                    }

                    foreach (var index in changes.InsertedIndices)
                    {
                        list.Insert(index, sender[index]);
                    }
                });
            });
        }
    }
}
