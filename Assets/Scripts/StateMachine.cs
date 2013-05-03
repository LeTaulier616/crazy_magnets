using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class State
{
	protected StateMachine machine;

	public void SetStateMachine(StateMachine m)
	{
		this.machine = m;
	}

	virtual public void EnterState(GameObject it)
	{
	}

	virtual public void UpdateState(GameObject it)
	{
	}

	virtual public void ExitState(GameObject it)
	{
	}
}

public class StateMachine : MonoBehaviour
{
	public State curState { get; private set; }

	void Start()
	{
		this.curState = null;
	}

	void Update()
	{
		if (this.curState != null)
			this.curState.UpdateState(this.gameObject);
	}

	public void SwitchState(State state)
	{
		if (this.curState != null)
			this.curState.ExitState(this.gameObject);
		if (state != null)
		{
			state.SetStateMachine(this);
			state.EnterState(this.gameObject);
		}
		this.curState = state;
		Debug.Log(this.ToString() + " Switch to " + state.ToString());
	}
}