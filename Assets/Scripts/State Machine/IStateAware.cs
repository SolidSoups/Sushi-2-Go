namespace State_Machine
{
  public interface IStateAware
  {
    void OnEnterState(State state);
    void OnExitState(State state);
  }
}