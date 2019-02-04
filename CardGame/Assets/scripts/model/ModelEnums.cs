public enum TraditionType { None, Test, BaseTraditions }

public enum CarType { None, Test, Engine, General, Workhouse, Armory, Refinery, Cannon, LivingQuarters, CommandCenter }

public enum CardHandlingMode { Regular, Event, Discard, Exhaust }

//TODO consider making into flags
public enum LocationContent {
	Test, 
	Howitizer, 
	Armory, 
	Workhouse, 
	TrainWreck, 
	OldHouses, 
	FuelRefinery,
	Woods,
	WildAnimals,
	LivingPeople,
	FuelStorage,
	Storehouse,
	Mine,
	ArmyBase,
}

public enum Dangers {
	Avalanche,
	Robbers,
	WildAnimalsAttack,
}

public static class ModelGlobal {
	public static string CarName(CarType carType) {
		switch (carType) {
			case CarType.Engine:
				return "Engine";
			case CarType.General:
				return "Basic";
			case CarType.Workhouse:
				return "Workhouse";
			case CarType.Armory:
				return "Armory";
			case CarType.Refinery:
				return "Refinery";
			case CarType.Cannon:
				return "Cannon";
			case CarType.LivingQuarters:
				return "Housing";
			case CarType.CommandCenter:
				return "Command";
		}

		AssertUtils.UnreachableCode($"unknown type {carType}");
		return "";
	}
}