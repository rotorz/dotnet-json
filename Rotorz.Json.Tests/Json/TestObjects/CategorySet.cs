// Copyright (c) Rotorz Limited. All rights reserved.

using System.Collections.Generic;

namespace Rotorz.Json.Tests.TestObjects {

	public class CategorySet : ICollection<int> {

		private HashSet<int> _set = new HashSet<int>();

		#region ICollection<int> Members

		public void Add(int categoryNumber) {
			_set.Add(categoryNumber);
		}

		public void Clear() {
			_set.Clear();
		}

		public bool Contains(int categoryNumber) {
			return _set.Contains(categoryNumber);
		}

		public void CopyTo(int[] array, int arrayIndex) {
			_set.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return _set.Count; }
		}

		bool ICollection<int>.IsReadOnly {
			get { return false; }
		}

		public bool Remove(int categoryNumber) {
			return _set.Remove(categoryNumber);
		}

		#endregion

		#region IEnumerable<int> Members

		public IEnumerator<int> GetEnumerator() {
			return _set.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return _set.GetEnumerator();
		}

		#endregion

	}

}
