using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BaseEnumerationTests {
	private delegate double ChangeListAction(string param1, int number);
	private delegate IEnumerable<int> EnumerateIntToInt(IEnumerable<int> enumerable);

	private class CheckingEnumerable<T> : IEnumerable<T> {
		public int CurrentCount {
			get {
				return latestCreatedEnumerator?.CurrentCount ?? 0;
			}
		}
		public int MoveNextCount {
			get {
				return latestCreatedEnumerator?.MoveNextCount ?? 0;
			}
		}
		public int ResetCount {
			get {
				return latestCreatedEnumerator?.ResetCount ?? 0;
			}
		}
		public int DisposeCount {
			get {
				return latestCreatedEnumerator?.DisposeCount ?? 0;
			}
		}
		public int GetEnumeratorCount { get; private set; }
		private CheckingEnumerator latestCreatedEnumerator;

		private IEnumerable<T> internalEnumerable;

		private class CheckingEnumerator : IEnumerator<T> {
			public int CurrentCount { get; private set; }
			public int MoveNextCount { get; private set; }
			public int ResetCount { get; private set; }
			public int DisposeCount { get; private set; }
			private IEnumerator<T> internalEnumerator;

			public CheckingEnumerator(IEnumerator<T> enumerator) {
				internalEnumerator = enumerator;
			}

			public T Current {
				get {
					++CurrentCount;
					return internalEnumerator.Current;
				}
			}

			object IEnumerator.Current {
				get {
					++CurrentCount;
					return internalEnumerator.Current;
				}
			}

			public void Dispose() {
				++DisposeCount;
				internalEnumerator.Dispose();
			}

			public bool MoveNext() {
				++MoveNextCount;
				return internalEnumerator.MoveNext();
			}

			public void Reset() {
				++ResetCount;
				internalEnumerator.Reset();
			}
		}

		public CheckingEnumerable(IEnumerable<T> enumerable = null) {
			internalEnumerable = (enumerable ?? new List<T>());
		}

		public IEnumerator<T> GetEnumerator() {
			++GetEnumeratorCount;
			latestCreatedEnumerator =
				new CheckingEnumerator(internalEnumerable.GetEnumerator());
			return latestCreatedEnumerator;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			++GetEnumeratorCount;
			latestCreatedEnumerator =
				new CheckingEnumerator(internalEnumerable.GetEnumerator());
			return latestCreatedEnumerator;
		}
	}

	private bool singleEnumerableDoNotMaterialize<T, Y>(
		Func<IEnumerable<T>, IEnumerable<Y>> enumerableCreation) {
		var enumerable = new CheckingEnumerable<T>();
		var resultingEnumerable = enumerableCreation(enumerable);
		return enumerable.GetEnumeratorCount == 0;
	}

	private bool singleEnumerableMaterializeOnce<T, Y>(
		Func<IEnumerable<T>, IEnumerable<Y>> enumerableCreation) {
		var enumerable = new CheckingEnumerable<T>();
		var resultingEnumerable = enumerableCreation(enumerable);
		foreach (var value in resultingEnumerable) {
		}
		return enumerable.GetEnumeratorCount == 1;
	}

	private bool singleEnumerableEnumerateOnce<T, Y>(
		Func<IEnumerable<T>, IEnumerable<Y>> enumerableCreation,
		IEnumerable<T> baseEnumerable) {
		var list = baseEnumerable.ToList();
		var enumerable = new CheckingEnumerable<T>(list);
		var resultingEnumerable = enumerableCreation(enumerable);
		foreach (var value in resultingEnumerable) {
		}
		return enumerable.MoveNextCount == list.Count + 1 &&
			enumerable.CurrentCount == list.Count;
	}

	private bool twoEnumerablesDoNotMaterialize<T, Y>(
	Func<IEnumerable<T>, IEnumerable<T>, IEnumerable<Y>> enumerableCreation) {
		var enumerable1 = new CheckingEnumerable<T>();
		var enumerable2 = new CheckingEnumerable<T>();
		var resultingEnumerable = enumerableCreation(enumerable1, enumerable2);
		return enumerable1.GetEnumeratorCount == 0 &&
			 enumerable2.GetEnumeratorCount == 0;
	}

	private bool twoEnumerablesMaterializeOnce<T, Y>(
		Func<IEnumerable<T>, IEnumerable<T>, IEnumerable<Y>> enumerableCreation) {
		var enumerable1 = new CheckingEnumerable<T>();
		var enumerable2 = new CheckingEnumerable<T>();
		var resultingEnumerable = enumerableCreation(enumerable1, enumerable2);
		foreach (var value in resultingEnumerable) {
		}
		return enumerable1.GetEnumeratorCount == 1 &&
			enumerable2.GetEnumeratorCount == 1;
	}

	private bool twoEnumerablesEnumerateOnce<T, Y>(
		Func<IEnumerable<T>, IEnumerable<T>, IEnumerable<Y>> enumerableCreation,
		IEnumerable<T> baseEnumerable1,
		IEnumerable<T> baseEnumerable2) {
		var list1 = baseEnumerable1.ToList();
		var list2 = baseEnumerable2.ToList();
		var enumerable1 = new CheckingEnumerable<T>(list1);
		var enumerable2 = new CheckingEnumerable<T>(list2);
		var resultingEnumerable = enumerableCreation(enumerable1, enumerable2);
		foreach (var value in resultingEnumerable) {
		}
		return enumerable1.MoveNextCount == list1.Count + 1 &&
			enumerable1.CurrentCount == list1.Count &&
			enumerable2.MoveNextCount == list2.Count + 1 &&
			enumerable2.CurrentCount == list2.Count;
	}

	private void lazyEvaluationSingleEnumerableTests(
		Func<IEnumerable<int>, IEnumerable<int>> enumerableCreation) {

		Assert.IsTrue(singleEnumerableDoNotMaterialize(enumerableCreation));
		Assert.IsTrue(singleEnumerableMaterializeOnce(enumerableCreation));
		Assert.IsTrue(singleEnumerableEnumerateOnce(
			enumerableCreation,
			Enumerable.Range(1, 3)));
	}

	private void lazyEvaluationTwoEnumerablesTests(
	Func<IEnumerable<int>, IEnumerable<int>, IEnumerable<int>> enumerableCreation) {

		Assert.IsTrue(twoEnumerablesDoNotMaterialize(enumerableCreation));
		Assert.IsTrue(twoEnumerablesMaterializeOnce(enumerableCreation));
		Assert.IsTrue(twoEnumerablesEnumerateOnce(
			enumerableCreation,
			Enumerable.Range(1, 3),
			Enumerable.Range(4, 2)));
	}

	[Test]
	public void TestDuplicate() {
		lazyEvaluationSingleEnumerableTests(MyExtensions.Duplicate);

		Assert.AreEqual(Enumerable.Range(1, 3), Enumerable.Range(1, 3).Duplicate());
	}

	[Test]
	public void TestShuffle() {
		lazyEvaluationSingleEnumerableTests(MyExtensions.Shuffle);

		CollectionAssert.AreNotEqual(Enumerable.Range(1, 100),
			Enumerable.Range(1, 100).Shuffle());
		CollectionAssert.AreEquivalent(Enumerable.Range(1, 100),
			Enumerable.Range(1, 100).Shuffle());

		CollectionAssert.AreEquivalent(new object[0], new object[0].Shuffle());
	}

	[Test]
	public void TestChooseRandomValuesWithoutRepetitions() {
		CollectionAssert.AreNotEqual(Enumerable.Range(1, 100),
			Enumerable.Range(1, 100).ChooseRandomValuesWithoutRepetitions(100));
		CollectionAssert.AreEquivalent(Enumerable.Range(1, 100),
			Enumerable.Range(1, 100).ChooseRandomValuesWithoutRepetitions(100));
	}

	[Test]
	public void TestInterleave() {
		lazyEvaluationTwoEnumerablesTests(MyExtensions.Interleave);
		CollectionAssert.AreEqual(new object[0], new object[0].Interleave(new object[0]));

		CollectionAssert.AreNotEqual(
			Enumerable.Range(1, 50).Interleave(Enumerable.Range(51, 50)),
			Enumerable.Range(1, 100));
		CollectionAssert.AreEquivalent(
			Enumerable.Range(1, 100),
			Enumerable.Range(1, 50).Interleave(Enumerable.Range(51, 50)));

		// Verify order is kept
		var interleaveEnumeration = Enumerable.Range(1, 50)
			.Interleave(Enumerable.Range(51, 50))
			.ToList();
		var firstIndices = Enumerable.Range(1, 50)
			.Select(val => interleaveEnumeration.IndexOf(val))
			.ToList();
		var secondIndices = Enumerable.Range(51, 50)
			.Select(val => interleaveEnumeration.IndexOf(val))
			.ToList();
		CollectionAssert.AreEqual(firstIndices, firstIndices.OrderBy(val => val));
		CollectionAssert.AreEqual(secondIndices, secondIndices.OrderBy(val => val));
	}

	private IEnumerator simpleEnumerator(List<int> list, int toAdd) {
		list.Add(toAdd);

		yield return null;
	}

	private IEnumerator actTwiceEnumerator(List<int> list, int toAdd) {
		list.Add(toAdd);

		yield return null;

		list.Add(toAdd + 1);

		yield return null;
	}

	private IEnumerator actAfterEnumerator(List<int> list, int toAdd) {
		list.Add(toAdd);

		yield return null;

		list.Add(toAdd + 1);
	}

	private IEnumerator assertEnumerator(List<int> list, int toAdd) {
		CollectionAssert.DoesNotContain(list, toAdd);

		yield return null;

		CollectionAssert.Contains(list, toAdd);
	}

	[Test]
	public void joinBasicEnumeratorsCorrectly() {
		var list = new List<int>();

		var enumerator = simpleEnumerator(list, 1).Join(simpleEnumerator(list, 2));

		while (enumerator.MoveNext()) {
		}

		CollectionAssert.AreEqual(new List<int> { 1, 2 }, list);
	}

	[Test]
	public void joinLongEnumeratorsCorrectly() {
		var list = new List<int>();

		var enumerator = actTwiceEnumerator(list, 1).Join(simpleEnumerator(list, 3));

		while (enumerator.MoveNext()) {
		}

		CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, list);
	}

	[Test]
	public void joinComplexEnumeratorsCorrectly() {
		var list = new List<int>();

		var enumerator = simpleEnumerator(list, 1).Join(actAfterEnumerator(list, 2));

		while (enumerator.MoveNext()) {
		}

		CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, list);
	}

	[Test]
	public void joinMultipleEnumeratorsCorrectly() {
		var list = new List<int>();

		var enumerator = simpleEnumerator(list, 3).Join(actAfterEnumerator(list, 4));
		enumerator = actTwiceEnumerator(list, 1).Join(enumerator);

		while (enumerator.MoveNext()) {
		}

		CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4, 5 }, list);
	}

	[Test]
	public void updateSideEffectsCorrectly() {
		var list = new List<int>();

		var enumerator = actAfterEnumerator(list, 1).Join(actAfterEnumerator(list, 3));

		CollectionAssert.AreEqual(new List<int>(), list);

		UnityEngine.Assertions.Assert.IsTrue(enumerator.MoveNext());

		CollectionAssert.AreEqual(new List<int> { 1 }, list);

		UnityEngine.Assertions.Assert.IsTrue(enumerator.MoveNext());

		CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, list);

		UnityEngine.Assertions.Assert.IsFalse(enumerator.MoveNext());

		CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4 }, list);
	}

	[Test]
	public void assertByOrderCorrectly() {
		var list = new List<int> { 1 };

		var enumerator = assertEnumerator(list, 1).Join(assertEnumerator(list, 2));

		list.Clear();
		UnityEngine.Assertions.Assert.IsTrue(enumerator.MoveNext());
		list.Add(1);

		UnityEngine.Assertions.Assert.IsTrue(enumerator.MoveNext());
		list.Add(2);

		UnityEngine.Assertions.Assert.IsFalse(enumerator.MoveNext());
	}

	[Test]
	public void assertEnumeration() {
		var list = new List<int>();

		var enumerator = simpleEnumerator(list, 1).Join(assertEnumerator(list, 2));

		CollectionAssert.AreEqual(new List<int>(), list);

		UnityEngine.Assertions.Assert.IsTrue(enumerator.MoveNext());

		CollectionAssert.AreEqual(new List<int> { 1 }, list);

		UnityEngine.Assertions.Assert.IsTrue(enumerator.MoveNext());
		list.Add(2);

		UnityEngine.Assertions.Assert.IsFalse(enumerator.MoveNext());
	}
}
