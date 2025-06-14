using System;

[AttributeUsage(AttributeTargets.Class)]
public class ExternalAttribute : Attribute
{
    public string GameObjectName { get; }
    public ExternalAttribute(string name) => GameObjectName = name;
}