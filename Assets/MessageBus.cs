using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class MessageBus {
	private Dictionary<Type,List<object>> callbacks = new Dictionary<Type, List<object>>();

	public void On<T>(Action<T> callback) {
		if (!callbacks.ContainsKey(typeof(T)))
			callbacks[typeof(T)] = new List<object>();
		callbacks[typeof(T)].Add(callback);
	}

	private List<Action> toHandle = new List<Action>();
	private bool isHandling;
	public void Send<T>(T message) {
		toHandle.Add(() => {
			if (!callbacks.ContainsKey(typeof(T)))
				return;
			
			foreach (var c in callbacks[typeof(T)])
				(c as Action<T>)(message);
		});

		if (isHandling)
			return;

		isHandling = true;
		while (toHandle.Any()) {
			var handle = toHandle[0];
			toHandle.RemoveAt(0);
			handle();
		}
		isHandling = false;
	}
}