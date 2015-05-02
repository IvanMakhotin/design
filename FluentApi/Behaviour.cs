using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentTask
{
    class Behavior
    {
        private Queue<Action> actions; 
        public Behavior()
        {
            actions = new Queue<Action>();
        }

        public Behavior(IEnumerable<Action> actions)
        {
            actions = new Queue<Action>(actions);
        }

        public Behavior Say(string message)
        {
            actions.Enqueue(new Action(() => Console.WriteLine(message)));
            return this;
        }

        public Behavior Delay(TimeSpan timeSpan)
        {
            Action newAction = () => Thread.Sleep(timeSpan);
            actions.Enqueue(newAction);
            return this;
        }


        public Behavior UntilKeyPressed(Func<Behavior, Behavior> inner)
        {
            Action newAction = () =>
            {
                Behavior innerBehaviour = inner(new Behavior());

                while (!Console.KeyAvailable)
                {
                    innerBehaviour.Execute();
                }
                Console.ReadKey();
            };

            actions.Enqueue(newAction);
            return this;
        }

        public Behavior Jump(JumpHeight jump)
        {
            return this;
        }

        public void Execute()
        {
            foreach (var action in actions) action.Invoke();
        }
    }
}
