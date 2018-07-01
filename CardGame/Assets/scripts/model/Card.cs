using System.Collections.Generic;
using UnityEngine;

public class Card {
  public string Name { get; private set; }

  public Card(string name) {
    Name = name;
  }

  public override string ToString() {
    return string.Format("name: {0}", Name);
  }

  public override bool Equals(object obj) {
    var card = obj as Card;
    return card != null &&
           Name.Equals(card.Name);
  }

  public override int GetHashCode() {
    return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
  }
}
