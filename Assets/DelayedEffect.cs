using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class DelayedEffect {
	public float Delay;
	public Action<Game> Callback;
}
