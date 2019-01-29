using System.Text;

public class StoryEventLogger : IStoryEventLogger {
  private readonly StringBuilder turnStringBuilder = new StringBuilder();
  private readonly StringBuilder totalLog = new StringBuilder();

  public void EndedTurn() {
    throw new System.NotImplementedException();
  }

  private void passTurnLogToTotalLog() {
    totalLog.AppendLine(turnStringBuilder.ToString());
    turnStringBuilder.Clear();
  }

  public void EnteredLocation(Location location) {

  }

  private string locationContentDescription(LocationContent content) {
		switch (content) {
			case LocationContent.Test:
				break;
			case LocationContent.TrainWreck:
				return "wrecked train";
			case LocationContent.Howitizer:
				return "howitzer";
			case LocationContent.Armory:
				return "armory";
			case LocationContent.Workhouse:
				return "old workshop";
			case LocationContent.OldHouses:
				return "old houses";
			case LocationContent.FuelRefinery:
				return "refinery";
			case LocationContent.Woods:
				return "woods";
			case LocationContent.WildAnimals:
				return "wild animals";
			case LocationContent.LivingPeople:
				return "small settlement";
			case LocationContent.FuelStorage:
				return "fuel storage";
			case LocationContent.Storehouse:
				return "storehouse";
			case LocationContent.Mine:
				return "mines";
		}

		return "";
	}

  public void FacedEvent(EventCard eventCard, Card conclusion) {
  }

  public string GetCurrentTurnLog() {
    return turnStringBuilder.ToString();
  }

  public string GetTotalLog() {
    if (turnStringBuilder.Length > 1) {
      passTurnLogToTotalLog();
    }

    return totalLog.ToString();
  }

  private readonly static string[] lostPopulationDescriptions = new []{
      "Some people couldn't face the hardship and left.",
      "Some population left you for lack of materials",
      "There wasn't enough food. Some left. Some died. The train moved on.",
      "The hunger consumed us.",
      "There wasn't enough food. We had some hard decisions to make, but those that remained standing were still hopeful",
    };

  public void LostPopulation() {
    turnStringBuilder.AppendLine(lostPopulationDescriptions.ChooseRandomValue());
  }

  public void PlayedCard(Card card) {
  }
}