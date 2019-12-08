using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public struct UrlQuery : IEquatable<UrlQuery>, ICollection<KeyValuePair<string, StringValues>>
    {
        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

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

        public void CopyTo(KeyValuePair<string, StringValues>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Equals(UrlQuery other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, StringValues> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
