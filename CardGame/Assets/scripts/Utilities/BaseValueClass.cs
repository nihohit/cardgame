// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class ValueClassExtensions {
	public static int GetHashCodeFromProperties(this object obj) {
		return Hasher.GetHashCode(obj
			.GetType()
			.GetProperties()
			.Select(propertyInfo => propertyInfo.GetValue(obj))
			.ToArray());
	}

	public static bool EqualityFromProperties(this object obj, object other) {
		if (other == null) {
			return false;
		}
		var type = obj.GetType();
		return (other.GetType().Equals(type)) &&
			type.GetProperties()
				.All(info => {
					var thisValue = info.GetValue(obj);
					var otherValue = info.GetValue(other);
					return thisValue == otherValue ||
						(thisValue != null && thisValue.Equals(otherValue));
				});
	}

	public static string ToStringFromProperties(this object obj) {
		var stringBuilder = new StringBuilder();
		var type = obj.GetType();
		stringBuilder.AppendLine($"{type}:");
		foreach (var property in type.GetProperties()) {
			stringBuilder.AppendLine($"{property.Name}: {property.GetValue(obj)}");
		}
		return stringBuilder.ToString();
	}

	public static T ToObject<T>(this IDictionary<string, object> source) where T : class {
		var constructor = typeof(T).GetConstructors().First(constructorInfo => constructorInfo.GetParameters().Length == source.Count);
		var caseInsensitiveDictionary = new Dictionary<string, object>(source, StringComparer.OrdinalIgnoreCase);

		var constructorArguments = constructor.GetParameters()
			.Select(parameter => caseInsensitiveDictionary[parameter.Name])
			.ToArray();

		return (T)constructor.Invoke(constructorArguments);
	}

	public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance) {
		return source.GetType().GetProperties(bindingAttr).ToDictionary(
				propInfo => propInfo.Name,
				propInfo => propInfo.GetValue(source, null)
		);
	}

	public static T SetValue<T>(this T source, string propertyName, object propertyValue) where T : class {
		var dictionary = source.AsDictionary();
		dictionary[propertyName] = propertyValue;
		return dictionary.ToObject<T>();
	}
}

public class BaseValueClass {
	public override bool Equals(object obj) {
		return this.EqualityFromProperties(obj);
	}

	public override int GetHashCode() {
		return this.GetHashCodeFromProperties();
	}

	public override string ToString() {
		return this.ToStringFromProperties();
	}

	public T ShallowClone<T>() {
		AssertUtils.AreEqual(typeof(T), GetType());
		return (T)MemberwiseClone();
	}
}
