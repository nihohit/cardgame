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
	public int NumberOfTimesPlayed { get; set; }

	public override string ToString() {
		var stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(Name);
		addString(stringBuilder, GoldCost, "Gold-");
		addString(stringBuilder, IndustryCost, "Ind-");
		addString(stringBuilder, GoldCost, "Pop-");
		addString(stringBuilder, GoldGain, "Gold+");
		addString(stringBuilder, IndustryGain, "Ind+");
		addString(stringBuilder, PopulationGain, "Pop+");
		return stringBuilder.ToString();
	}

	private void addString(StringBuilder builder, int propertyValue, string propertyDescription) {
		if (propertyValue != 0) {
			builder.AppendLine($"{propertyDescription}: {propertyValue}");
		}
	}
}
