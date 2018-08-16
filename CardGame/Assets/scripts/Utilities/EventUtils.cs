// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

public static class EventUtils {
	public static void LogEventCardPlayed(Card playedCard, string eventCardName, string state, IEnumerable<Card> hand) {
		logEvent("Event_Played", eventDictionary(eventCardName, state, hand, new Dictionary<string, object> {
				{"played_card", playedCard.Name}
			}));
	}

	public static void LogStartTurnEvent(string eventCardName, string state, IEnumerable<Card> hand) {
		logEvent("Start_Turn", eventDictionary(eventCardName, state, hand, new Dictionary<string, object> {}));
	}

	public static void LogEndTurnEvent(IEnumerable<Card> playedCards, string eventCardName, string state, IEnumerable<Card> hand) {
		logEvent("End_Turn", eventDictionary(eventCardName, state, hand, new Dictionary<string, object> {
				{"played_cards", cardsString(playedCards) }
			}));
	}

	private static void logEvent(string eventName, Dictionary<string, object> eventDictionary) {
		var result = Analytics.CustomEvent(eventName, eventDictionary);
		var printedDictionary = string.Join("\n", eventDictionary);
		switch (result) {
			case AnalyticsResult.Ok:
				Debug.Log($"Sent {eventName} event with content {printedDictionary}");
				break;
			default:
				Debug.Log($"Received {result} when sending {eventName} event with content {printedDictionary}");
				break;
		}
	}

	private static string cardsString(IEnumerable<Card> cards) {
		return string.Join(", ", cards.Select(card => card.Name).OrderBy(name => name));
	}

	private static Dictionary<string, object> eventDictionary(string eventCardName, string state, IEnumerable<Card> hand, Dictionary<string, object> additionalInfo) {
		return new Dictionary<string, object> {
				{ "state", state },
				{ "hand", cardsString(hand) },
				{ "event", eventCardName },
		}.Union(additionalInfo).ToDictionary(pair => pair.Key, pair => pair.Value);
	}
}
