using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneManager : MonoBehaviour {
  public GameObject CardPrefab;

  private System.Random random = new System.Random();
  private DeckScript deck;
  private DeckScript discardPile;
  private CardsState _cards;
  private CardsState cards { get {
      return _cards;
    }  set {
      _cards = value;
      deck.SetCardNumber(value.CurrentDeck.Count());
      discardPile.SetCardNumber(value.DiscardPile.Count());
    }
  }

  // Use this for initialization
  void Start () {
    deck = GameObject.Find("Deck").GetComponent<DeckScript>();
    discardPile = GameObject.Find("Discard Pile").GetComponent<DeckScript>();
    cards = CardsState.NewState(new[] {
      new Card("foo"),
      new Card("bar"),
      new Card("baz")
    }).ShuffleCurrentDeck(random);
  }
	
	// Update is called once per frame
	void Update () {
		
	}
}
