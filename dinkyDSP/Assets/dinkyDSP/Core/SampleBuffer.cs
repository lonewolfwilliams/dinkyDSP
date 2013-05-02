using System;

/*
 * Gareth Williams
 * 
 * dataModel to abstract buffered audio from audioclip instances in unityEngine to com.lonewolfwilliams.dinkyDSP namespace
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class SampleBuffer
	{
		public float[] buffer;
		public int channels;
		public int sampleRate;
		public float fundamentalPitch;
	}
}

