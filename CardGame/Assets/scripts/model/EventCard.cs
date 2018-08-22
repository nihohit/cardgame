using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EventCard {
  public string Name { get; set; }
  public IEnumerable<Card> Options { get; set; }

  public EventCard ShallowClone() {
    // TODO - verify that no internal card is modified, otherwise it will affect every clone.
		return (EventCard)MemberwiseClone();
	}
}