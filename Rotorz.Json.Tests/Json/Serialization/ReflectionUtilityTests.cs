// Copyright (c) Rotorz Limited. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rotorz.Json.Tests.TestObjects;

namespace Rotorz.Json.Serialization.Tests {

	[TestClass]
	public class ReflectionUtilityTests {

		#region IsNumericType(Type)

		[TestMethod]
		public void IsNumericType_BasicNumericTypes() {
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(long)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(ulong)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(int)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(uint)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(short)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(ushort)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(byte)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(sbyte)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(char)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(float)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(double)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(decimal)));
		}

		[TestMethod]
		public void IsNumericType_NullableNumericTypes() {
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(long?)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(ulong?)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(int?)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(uint?)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(short?)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(ushort?)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(byte?)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(sbyte?)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(char?)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(float?)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(double?)));
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(decimal?)));
		}

		[TestMethod]
		public void IsNumericType_EnumTypes() {
			Assert.IsTrue(ReflectionUtility.IsNumericType(typeof(EnumForTesting)));
		}

		[TestMethod]
		public void IsNumericType_NonNumericTypes() {
			Assert.IsFalse(ReflectionUtility.IsNumericType(typeof(bool)));
			Assert.IsFalse(ReflectionUtility.IsNumericType(typeof(bool?)));
			Assert.IsFalse(ReflectionUtility.IsNumericType(typeof(string)));
			Assert.IsFalse(ReflectionUtility.IsNumericType(typeof(object)));
		}

		#endregion

		#region IsIntegralType(Type)

		[TestMethod]
		public void IsIntegralType_BasicIntegralTypes() {
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(long)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(ulong)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(int)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(uint)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(short)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(ushort)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(byte)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(sbyte)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(char)));
		}

		[TestMethod]
		public void IsIntegralType_BasicNonIntegralTypes() {
			Assert.IsFalse(ReflectionUtility.IsIntegralType(typeof(float)));
			Assert.IsFalse(ReflectionUtility.IsIntegralType(typeof(double)));
			Assert.IsFalse(ReflectionUtility.IsIntegralType(typeof(decimal)));
		}

		[TestMethod]
		public void IsIntegralType_NullableIntegralTypes() {
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(long?)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(ulong?)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(int?)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(uint?)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(short?)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(ushort?)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(byte?)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(sbyte?)));
			Assert.IsTrue(ReflectionUtility.IsIntegralType(typeof(char?)));
		}

		[TestMethod]
		public void IsIntegralType_NullableNonIntegralTypes() {
			Assert.IsFalse(ReflectionUtility.IsIntegralType(typeof(float?)));
			Assert.IsFalse(ReflectionUtility.IsIntegralType(typeof(double?)));
			Assert.IsFalse(ReflectionUtility.IsIntegralType(typeof(decimal?)));
		}

		#endregion

		#region IsBooleanType(Type)

		[TestMethod]
		public void IsBooleanType_BasicBooleanTypes() {
			Assert.IsTrue(ReflectionUtility.IsBooleanType(typeof(bool)));
			Assert.IsTrue(ReflectionUtility.IsBooleanType((true).GetType()));
			Assert.IsTrue(ReflectionUtility.IsBooleanType((false).GetType()));
		}

		[TestMethod]
		public void IsBooleanType_NullableBooleanType() {
			Assert.IsTrue(ReflectionUtility.IsBooleanType(typeof(bool?)));
		}

		[TestMethod]
		public void IsBooleanType_NonBooleanTypes() {
			Assert.IsFalse(ReflectionUtility.IsBooleanType(typeof(int)));
			Assert.IsFalse(ReflectionUtility.IsBooleanType(typeof(string)));
			Assert.IsFalse(ReflectionUtility.IsBooleanType(typeof(byte)));
			Assert.IsFalse(ReflectionUtility.IsBooleanType(typeof(object)));
			Assert.IsFalse(ReflectionUtility.IsBooleanType(typeof(EnumForTesting)));
		}

		#endregion

	}

}
