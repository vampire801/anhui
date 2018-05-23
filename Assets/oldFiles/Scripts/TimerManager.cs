using System;
using System.Collections.Generic;
using System.Linq;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class TimerManager
{
    private static Dictionary<int, MobaTimer> _timers = new Dictionary<int, MobaTimer>();

    private static int timerID = 0;

    public static void AddTimer(MobaTimer timer)
    {
        timer.globalID = timerID;
        _timers.Add(timer.globalID, timer);
        timerID++;
    }


    public void Tick(float dt)
    {
        foreach (MobaTimer timer in _timers.Values.ToArray())
        {
            if (timer.end)
            {
                _timers.Remove(timer.globalID);
            }
            else
            {
                timer.Tick(dt);
            }
        }
    }
}

public class MobaTimer
{
    private float _elapsedTime;

    private Action _action;

    private Action _cancelAction;

    private float _delay;

    private bool _end = false;

    private bool _repeat = false;

    public bool end
    {
        get
        {
            return _end;
        }
    }

    public int globalID;

    public static void Invoke(Action action, Action cancelAction, float delay)
    {
        MobaTimer timer = new MobaTimer(action, cancelAction, delay);
        TimerManager.AddTimer(timer);
    }


    public static void InvokeRepeat(Action action, Action cancelAction, float delay)
    {
        MobaTimer timer = new MobaTimer(action, cancelAction, delay, true);
        TimerManager.AddTimer(timer);
    }
    public static void CancelInvoke(Action action,Action cancelAction)
    {
        
    }
    private MobaTimer(Action action, Action cancelAction, float delay, bool repeat = false)
    {
        _action = action;
        _delay = delay;

        _repeat = repeat;

        _cancelAction = cancelAction;

        _cancelAction += OnCancel;
    }

    private void OnCancel()
    {
        _end = true;
    }

    public void Tick(float dt)
    {
        if (_elapsedTime >= _delay)
        {
            if (_repeat)
            {
                _elapsedTime = _elapsedTime - _delay;
            }
            else
            {
                _end = true;
            }

            if (_action != null)
                _action();
            if (_cancelAction !=null )
                _cancelAction();
        }
        _elapsedTime += dt;
    }
}