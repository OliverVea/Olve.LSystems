using System.Collections.Frozen;
using Olve.Utilities.Types.Results;

namespace Olve.LSystems.Turtle.Readers;

public class TurtleProgramReader(IEnumerable<ITurtleInstructionReader> readers)
{
    private class StringEqualityComparer(StringComparison stringComparison) : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            return string.Compare(x, y, stringComparison) == 0;
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
    public Result<IEnumerable<TurtleInstruction>> ReadTurtleProgram(string turtleProgram, TurtleReaderConfiguration configuration)
    {
        IEqualityComparer<string> comparer = new StringEqualityComparer(configuration.StringComparison);
        
        var readerLookup = readers.ToFrozenDictionary(x => x.CommandName, x => x, comparer);
        
        var instructionStrings = turtleProgram.Split(configuration.InstructionDelimiter,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        var results = instructionStrings.Select(x => ReadInstruction(x, readerLookup, configuration));

        if (results.TryPickProblems(out var problems, out var instructions))
        {
            return problems.Prepend(new ResultProblem("Failed to read turtle program '{0}'", turtleProgram));
        }

        return Result.Success(instructions);
    }

    private Result<TurtleInstruction> ReadInstruction(
        string instructionInvocation,
        IReadOnlyDictionary<string, ITurtleInstructionReader> readerLookup,
        TurtleReaderConfiguration configuration)
    {
        var nameResult = InstructionHelper.GetName(instructionInvocation, configuration);
        if (nameResult.TryPickProblems(out var problems, out var instructionName))
        {
            return problems.Prepend(new ResultProblem("Failed to get instruction name from instruction invocation '{0}'", instructionInvocation));
        }

        if (!readerLookup.TryGetValue(instructionName, out var reader))
        {
            return new ResultProblem("Could not read instruction with name '{0}' for instruction invocation '{1}'", instructionName, instructionInvocation);
        }

        var readResult = reader.ReadInstruction(instructionInvocation, configuration);
        if (readResult.TryPickProblems(out problems, out var instruction))
        {
            return problems.Prepend(new ResultProblem("Failed reading instruction with name '{0}' and invocation '{1}'", instructionName, instructionInvocation));
        }

        return instruction;
    }
}