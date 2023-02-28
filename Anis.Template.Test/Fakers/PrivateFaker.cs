using System;
using Bogus;

namespace Anis.Template.Test.Fakers
{
    public class PrivateFaker<T> : Faker<T> where T : class
    {
        public PrivateFaker<T> UsePrivateConstructor()
        {
            return (PrivateFaker<T>)base.CustomInstantiator(f => Activator.CreateInstance(typeof(T), nonPublic: true) as T ?? throw new InvalidOperationException());
        }
    }
}
