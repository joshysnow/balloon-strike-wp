namespace GameCore
{
    public interface Serializable
    {
        bool Activate(bool instancePreserved);
        void Deactivate();
    }
}