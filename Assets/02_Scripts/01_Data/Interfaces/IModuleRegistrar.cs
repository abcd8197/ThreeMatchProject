using System;
namespace CodeJay.Module
{
    // In 키워드는 Main.
    public interface IModuleRegistrar<in T> where T : IModule
    {
        public void Register(T module);
        public Type GetHandlerType() => typeof(T);
    }
}
