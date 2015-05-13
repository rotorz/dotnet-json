// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.Collections.Generic;

namespace Rotorz.Json.Internal {

	internal sealed class SimpleStack<T> {

		private readonly List<T> _stack = new List<T>();

		public void Push(T value) {
			_stack.Add(value);
		}

		public T Pop() {
			var top = Peek();
			_stack.RemoveAt(_stack.Count - 1);
			return top;
		}

		public T Peek() {
			if (_stack.Count == 0)
				throw new InvalidOperationException("Stack is empty.");

			return _stack[_stack.Count - 1];
		}

		public int Count {
			get { return _stack.Count; }
		}

	}

}
