using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorExitBehaviour : StateMachineBehaviour
{
	private Actor actor;
	public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
	{
		if(actor == null)
		{
			actor = animator.transform.GetComponent<Actor>();
		}
		actor.OnStateMachineExit();
	}
}
