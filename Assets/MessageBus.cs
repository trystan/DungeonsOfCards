using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Subscription {
	MessageBus messageBus;
	Type type;
	object callback;

	public Subscription(MessageBus messageBus, Type type, object callback) {
		this.messageBus = messageBus;
		this.type = type;
		this.callback = callback;
	}

	public void Remove() {
		messageBus.Off(type, callback);
	}
}

public class MessageBus {
	private Dictionary<Type,List<object>> callbacks = new Dictionary<Type, List<object>>();

	public Subscription On<T>(Action<T> callback) {
		if (!callbacks.ContainsKey(typeof(T)))
			callbacks[typeof(T)] = new List<object>();
		callbacks[typeof(T)].Add(callback);

		return new Subscription(this, typeof(T), callback);
	}

	public void Off(Type type, object callback) {
		if (callbacks.ContainsKey(type))
			callbacks[type].Remove(callback);
	}

	public void Off<T>(Action<T> callback) {
		if (callbacks.ContainsKey(typeof(T)))
			callbacks[typeof(T)].Remove(callback);
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