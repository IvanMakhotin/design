using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    class HelpCommand : BaseCommand
    {
        private readonly Lazy<ICommand[]> arguments;

        public HelpCommand(Lazy<ICommand[]> arguments)
        {
            this.arguments = arguments;
        }

        public override void Execute()
        {
            foreach (var argument in arguments.Value)
                Console.WriteLine(argument);
        }
    }
}