using System;

/*
 * Gareth Williams
 * 
 * some AudioNodes can have their frequency set
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public interface IHasPitch
	{
		float Frequency {get; set;}
	}
}

