using System;
using System.Collections.Generic;

/*
 * Gareth Williams
 * 
 * defines common data for the com.lonewolfwilliams.dinkyDSP core
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public static class Common
	{
		public enum noteDuration
		{
			whole,
			half,
			quarter,
			eighth,
			sixteenth,
			wholeTriplet,
			halfTriplet,
			quarterTriplet,
			eightTriplet,
			sixteenthTriplet
		}
		
		//http://www.phy.mtu.edu/~suits/notefreqs.html
		static Dictionary<string, float> m_noteToFrequency = null;
		public static Dictionary<string, float> noteToFrequency
		{
			get
			{
				//(one octave)
				if(m_noteToFrequency == null)
				{
					m_noteToFrequency = new Dictionary<string, float>();	
					m_noteToFrequency.Add("C", 	32.70f); //octave 1
					m_noteToFrequency.Add("C#", 34.65f);
					m_noteToFrequency.Add("D", 	36.71f);
					m_noteToFrequency.Add("D#", 38.89f);
					m_noteToFrequency.Add("E", 	41.20f);
					m_noteToFrequency.Add("F", 	43.65f);
					m_noteToFrequency.Add("F#", 46.25f);
					m_noteToFrequency.Add("G", 	49.00f);
					m_noteToFrequency.Add("G#", 51.91f);
					m_noteToFrequency.Add("A", 	55.00f);
					m_noteToFrequency.Add("A#", 58.27f);
					m_noteToFrequency.Add("B", 	61.74f);
				}
				
				return m_noteToFrequency;
			}
		}
		
		public static string rest = "-";
		
		//major scales from the circle of fifths
		//http://www.your-personal-singing-guide.com/images/Major-Scales.jpg
		public static string[] cMajor = new string[]{"C", "D", "E", "F", "G", "A", "B"};
		public static string[] gMajor = new string[]{"G", "A", "B", "C", "D", "E", "F#"};
		public static string[] dMajor = new string[]{"D", "E", "F#", "G", "A", "B", "C#"};
		public static string[] aMajor = new string[]{"A", "B", "C#", "D", "E", "F#", "G#"};
		public static string[] eMajor = new string[]{"E", "F#", "G#", "A", "B", "C#", "D#"};
		public static string[] bMajor = new string[]{"B", "C#", "D#", "E", "F#", "G#", "A#"};
		
		public static string[] fSharpMajor = new string[]{"F#", "G#", "A#", "B", "C#", "D#", "E#"};
		//public string[] gFlatMajor = new string[]{"F#", "G#", "A#", "B", "C#", "D#", "E#"}; //homogenous
		
		public static string[] dFlatMajor = new string[]{"C#", "D#", "E#", "F#", "G#", "A#", "B#"};
		public static string[] aFlatMajor = new string[]{"B#", "A#", "C", "C#", "D#", "F", "G"};
		public static string[] eFlatMajor = new string[]{"D#", "F", "G", "B#", "A#", "C", "D"};
		public static string[] bFlatMajor = new string[]{"A#", "C", "D", "D#", "F", "G", "A"};
		public static string[] fMajor = new string[]{"F", "G", "A", "A#", "C", "D", "E"};
		
		//minor scales from the minor (inner) circle of fifths
		public static string[] aMinor = new string[]{"A", "B", "C", "D", "E", "F", "G"};
		public static string[] eMinor = new string[]{"E", "F#", "G", "A", "B", "C", "D"};
		public static string[] bMinor = new string[]{"B", "C#", "D", "E", "F#", "G", "A"};
		public static string[] fSharpMinor = new string[]{"F#", "G#", "A", "B", "C#", "D", "E"};
		public static string[] cSharpMinor = new string[]{"C#", "D#", "E", "F#", "G#", "A", "B"};
		public static string[] gSharpMinor = new string[]{"G#", "A#", "B", "C#", "D#", "E", "F#"};
		
		public static string[] dSharpMinor = new string[]{"D#", "E#", "F#", "G#", "A#", "B", "C#"};
		//public string[] eFlatMinor = new string[]{"D#", "E#", "F#", "G#", "A#", "B", "C#"}; //homogenous
		
		public static string[] bFlatMinor = new string[]{"A#", "B#", "C#", "D#", "E#", "F#", "G#"};
		public static string[] fMinor = new string[]{"F", "G", "G#", "A#", "C", "C#", "D#"};
		public static string[] cMinor = new string[]{"C", "D", "D#", "F", "G", "G#", "A#"};
		public static string[] gMinor = new string[]{"G", "A", "A#", "C", "D", "D#", "F"};
		public static string[] dMinor = new string[]{"D", "E", "F", "G", "A", "A#", "C"};
	}
}

