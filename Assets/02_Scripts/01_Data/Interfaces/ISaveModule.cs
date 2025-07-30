namespace CodeJay.Module
{
    public interface ISaveModule : IModule
    {
        public void Initialize(SaveData saveData);
        public void Save(SaveData saveData);
    }
}
