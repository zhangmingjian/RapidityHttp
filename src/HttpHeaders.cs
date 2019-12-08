using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public struct HttpHeaders : IDictionary<string, StringValues>, ICollection<KeyValuePair<string, StringValues>>
    {
        public StringValues this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<string> Keys => throw new NotImplementedException();

        public ICollection<StringValues> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(string key, StringValues value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<string, StringValues> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, StringValues> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, StringValues>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, StringValues> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
