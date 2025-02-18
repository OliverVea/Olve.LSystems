using Olve.Utilities.Types.Results;

namespace Olve.LSystems.Turtle.Readers;

public static class InstructionHelper
{
    public static Result<string> GetName(string instructionInvocation, TurtleReaderConfiguration configuration)
    {
        var end = instructionInvocation.IndexOf(configuration.ArgumentListStart, configuration.StringComparison);

        end = end == -1 ? instructionInvocation.Length : end;

        return instructionInvocation[..end];
    }
    
    public static Result<string[]> GetArguments(string instructionInvocation, TurtleReaderConfiguration configuration)
    {
        int openCount = 0, closeCount = 0;
        foreach (var ch in instructionInvocation)
        {
            if (ch == configuration.ArgumentListStart) openCount++;
            if (ch == configuration.ArgumentListEnd) closeCount++;
        }

        if (openCount == 0 && closeCount == 0) return Array.Empty<string>();

        if (openCount != 1 || closeCount != 1)
        {
            return new ResultProblem(
                "Expected either 0 of '{0}' and '{1}' or 1 of both, got '{2}' of '{0}' and '{3}' of '{1}'",
                configuration.ArgumentListStart, configuration.ArgumentListEnd, openCount, closeCount);
        }

        var start = instructionInvocation.IndexOf(configuration.ArgumentListStart, configuration.StringComparison) + 1;
        var end = instructionInvocation.IndexOf(configuration.ArgumentListEnd, configuration.StringComparison);

        if (start > end)
        {
            return new ResultProblem("Expected {0} before {1} but was opposite",
                configuration.ArgumentListStart, configuration.ArgumentListEnd);
        }

        return instructionInvocation[start..end]
            .Split(configuration.ArgumentDelimiter, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }
}