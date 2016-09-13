using System;
using System.Threading;
using Microsoft.Samples.Debugging.CorDebug;
using Microsoft.Samples.Debugging.CorDebug.NativeApi;
using Mono.Debugging.Evaluation;

namespace Mono.Debugging.Win32
{
	class CorMethodCall: AsyncOperation
	{
		readonly CorEvaluationContext context;
		readonly CorFunction function;
		readonly CorType[] typeArgs;
		readonly CorValue[] args;

		public ManualResetEvent DoneEvent = new ManualResetEvent (false);

		readonly CorEval eval;

		public bool IsException { get; private set; }

		public CorMethodCall (CorEvaluationContext context, CorFunction function, CorType[] typeArgs, CorValue[] args)
		{
			this.context = context;
			this.function = function;
			this.typeArgs = typeArgs;
			this.args = args;
			eval = context.Eval;
			IsException = false;
		}

		void ProcessOnEvalComplete (object sender, CorEvalEventArgs evalArgs)
		{
			DoProcessEvalFinished (evalArgs);
		}

		void ProcessOnEvalException (object sender, CorEvalEventArgs evalArgs)
		{
			IsException = true;
			DoProcessEvalFinished (evalArgs);
		}

		void DoProcessEvalFinished (CorEvalEventArgs evalArgs)
		{
			UnSubcribeOnEvals ();

			context.Session.OnEndEvaluating ();
			// TODO: check that evalArgs.Eval == this.eval
			//exception = eargs.Eval.Result;
			DoneEvent.Set ();
			evalArgs.Continue = false;
		}

		void SubscribeOnEvals ()
		{
			context.Session.Process.OnEvalComplete += ProcessOnEvalComplete;
			context.Session.Process.OnEvalException += ProcessOnEvalException;
		}

		void UnSubcribeOnEvals ()
		{
			context.Session.Process.OnEvalComplete -= ProcessOnEvalComplete;
			context.Session.Process.OnEvalException -= ProcessOnEvalException;
		}

		public override string Description
		{
			get
			{
				var met = function.GetMethodInfo (context.Session);
				if (met == null)
					return "<Unknown>";
				if (met.DeclaringType == null)
					return met.Name;
				return met.DeclaringType.FullName + "." + met.Name;
			}
		}

		public override void Invoke ( )
		{
			SubscribeOnEvals ();

			if (function.GetMethodInfo (context.Session).Name == ".ctor")
				eval.NewParameterizedObject (function, typeArgs, args);
			else
				eval.CallParameterizedFunction (function, typeArgs, args);
			context.Session.Process.SetAllThreadsDebugState (CorDebugThreadState.THREAD_SUSPEND, context.Thread);
			context.Session.ClearEvalStatus ();
			context.Session.OnStartEvaluating ();
			context.Session.Process.Continue (false);
		}

		public override void Abort ( )
		{
			UnSubcribeOnEvals ();
			eval.Abort ();
		}

		public override void Shutdown ( )
		{
			try {
				Abort ();
			}
			catch (Exception e) {
			}
			DoneEvent.Set ();
		}

		public override bool WaitForCompleted (int timeout)
		{
			return DoneEvent.WaitOne (timeout, false);
		}

		public CorValue Result
		{
			get
			{
				return eval.Result;
			}
		}
	}
}
