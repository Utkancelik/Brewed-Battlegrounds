using System;
using System.Collections.Generic;
using UnityEngine;

/*
public class DIContainer
{
    private static DIContainer _instance;
    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public static DIContainer Instance => _instance ??= new DIContainer();

    public void Register<T>(T service)
    {
        _services[typeof(T)] = service;
    }

    public T Resolve<T>()
    {
        return (T)_services[typeof(T)];
    }
}
*/