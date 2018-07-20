using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Card {
  public string Name { get; set; }
	public int GoldCost { get; set; }
	public int IndustryCost { get; set; }
	public int PopulationCost { get; set; }
	public int GoldGain { get; set; }
	public int IndustryGain { get; set; }
	public int PopulationGain { get; set; }

	public override string ToString() {
		var stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(Name);
		addString(stringBuilder, GoldCost, "Gold cost");
		addString(stringBuilder, IndustryCost, "Industry cost");
		addString(stringBuilder, GoldCost, "Population cost");
		addString(stringBuilder, GoldGain, "Gold gain");
		addString(stringBuilder, IndustryGain, "Industry gain");
		addString(stringBuilder, PopulationGain, "Population gain");
		return stringBuilder.ToString();
	}

	private void addString(StringBuilder builder, int propertyValue, string propertyDescription) {
		if (propertyValue != 0) {
			builder.AppendLine($"{propertyDescription}: {propertyValue}");
		}
	}
}
