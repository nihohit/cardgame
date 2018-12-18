using System.Collections.Generic;


public class Location : BaseValueClass {
	public string Name { get; }
	public IEnumerable<LocationContent> Content { get; }

	public Location(string name, 
		IEnumerable<LocationContent> content) {
		Name = name;
		Content = content;
	}
}