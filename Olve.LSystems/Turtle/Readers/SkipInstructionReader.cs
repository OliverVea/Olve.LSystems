using Olve.Utilities.Types.Results;

namespace Olve.LSystems.Turtle.Readers;

public class SkipInstructionReader : ITurtleInstructionReader
{
    public string CommandName => "Skip";
    
    public Result<TurtleInstruction> ReadInstruction(string instructionInvocation, TurtleReaderConfiguration configuration)
    {
        var argumentResult = InstructionHelper.GetArguments(instructionInvocation, configuration);
        if (argumentResult.TryPickProblems(out var problems, out var arguments))
        {
            return problems;
        }

        if (arguments.Length != 1)
        {
            return new ResultProblem("Expected a single instruction argument, got '{0}'", arguments.Length);
        }

        var argument = arguments.Single();
        if (!float.TryParse(argument, out var distance))
        {
            return new ResultProblem("'{0}' is not a valid number", distance);
        }

        Turtle.Move instruction = new(distance, false);
        return (TurtleInstruction) instruction;
    }
}