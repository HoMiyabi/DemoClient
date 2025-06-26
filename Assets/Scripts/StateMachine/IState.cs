namespace Kirara
{
    public interface IState
    {
        public void OnEnter();
        public void OnExit();
        public void Update();
    }
}