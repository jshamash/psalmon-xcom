using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Funding {
	
	public static string[] continents = new string[] {"Africa"
													, "Asia"
													, "Australia"
													, "Europe"
													, "North America"
													, "South America"};
	
	public static int[] fundPerContinent = new int[] {2500
													, 3200
													, 1000
													, 4300
													, 3300
													, 1500};
	
	private static int BASEBONUS = 500;
	
	private static int MAXPANIC = 5;
	
	// Max Level 5(Continent leave council)
	Dictionary<string, int> panicLevel;
	
	public Funding() {
		panicLevel = new Dictionary<string, int>();
		
		foreach(string iS in continents) {
			panicLevel.Add(iS, 0);	
		}
	}
	
	public int getSpecialRequest() {
		return 0;
	}
	
	public void addPanicLevel(string continent) {
		if (!panicLevel.ContainsKey(continent)) {
			panicLevel.Add(continent, 0);
		}
		if(panicLevel[continent] < MAXPANIC) {
			panicLevel[continent] += 1;
		}
	}
	
	public void removePanicLevel(string continent) {
		if( panicLevel[continent] > 0 ) {
			panicLevel[continent] -= 1;
		}
	}
	
	public int getPanicLevel(string continent) {
		return panicLevel[continent];
	}
	
	public int getMonthlyFund(int numBases) {
		int fund = 0;
		for(int i = 0; i < continents.Length; i++) {
			if ( panicLevel[continents[i]] < MAXPANIC ) {
				fund += fundPerContinent[i];
			}
		}
		fund += numBases * BASEBONUS;
		
		return fund;
	}
}