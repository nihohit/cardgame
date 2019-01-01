using System.Collections.Generic;
using System.Linq;

public class Location : BaseValueClass {
	public string Name { get; }
	public IEnumerable<LocationContent> Content { get; }
	public IEnumerable<Dangers> Dangers { get; }

	public Location(string name, 
		IEnumerable<LocationContent> content,
		IEnumerable<Dangers> dangers = null) {
		Name = name;
		Content = content;
		Dangers = dangers ?? Enumerable.Empty<Dangers>();
	}
}