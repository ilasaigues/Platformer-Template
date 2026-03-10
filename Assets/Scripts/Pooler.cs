
using System;
using System.Collections.Generic;

public class Pooler<T>
{
    public Stack<T> Available = new();
    public List<T> InUse = new();


    private readonly Func<T> _factoryMethod;
    private readonly Action<T> _prepareMethod;
    private readonly Action<T> _releaseMethod;

    public Pooler(Func<T> factoryMethod, Action<T> prepareMethod, Action<T> releaseMethod)
    {
        _factoryMethod = factoryMethod;
        _prepareMethod = prepareMethod;
        _releaseMethod = releaseMethod;
    }


    public T GetElement()
    {
        T element;
        if (Available.Count > 0)
        {
            element = Available.Pop();
        }
        else
        {
            element = _factoryMethod();
        }
        InUse.Add(element);
        _prepareMethod(element);
        return element;
    }

    public void Release(T element)
    {
        InUse.Remove(element);
        Available.Push(element);
        _releaseMethod(element);
    }

}