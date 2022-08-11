using UnityEngine;
using UnityEngine.Events;


public class GameEventListener : MonoBehaviour
{
    [SerializeField] private GameEvent _gameEvent;
    [SerializeField] private UnityEvent _unityEvent;


    void Awake() => _gameEvent.Register(this);


    void OnDestroy() => _gameEvent.Unregister(this);


    public void RaiseEvent() => _unityEvent.Invoke();
}