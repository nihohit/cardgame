using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EventCard {
  public string Name { get; set; }
  public Card Option1 { get; set; }
  public Card Option2 { get; set; }
  public Card Default { get; set; }

  public EventCard ShallowClone() {
    // TODO - verify that no internal card is modified, otherwise it will affect every clone.
		return (EventCard)MemberwiseClone();
	}
}