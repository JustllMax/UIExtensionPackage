using System;
using System.Collections.Generic;

namespace UIExtensionPackage.ExtendedUI.Extensions
{

    /// <summary>
    /// Class extends different Collection structures
    /// </summary>
    public static class CollectionsExtensions
    {
        /// <summary>
        /// Adds each given element to the list without a need for an array
        /// </summary>
        public static void AddRange<T>(this List<T> list, params T[] elements) => list.AddRange(elements);

        /// <summary>
        /// Executes action for each element in collection
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T element in collection) action(element);
        }

        /// <summary>
        /// Searches through collection for item that matches predicate. Out found element.
        /// </summary>
        public static bool TryFind<T>(this IEnumerable<T> collection, Predicate<T> predicate, out T element)
        {
            element = default;
            foreach (T item in collection)
            {
                if (!predicate(item)) continue;
                element = item;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes item from collection that matches predicate
        /// </summary>
        public static bool TryRemove<T>(this ICollection<T> collection, Predicate<T> predicate)
        {
            foreach (T item in collection)
            {
                if (!predicate(item)) continue;
                collection.Remove(item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds element only if it doesn't already exist in the collection
        /// </summary>
        public static bool AddUnique<T>(this ICollection<T> collection, T element)
        {
            if (!collection.Contains(element))
            {
                collection.Add(element);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes element only if it exists in the collection
        /// </summary>
        public static bool RemoveElement<T>(this ICollection<T> collection, T element)
        {
            if (collection.Contains(element))
            {
                collection.Remove(element);
                return true;
            }

            return false;
        }
        
        
        /// <summary>
        /// Searches for value in dictionary, doesn't throw NullExceptions when key is missing
        /// </summary>
        public static bool TryFind<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            value = default!;
            
            if(!dictionary.ContainsKey(key)) return false;
                
            if (dictionary.TryGetValue(key, out value))
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Searches for element in collection
        /// </summary>
        public static bool Contains<T>(this IEnumerable<T> collection, T element)
        {
            foreach (T item in collection)
            {
                if (Equals(item, element)) return true;
            }

            return false;
        }

        /// <summary>
        /// Checks is the given list null or an empty list
        /// </summary>
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list == null || list.Count == 0;
        }

        /// <summary>
        /// Moves element to given index
        /// </summary>
        public static void Move<T>(this List<T> list, T item, int newIndex)
        {
            int oldIndex = list.IndexOf(item);
            if (oldIndex >= 0) list.Move(oldIndex, newIndex);
        }
        
        /// <summary>
        /// Moves element at index to new index
        /// </summary>
        public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
        {
            T item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }
    }
}