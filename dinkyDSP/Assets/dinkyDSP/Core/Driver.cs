using System;
using System.Collections.Generic;

/*
 * Gareth Williams
 * 
 * Driver is the entry point into the synthAPI, it requests samples from the root element of the audio graph
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{		
	public class Driver
	{
		public static event EventHandler PreSampleGenerated;
		
		public IAudioNode rootNode;
		public static int sampleRate = 48000; //unity default as of 4.01f
		public static int channels = 2; //unity default as of 4.01f
		
		public Driver ()
		{
#if SERVICE_MODE
			UnityEngine.Debug.Log("new Driver");
#endif
		}
		
		public void GenerateSamples(ref float[] buffer)
		{
			if(rootNode == null)
			{
				return;	
			}
			
			int blockSize = buffer.Length;
			for( int sample = 0; sample < blockSize; sample++)
			{
				if(PreSampleGenerated != null)
				{
					PreSampleGenerated(this, EventArgs.Empty);	
				}
				
				buffer[sample] = (float)rootNode.GetSample();
			}
		}
	}
}

