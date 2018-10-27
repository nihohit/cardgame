using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainViewScript : MonoBehaviour {

	private IReadOnlyList<TrainCarScript> carViews;

	// Use this for initialization
	void Awake () {
		carViews = GetComponentsInChildren<TrainCarScript>().ToList();
	}
	
	public void setTrain(IReadOnlyList<TrainCar> cars) {
		AssertUtils.EqualOrGreater(carViews.Count, cars.Count);

		for (int i = 0; i < cars.Count; i++) {
			carViews[i].Car = cars[i];
			carViews[i].gameObject.SetActive(true);
		}

		for (int i = cars.Count; i < carViews.Count; i++) {
			carViews[i].gameObject.SetActive(false);
		}
	}
}
