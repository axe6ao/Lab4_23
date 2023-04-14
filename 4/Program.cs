using System;

namespace EventDrivenWorkflow
{
    class Program
    {
        static void Main(string[] args)
        {
            var workflow = new Workflow();

            workflow.ActionCompleted += (sender, e) =>
            {
                Console.WriteLine($"Action {e.ActionName} completed successfully.");
            };

            workflow.ActionFailed += (sender, e) =>
            {
                Console.WriteLine($"Action {e.ActionName} failed with error: {e.ErrorMessage}.");
            };

            workflow.Start();

            Console.ReadKey();
        }
    }

    class Workflow
    {
        private readonly Action[] _actions;
        private int _currentIndex;

        public event EventHandler<ActionEventArgs> ActionCompleted;
        public event EventHandler<ActionEventArgs> ActionFailed;

        public Workflow()
        {
            _actions = new Action[]
            {
                new Action("Action 1", 2000),
                new Action("Action 2", 3000),
                new Action("Action 3", 4000),
                new Action("Action 4", 5000)
            };
        }

        public void Start()
        {
            _currentIndex = 0;
            NextAction();
        }

        private void NextAction()
        {
            if (_currentIndex < _actions.Length)
            {
                var action = _actions[_currentIndex];
                action.Execute();

                action.ActionCompleted += (sender, e) =>
                {
                    OnActionCompleted(action.Name);
                    _currentIndex++;
                    NextAction();
                };

                action.ActionFailed += (sender, e) =>
                {
                    OnActionFailed(action.Name, e.ErrorMessage);
                };
            }
            else
            {
                Console.WriteLine("Workflow completed.");
            }
        }

        protected virtual void OnActionCompleted(string actionName)
        {
            ActionCompleted?.Invoke(this, new ActionEventArgs(actionName));
        }

        protected virtual void OnActionFailed(string actionName, string errorMessage)
        {
            ActionFailed?.Invoke(this, new ActionEventArgs(actionName, errorMessage));
        }
    }

    class Action
    {
        private readonly string _name;
        private readonly int _duration;

        public event EventHandler<ActionEventArgs> ActionCompleted;
        public event EventHandler<ActionEventArgs> ActionFailed;

        public string Name => _name;

        public Action(string name, int duration)
        {
            _name = name;
            _duration = duration;
        }

        public void Execute()
        {
            Console.WriteLine($"Executing action {_name}...");

            try
            {
                System.Threading.Thread.Sleep(_duration);
                OnActionCompleted();
            }
            catch (Exception ex)
            {
                OnActionFailed(ex.Message);
            }
        }

        protected virtual void OnActionCompleted()
        {
            ActionCompleted?.Invoke(this, new ActionEventArgs(_name));
        }

        protected virtual void OnActionFailed(string errorMessage)
        {
            ActionFailed?.Invoke(this, new ActionEventArgs(_name, errorMessage));
        }
    }

    class ActionEventArgs : EventArgs
    {
        public string ActionName { get; }
        public string ErrorMessage { get; }

        public ActionEventArgs(string actionName)
        {
            ActionName = actionName;
        }

        public ActionEventArgs(string actionName, string errorMessage)
        {
            ActionName = actionName;
            ErrorMessage = errorMessage;
        }
    }
}
