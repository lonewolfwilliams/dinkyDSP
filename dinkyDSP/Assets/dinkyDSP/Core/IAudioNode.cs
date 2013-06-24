using System;

/*
 * Gareth Williams
 * 
 * The com.lonewolfwilliams.dinkyDSP is composed of a graph (linked list) of IAudioNodes that provide a sample
 * into each following graph item
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public delegate void SampleEventHandler(double sample);
	public interface IAudioNode
	{
		event SampleEventHandler SampleGenerated;
		double GetSample();
	}
}

