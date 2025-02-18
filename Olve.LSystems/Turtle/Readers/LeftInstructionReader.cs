using Olve.Utilities.Types.Results;

namespace Olve.LSystems.Turtle.Readers;

public class LeftInstructionReader : ITurtleInstructionReader
{
    public string CommandName => "Left";
    
    public Result<TurtleInstruction> ReadInstruction(string instructionInvocation, TurtleReaderConfiguration configuration)
    {
        var argumentResult = InstructionHelper.GetArguments(instructionInvocation, configuration);
        if (argumentResult.TryPickProblems(out var problems, out var arguments))
        {
            return problems.Prepend(new ResultProblem("Could not get arguments"));
        }

        if (arguments.Length != 1)
        {
            return new ResultProblem("Expected a single instruction argument, got '{0}'", arguments.Length);
        }

        var argument = arguments.Single();
        if (!float.TryParse(argument, out var angle))
        {
            return new ResultProblem("'{0}' is not a valid number", angle);
        }

        Turtle.Turn instruction = new(angle);
        return (TurtleInstruction) instruction;
    }
}