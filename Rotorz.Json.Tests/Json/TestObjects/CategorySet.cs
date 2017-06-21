// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System.Collections.Generic;

namespace Rotorz.Json.Tests.TestObjects
{
    public class CategorySet : ICollection<int>
    {
        private HashSet<int> set = new HashSet<int>();


        #region ICollection<int> Members

        public void Add(int categoryNumber)
        {
            this.set.Add(categoryNumber);
        }

        public void Clear()
        {
            this.set.Clear();
        }

        public bool Contains(int categoryNumber)
        {
            return this.set.Contains(categoryNumber);
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            this.set.CopyTo(array, arrayIndex);
        }

        public int Count {
            get { return this.set.Count; }
        }

        bool ICollection<int>.IsReadOnly {
            get { return false; }
        }

        public bool Remove(int categoryNumber)
        {
            return this.set.Remove(categoryNumber);
        }

        #endregion


        #region IEnumerable<int> Members

        public IEnumerator<int> GetEnumerator()
        {
            return this.set.GetEnumerator();
        }

        #endregion


        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.set.GetEnumerator();
        }

        #endregion
    }
}
