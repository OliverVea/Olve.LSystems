namespace Olve.LSystems.Turtle.Readers;

public class TurtleReaderConfiguration
{
    public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;
    public char InstructionDelimiter { get; set; } = ',';
    public char ArgumentListStart { get; set; } = '(';
    public char ArgumentListEnd { get; set; } = ')';
    public char ArgumentDelimiter { get; set; } = ',';
}