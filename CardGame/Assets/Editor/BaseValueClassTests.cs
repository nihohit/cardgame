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
		public IEnumerable<object> EnumerableProperty { get; set; }

		public TestValueClass() {	}

		public TestValueClass(int integerProperty,
		                      Value structProperty, 
		                      ReferenceValue classProperty,
		                     IEnumerable<object> enumerableProperty) {
			IntegerProperty = integerProperty;
			StructProperty = structProperty;
			ClassProperty = classProperty;
			EnumerableProperty = enumerableProperty;
		}
	}

	private class ReadonlyValueClass : BaseValueClass {
		public int IntegerProperty { get; }
		public Value StructProperty { get; }
		public ReferenceValue ClassProperty { get; }
		public IEnumerable<object> EnumerableProperty { get; }

		public ReadonlyValueClass(int integerProperty,
													Value structProperty,
													ReferenceValue classProperty,
												 IEnumerable<object> enumerableProperty) {
			IntegerProperty = integerProperty;
			StructProperty = structProperty;
			ClassProperty = classProperty;
			EnumerableProperty = enumerableProperty;
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

	[Test]
	public void EqualAccordingToEnumerableProperty() {
		first.EnumerableProperty = new List<object>();

		checkAreNotEqual();

		second.EnumerableProperty = new object[0];

		checkAreEqual();

		first.EnumerableProperty = new List<object> {
			1
		};

		checkAreNotEqual();

		second.EnumerableProperty = new object[] {1};

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
		first.EnumerableProperty = new object[] { 1 };

		var expectedValue = 3;
		var result = first.CopyWithSetValue("IntegerProperty", expectedValue);

		var expected = new TestValueClass {
			IntegerProperty = expectedValue,
			ClassProperty = first.ClassProperty,
			StructProperty = first.StructProperty,
			EnumerableProperty = first.EnumerableProperty
		};

		Assert.AreEqual(expected, result);
	}

	[Test]
	public void ModifyIntegerValue() {
		first.IntegerProperty = 2;
		first.ClassProperty = new ReferenceValue();
		first.StructProperty = new Value {
			IntegerValue = 1
		};
		first.EnumerableProperty = new object[] { 1 };

		var modificationValue = 3;
		var result = first.CopyWithModifiedValue("IntegerProperty", modificationValue);

		var expected = new TestValueClass {
			IntegerProperty = 5,
			ClassProperty = first.ClassProperty,
			StructProperty = first.StructProperty,
			EnumerableProperty = first.EnumerableProperty
		};

		Assert.AreEqual(expected, result);
	}

	[Test]
	public void ModifyIntegerValues() {
		first.IntegerProperty = 2;
		first.ClassProperty = new ReferenceValue();
		first.StructProperty = new Value {
			IntegerValue = 1
		};
		first.EnumerableProperty = new object[] { 1 };

		var modificationValue = 3;
		var result = first.CopyWithModifiedValues(new Dictionary<string, int> { { "IntegerProperty", modificationValue } });

		var expected = new TestValueClass {
			IntegerProperty = 5,
			ClassProperty = first.ClassProperty,
			StructProperty = first.StructProperty,
			EnumerableProperty = first.EnumerableProperty
		};

		Assert.AreEqual(expected, result);
	}

	[Test]
	public void SetStructValue() {
		first.IntegerProperty = 2;
		first.ClassProperty = new ReferenceValue();
		first.StructProperty = new Value {
			IntegerValue = 1
		};
		first.EnumerableProperty = new object[] { 1 };

		var expectedValue = new Value {
			IntegerValue = 2
		};
		var result = first.CopyWithSetValue("StructProperty", expectedValue);

		var expected = new TestValueClass {
			IntegerProperty = first.IntegerProperty,
			ClassProperty = first.ClassProperty,
			StructProperty = expectedValue,
			EnumerableProperty = first.EnumerableProperty
		};

		Assert.AreEqual(expected, result);
	}

	[Test]
	public void SetReferenceValue() {
		first.IntegerProperty = 2;
		first.ClassProperty = new ReferenceValue();
		first.StructProperty = new Value {
			IntegerValue = 1
		};
		first.EnumerableProperty = 1.Yield<object>();

		var expectedValue = new ReferenceValue {
			IntegerValue = 5
		};
		var result = first.CopyWithSetValue("ClassProperty", expectedValue);

		var expected = new TestValueClass {
			IntegerProperty = first.IntegerProperty,
			ClassProperty = expectedValue,
			StructProperty = first.StructProperty,
			EnumerableProperty = first.EnumerableProperty
		};

		Assert.AreEqual(expected, result);
	}

	[Test]
	public void SetEnumerableValue() {
		first.IntegerProperty = 2;
		first.ClassProperty = new ReferenceValue();
		first.StructProperty = new Value {
			IntegerValue = 1
		};

		var expectedValue = 1.Yield<object>();
		var result = first.CopyWithSetValue("EnumerableProperty", expectedValue);

		var expected = new TestValueClass {
			IntegerProperty = first.IntegerProperty,
			ClassProperty = first.ClassProperty,
			StructProperty = first.StructProperty,
			EnumerableProperty = expectedValue
		};

		Assert.AreEqual(expected, result);
	}

	[Test]
	public void CreateObjectFromObject() {
		first.IntegerProperty = 2;
		first.ClassProperty = new ReferenceValue();
		first.StructProperty = new Value {
			IntegerValue = 1
		};
		first.EnumerableProperty = 1.Yield<object>();
		var copy = first.AsDictionary().ToObject<TestValueClass>();

		Assert.AreEqual(first, copy);
	}

	[Test]
	public void CreateObjectFromObjectWithPartialDictionary() {
		first.IntegerProperty = 2;
		first.ClassProperty = new ReferenceValue();
		first.StructProperty = new Value {
			IntegerValue = 1
		};
		first.EnumerableProperty = 1.Yield<object>();
		var dictionary = first.AsDictionary();
		dictionary.Remove("ClassProperty");
		var copy = dictionary.ToObject<TestValueClass>();

		Assert.AreEqual(first.IntegerProperty, copy.IntegerProperty);
		Assert.AreEqual(first.StructProperty, copy.StructProperty);
		Assert.AreEqual(first.EnumerableProperty, copy.EnumerableProperty);
		Assert.AreEqual(copy.ClassProperty, null);
	}

	[Test]
	public void CreateReadonlyObjectFromObject() {
		var source = new ReadonlyValueClass(2, new Value {
			IntegerValue = 1
		}, new ReferenceValue(), new object[] { 1 });
		var copy = source.AsDictionary().ToObject<ReadonlyValueClass>();

		Assert.AreEqual(source, copy);
	}

	[Test]
	public void CreateReadonlyObjectFromObjectWithPartialDictionary() {
		var source = new ReadonlyValueClass(2, new Value {
			IntegerValue = 1
		}, new ReferenceValue(), new object[] { 1 });
		var dictionary = source.AsDictionary();
		dictionary.Remove("ClassProperty");
		var copy = dictionary.ToObject<ReadonlyValueClass>();

		Assert.AreEqual(source.IntegerProperty, copy.IntegerProperty);
		Assert.AreEqual(source.StructProperty, copy.StructProperty);
		Assert.AreEqual(source.EnumerableProperty, copy.EnumerableProperty);
		Assert.AreEqual(copy.ClassProperty, null);
	}
}
