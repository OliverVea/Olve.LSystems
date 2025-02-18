using Olve.Utilities.Types.Results;

namespace Olve.LSystems.Turtle.Readers;

public class PopInstructionReader : ITurtleInstructionReader
{
    public string CommandName => "Pop";
    
    public Result<TurtleInstruction> ReadInstruction(string instructionInvocation, TurtleReaderConfiguration configuration)
    {
        Turtle.Pop instruction = new();
        return (TurtleInstruction) instruction;
    }
}