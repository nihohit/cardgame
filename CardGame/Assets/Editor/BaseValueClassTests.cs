using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BaseValueClassTests {
	#region classes
	private struct Value {
		public int IntegerValue { get; set; }

		public override bool Equals(object obj) {
			return obj is Value && this.IntegerValue == ((Value)obj).IntegerValue;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}

	private class ReferenceValue {
		public int IntegerValue { get; set; }

		public override bool Equals(object obj) {
			 return obj is ReferenceValue && this.IntegerValue == ((ReferenceValue)obj).IntegerValue;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}

	private class TestValueClass : BaseValueClass {
		public int IntegerProperty { get; set; }
		public Value StructProperty { get; set; }
		public ReferenceValue ClassProperty { get; set; }

		public TestValueClass() {	}

		public TestValueClass(int integerProperty, Value structProperty, ReferenceValue classProperty) {
			IntegerProperty = integerProperty;
			StructProperty = structProperty;
			ClassProperty = classProperty;
		}
	}
	#endregion

	private TestValueClass first;
	private TestValueClass second;

	[SetUp]
	public void SetupTests() {
		first = new TestValueClass();
		second = new TestValueClass();
	}

	[Test]
	public void EqualIfNoPropertyIsSet() {
		checkAreEqual();
	}

	[Test]
	public void EqualAccordingToIntegerProperty() {
		first.IntegerProperty = 1;

		checkAreNotEqual();

		second.IntegerProperty = 1;

		checkAreEqual();
	}

	[Test]
	public void EqualAccordingToStructProperty() {
		first.StructProperty = new Value {
			IntegerValue = 1
		};

		checkAreNotEqual();

		second.StructProperty = new Value();

		checkAreNotEqual();

		second.StructProperty = new Value {
			IntegerValue = 1
		};

		checkAreEqual();
	}

	[Test]
	public void EqualAccordingToClassProperty() {
		first.ClassProperty = new ReferenceValue();

		checkAreNotEqual();

		second.ClassProperty = new ReferenceValue();

		checkAreEqual();

		first.ClassProperty.IntegerValue = 1;

		checkAreNotEqual();

		second.ClassProperty.IntegerValue = 1;

		checkAreEqual();
	}

	private void checkAreEqual() {
		Assert.AreEqual(first, second);
		Assert.AreEqual(second, first);
	}

	private void checkAreNotEqual() {
		Assert.AreNotEqual(first, second);
		Assert.AreNotEqual(second, first);
	}

	[Test]
	public void SetIntegerValue() {
		first.IntegerProperty = 2;
		first.ClassProperty = new ReferenceValue();
		first.StructProperty = new Value {
			IntegerValue = 1
		};

		var expectedValue = 3;
		var result = first.SetValue("IntegerProperty", expectedValue);

		var expected = new TestValueClass();
		expected.IntegerProperty = expectedValue;
		expected.ClassProperty = first.ClassProperty;
		expected.StructProperty = first.StructProperty;
		Assert.AreEqual(expected, result);
	}

	[Test]
	public void SetStructValue() {
		first.IntegerProperty = 2;
		first.ClassProperty = new ReferenceValue();
		first.StructProperty = new Value {
			IntegerValue = 1
		};

		var expectedValue = new Value {
			IntegerValue = 2
		};
		var result = first.SetValue("StructProperty", expectedValue);

		var expected = new TestValueClass();
		expected.IntegerProperty = first.IntegerProperty;
		expected.ClassProperty = first.ClassProperty;
		expected.StructProperty = expectedValue;
		Assert.AreEqual(expected, result);
	}

	[Test]
	public void SetReferenceValue() {
		first.IntegerProperty = 2;
		first.ClassProperty = new ReferenceValue();
		first.StructProperty = new Value {
			IntegerValue = 1
		};

		var expectedValue = new ReferenceValue();
		expectedValue.IntegerValue = 5;
		var result = first.SetValue("ClassProperty", expectedValue);

		var expected = new TestValueClass();
		expected.IntegerProperty = first.IntegerProperty;
		expected.ClassProperty = expectedValue;
		expected.StructProperty = first.StructProperty;
		Assert.AreEqual(expected, result);
	}
}
