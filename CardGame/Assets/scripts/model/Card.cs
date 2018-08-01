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
	public DeckType AddDeck { get; set; }
	public bool Exhaustible { get; set; }

	public override string ToString() {
		var stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(Name);
		//addString(stringBuilder, GoldCost, "Gold-");
		//addString(stringBuilder, IndustryCost, "Ind-");
		//addString(stringBuilder, PopulationCost, "Pop-");
		//addString(stringBuilder, GoldGain, "Gold+");
		//addString(stringBuilder, IndustryGain, "Ind+");
		//addString(stringBuilder, PopulationGain, "Pop+");
		if (AddDeck != DeckType.None) {
			stringBuilder.AppendLine($"Add deck: {AddDeck}");
		}
		if (Exhaustible) {
			stringBuilder.AppendLine("Exhaustible");
		}
		return stringBuilder.ToString();
	}

	private void addString(StringBuilder builder, int propertyValue, string propertyDescription) {
		addString(builder, propertyValue != 0 ? propertyValue.ToString() : null, propertyDescription);
	}

	private void addString(StringBuilder builder, string str, string propertyDescription) {
		if (str != null) {
			builder.AppendLine($"{propertyDescription}: {str}");
		}
	}

	public Card ShallowClone() {
		return (Card)MemberwiseClone();
	}

	public override bool Equals(object obj) {
		var card = obj as Card;
		return card != null &&
			card.GetType() == this.GetType() &&
			card.Name.Equals(Name) &&
			card.GoldCost == GoldCost &&
			card.IndustryCost == IndustryCost &&
			card.PopulationCost == PopulationCost &&
			card.GoldGain == GoldGain &&
			card.IndustryGain == IndustryGain &&
			card.PopulationGain == PopulationGain &&
			card.AddDeck == AddDeck &&
			card.Exhaustible == Exhaustible;
	}
}
