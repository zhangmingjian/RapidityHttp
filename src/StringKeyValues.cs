using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class StringKeyValues : IEquatable<StringKeyValues>, IEnumerable<KeyValuePair<string, IEnumerable<string>>>, IEnumerable
    {
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, IEnumerable<string>> InnerStore = new Dictionary<string, IEnumerable<string>>(StringComparer.CurrentCultureIgnoreCase);

        public StringKeyValues() { }

        public StringKeyValues(string key, string value)
        {
            Add(key, value);
        }

        public StringKeyValues(string key, IEnumerable<string> values)
        {
            Add(key, values);
        }

        public IEnumerable<string> this[string name] => Contains(name) ? InnerStore[name] : new List<string>();

        public int Count => InnerStore.Count;

        public IEnumerable<string> Keys => InnerStore.Keys;

        public virtual void Add(string key, string value)
        {
            if (Contains(key))
            {
                var values = this[key].ToList();
                values.Add(value);
                InnerStore[key] = values;
            }
            else
            {
                InnerStore[key] = new List<string> { value };
            }
        }

        public virtual void Add(string key, IEnumerable<string> values)
        {
            if (Contains(key))
            {
                var list = this[key].ToList();
                list.AddRange(values);
                InnerStore[key] = list;
            }
            else
            {
                InnerStore[key] = values;
            }
        }

        public virtual void Add(StringKeyValues keyValues)
        {
            foreach (var key in keyValues.Keys)
                Add(key, keyValues[key]);
        }

        public virtual void Remove(string name)
        {
            InnerStore.Remove(name);
        }

        public virtual void Clear()
        {
            InnerStore.Clear();
        }

        public virtual bool Contains(string name)
        {
            return InnerStore.ContainsKey(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(StringKeyValues other)
        {
            if (this.Count != other.Count) return false;
            foreach (var key in Keys)
            {
                if (!other.Contains(key)) return false;
                var values = this[key];
                IEnumerable<string> otherValues = other[key];
                if (values.Count() != otherValues.Count()) return false;
                foreach (var value in values)
                {
                    if (!otherValues.Contains(value, StringComparer.CurrentCultureIgnoreCase)) return false;
                }
            }
            return true;
        }

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            return InnerStore.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Dictionary<string, IEnumerable<string>>.Enumerator();
        }
    }
}