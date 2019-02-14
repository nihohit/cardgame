// Copyright (c) 2019 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour {
	private Subject<GameObject> touchedGameObject = new Subject<GameObject>();
	
	private void Update() {
		if (!hasTouch()) {
			return;
		}

		var touchPosition = toWorldPosition(touchLocation());
		sendObjectAtWorldLocation(touchPosition);
	}

	public IObservable<GameObject> TouchedGameObject => touchedGameObject;

	private bool hasTouch() {
#if (!UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID))
		return Input.touchCount == 1 &&
			Input.GetTouch(0).phase == TouchPhase.Ended &&
			!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#else
		return Input.GetMouseButtonDown(0) &&
			!EventSystem.current.IsPointerOverGameObject();
#endif
	}

	private Vector3 touchLocation() {
#if (!UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID))
		return Input.GetTouch(0).position;
#else
		return Input.mousePosition;
#endif
	}

	private Vector3 toWorldPosition(Vector3 touchPosition) {
		touchPosition.z = 10;
		return Camera.main.ScreenToWorldPoint(touchPosition);
	}

	private void sendObjectAtWorldLocation(Vector3 worldLocation) {
		var hits = Physics2D.RaycastAll(
			new Vector2(worldLocation.x, worldLocation.y), 
			Vector2.zero, 
			Mathf.Infinity);
		if (hits.Length == 0) {
			return;
		}
		var firstHit = hits
			.OrderBy(hit => hit.collider.gameObject.layer)
			.Reverse()
			.First();
		if (firstHit.collider != null) {
			touchedGameObject.OnNext(firstHit.collider.gameObject);
		}
	}
}
