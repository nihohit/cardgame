using System.Collections.Generic;
using System.Linq;

public class Location : BaseValueClass {
	public string Name { get; }
	public IEnumerable<LocationContent> Content { get; }
	public IEnumerable<Dangers> Dangers { get; }
	public string StoryEvent { get; }

	public Location(string name, 
		IEnumerable<LocationContent> content,
		IEnumerable<Dangers> dangers = null,
		string storyEvent = null) {
		Name = name;
		Content = content.ToList();
		Dangers = (dangers ?? Enumerable.Empty<Dangers>()).ToList();
		StoryEvent = storyEvent;
	}
}