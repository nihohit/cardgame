using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCarScript : MonoBehaviour {
	private static readonly object staticLock = new object();
	private static Sprite[] sprites;
	private SpriteRenderer spriteRenderer;
	private TrainCar car;
	public TrainCar Car {
		get {
			return car;
		}
		set {
			car = value;
			setImageForCar();
		}
	}

	private void setImageForCar() {
		spriteRenderer.sprite = sprites[carIndex()];
	}

	private int carIndex() {
		switch(car.Type) {
			case CarType.Cannon:
				return 7;
			case CarType.Engine:
				return 0;
			case CarType.General:
				return 6;
			case CarType.Workhouse:
				return 3;
			case CarType.Armory:
				return 2;
			case CarType.Refinery:
				return 8;
			case CarType.LivingQuarters:
				return 1;
			case CarType.None:
			case CarType.Test:
				break;
		}
		
		AssertUtils.UnreachableCode($"Unexpected type: {car.Type}");
		return -1;
	}

	// Use this for initialization
	void Awake () {
		lock (staticLock) {
			if (sprites == null) {
				sprites = Resources.LoadAll<Sprite>("Sprites/atlas");
			}
		}
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
}
