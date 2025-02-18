using Olve.Utilities.Types.Results;

namespace Olve.LSystems.Turtle.Readers;

public class PushInstructionReader : ITurtleInstructionReader
{
    public string CommandName => "Push";
    
    public Result<TurtleInstruction> ReadInstruction(string instructionInvocation, TurtleReaderConfiguration configuration)
    {
        Turtle.Push instruction = new();
        return (TurtleInstruction) instruction;
    }
}