using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUtils : Singleton<HUtils> {

	public override void Init() {

	}

	protected override void Shutdown() {

	}

	public static bool LayerInMask(int mask, int layer) {
		return mask == (mask | (1 << layer));
	}

	public static bool EnumAnyInMask(int mask, int enumValues) {
		return (mask & enumValues) > 0;
	}

	public static bool EnumAllInMask(int mask, int enumValues) {
		return mask == (mask | enumValues);
	}

}