using Olve.Utilities.Types.Results;

namespace Olve.LSystems.Turtle.Readers;

public interface ITurtleInstructionReader
{
    string CommandName { get; }
    Result<TurtleInstruction> ReadInstruction(string instructionInvocation, TurtleReaderConfiguration configuration);
}