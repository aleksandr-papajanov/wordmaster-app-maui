﻿using DynamicData;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.Infrastructure
{
    internal static class RealmExtensions
    {
        public static IObservable<IChangeSet<T>> ToObservableChangeSet1<T>(this IRealmCollection<T> collection)
        where T : RealmObject
        {
            return Observable.Create<IChangeSet<T>>(observer =>
            {
                return collection.SubscribeForNotifications((sender, changes) =>
                {
                    if (changes == null)
                    {
                        return; // initial subscription, ignore
                    }

                    var changeSet = new ChangeSet<T>();

                    foreach (var index in changes.DeletedIndices.OrderByDescending(i => i))
                    {
                        changeSet.RemoveAt(index); // Fixed: Removed the second argument as RemoveAt only takes one argument.
                    }

                    //foreach (var index in changes.ModifiedIndices)
                    //{
                    //    var old =new ChangeSet<T>();
                    //    var n = new ChangeSet<T>();

                    //    changeSet.Replace(old, n);
                    //}

                    foreach (var index in changes.InsertedIndices)
                    {
                        changeSet.Add(new Change<T>(ListChangeReason.Add, sender[index]));
                    }

                    observer.OnNext(changeSet);
                });
            });
        }

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
