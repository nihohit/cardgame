using System;
using System.Collections;
using UnityEngine;

public static class GameObjectExtensions {
  public static IEnumerator MoveOverSeconds(this GameObject objectToMove, Vector3 end, float seconds, Action continuation) {
    float elapsedTime = 0;
    Vector3 startingPos = objectToMove.transform.position;
    while (elapsedTime < seconds) {
      objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
      elapsedTime += Time.deltaTime;
      yield return new WaitForEndOfFrame();
    }
    objectToMove.transform.position = end;
    continuation();
  }

  public static IEnumerator MoveOverSpeed(this GameObject objectToMove, Vector3 end, float speed, Action continuation) {
    // speed should be 1 unit per second
    while (objectToMove.transform.position != end) {
      objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
      yield return new WaitForEndOfFrame();
    }

    continuation();
  }
}
