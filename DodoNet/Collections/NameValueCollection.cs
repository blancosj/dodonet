using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace DodoNet.Collections
{
    public class NameValueCollection<TValue> : NameObjectCollectionBase
    {
        /// <summary>
        /// Creates an empty collection.
        /// </summary>
        public NameValueCollection()
        {
        }

        /// <summary>
        /// Gets a key-and-value pair (DictionaryEntry) using an index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TValue this[int index]
        {
            get
            {
                return (TValue)this.BaseGet(index);
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[String key]
        {
            get
            {
                return (TValue)(this.BaseGet(key));
            }
            set
            {
                this.BaseSet(key, value);
            }
        }

        /// <summary>
        /// Gets a String array that contains all the keys in the collection.
        /// </summary>
        public String[] AllKeys
        {
            get
            {
                return (this.BaseGetAllKeys());
            }
        }

        /// <summary>
        /// Gets an Object array that contains all the values in the collection.
        /// </summary>
        public TValue[] AllValues
        {
            get
            {
                TValue[] ret = default(TValue[]);
                object[] tmp = this.BaseGetAllValues();
                ret = new TValue[tmp.Length];
                tmp.CopyTo(ret, 0);
                return ret;
            }
        }

        // Gets a value indicating if the collection contains keys that are not null.
        public Boolean HasKeys
        {
            get
            {
                return (this.BaseHasKeys());
            }
        }

        // Adds an entry to the collection.
        public void Add(String key, TValue value)
        {
            this.BaseAdd(key, value);
        }

        // Removes an entry with the specified key from the collection.
        public void Remove(String key)
        {
            this.BaseRemove(key);
        }

        // Removes an entry in the specified index from the collection.
        public void Remove(int index)
        {
            this.BaseRemoveAt(index);
        }

        // Clears all the elements in the collection.
        public void Clear()
        {
            this.BaseClear();
        }

    }
}
