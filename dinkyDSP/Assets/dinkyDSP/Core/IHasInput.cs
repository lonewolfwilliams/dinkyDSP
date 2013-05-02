using System;

/*
 * Gareth Williams
 * 
 * some audio nodes take an input
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public interface IHasInput
	{
		IAudioNode InputNode {get;set;}
	}
}

