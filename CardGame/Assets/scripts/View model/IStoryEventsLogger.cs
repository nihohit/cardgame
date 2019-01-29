public interface IStoryEventLogger {
  string GetTotalLog();

  string GetCurrentTurnLog();

  void EnteredLocation(Location location);

  void LostPopulation();

  void PlayedCard(Card card);

  void FacedEvent(EventCard eventCard, Card conclusion);

  void EndedTurn();
}