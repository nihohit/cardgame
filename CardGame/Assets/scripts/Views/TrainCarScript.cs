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
				return 31;
			case CarType.Engine:
				return 4;
			case CarType.General:
				return 23;
			case CarType.Workhouse:
				return 21;
			case CarType.Armory:
				return 15;
			case CarType.Refinery:
				return 28;
			case CarType.LivingQuarters:
				return 17;
			case CarType.None:
			case CarType.Test:
				break;
		}
		
		AssertUtils.UnreachableCode();
		return -1;
	}

	// Use this for initialization
	void Awake () {
		lock (staticLock) {
			if (sprites == null) {
				sprites = Resources.LoadAll<Sprite>("Sprites/trainSprites");
			}
		}
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
}
