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
}
