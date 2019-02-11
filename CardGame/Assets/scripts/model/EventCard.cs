using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EventCard : BaseValueClass {
  public string Name { get; set; }
	public string Description { get; set; }
	public IEnumerable<Card> Options { get; set; }
}