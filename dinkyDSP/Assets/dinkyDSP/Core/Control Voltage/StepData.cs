using System;

/*
 * Gareth Williams
 * 
 * step data is used by the step sequencer to store a note and octave
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public struct StepData
	{
		public string note;
		public int octave;
		
		public StepData(string note, int octave)
		{
			this.note = note;
			this.octave = octave;
		}
	}
}

