// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
}
