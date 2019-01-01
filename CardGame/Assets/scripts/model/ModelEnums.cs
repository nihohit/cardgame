public enum TraditionType { None, Test, BaseTraditions }

public enum CarType { None, Test, Engine, General, Workhouse, Armory, Refinery, Cannon, LivingQuarters }

public enum CardHandlingMode { Regular, Event, Replace, Exhaust }

//TODO consider making into flags
public enum LocationContent {
	Test, 
	GeneralCarComponents, 
	CannonCarComponents, 
	ArmoryCarComponents, 
	WorkhouseCarComponents, 
	EngineCarComponents, 
	LivingQuartersCarComponents, 
	RefineryCarComponents,
	Woods,
	WildAnimals,
	LivingPeople,
	FuelStorage,
	SpareMaterials,
	MinableMaterials,
}

public enum Dangers {
	Avalanche,
	Robbers,
	WildAnimalsAttack,
	
}