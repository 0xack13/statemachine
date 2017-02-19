using System;
using System.Collections.Generic;
 
namespace StateMachine
{
    public enum ProcessState
    {
        Inactive,
        Active,
        Paused,
        Terminated
    }
 
    public enum Command
    {
        Begin,
        End,
        Pause,
        Resume,
        Exit
    }
 
    public class Process
    {
        class StateTransition
        {
            readonly ProcessState current;
            readonly Command command;
 
            public StateTransition(ProcessState state, Command command)
            {
                this.current = state;
                this.command = command;
            }
 
            public override int GetHashCode()
            {
                return 17 + 31 * this.current.GetHashCode() + 31 * this.command.GetHashCode();
            }
 
            public override bool Equals(object obj)
            {
                StateTransition other = (StateTransition)obj;
                return other != null && this.current == other.current && this.command == other.command;
            }
        }
 
        Dictionary<StateTransition, ProcessState> transitions;
        public ProcessState CurrentState { get; private set; }
 
        public Process()
        {
            CurrentState = ProcessState.Inactive;
            transitions = new Dictionary<StateTransition, ProcessState>
            {
                { new StateTransition(ProcessState.Inactive, Command.Exit), ProcessState.Terminated },
                { new StateTransition(ProcessState.Inactive, Command.Begin), ProcessState.Active },
                { new StateTransition(ProcessState.Active, Command.End), ProcessState.Inactive },
                { new StateTransition(ProcessState.Active, Command.Pause), ProcessState.Paused },
                { new StateTransition(ProcessState.Paused, Command.End), ProcessState.Inactive },
                { new StateTransition(ProcessState.Paused, Command.Resume), ProcessState.Active }
            };
        }
 
        public ProcessState getNext(Command command)
        {
            StateTransition transition = new StateTransition(CurrentState, command);
            ProcessState nextState;
 
            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
             
            return nextState;
        }
 
        public ProcessState moveNext(Command command)
        {
            CurrentState = getNext(command);
            return CurrentState;
        }
    }
 
 
    public class Run
    {
        static void Main(string[] args)
        {
            Process process = new Process();
            Console.WriteLine("Current State = " + process.CurrentState);
            Console.WriteLine("Command.Begin: Current State = " + process.moveNext(Command.Begin));
            Console.WriteLine("Command.Pause: Current State = " + process.moveNext(Command.Pause));
            Console.WriteLine("Command.End: Current State = " + process.moveNext(Command.End));
            Console.WriteLine("Command.Exit: Current State = " + process.moveNext(Command.Exit));
            Console.ReadLine();
        }
    }
}
