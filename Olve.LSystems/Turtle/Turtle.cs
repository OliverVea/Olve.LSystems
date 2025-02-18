using OneOf;

namespace Olve.LSystems.Turtle;

public static class Turtle
{
    public readonly record struct Point(float X, float Y);
    public readonly record struct Position(Point Origin, float Angle);
    public readonly record struct LineSegment(Point From, Point To);

    // Instructions
    public readonly record struct Move(float Distance, bool Draw = true);
    public readonly record struct Turn(float Angle);
    public readonly record struct Set(Position Position);
    public readonly record struct Reset;
    public readonly record struct Push;
    public readonly record struct Pop;


    
    public static IEnumerable<LineSegment> RunInstructions(IEnumerable<TurtleInstruction> instructions, Position initialPosition = default)
    {
        var position = initialPosition;
        Stack<Position> positionStack = new();

        foreach (var instruction in instructions)
        {
            LineSegment? lineSegment = null;
            var currentPosition = position;
            
            position = instruction.Match(
                move =>
                {
                    var from = currentPosition.Origin;
                    var to = new Point(
                        from.X + move.Distance * float.Cos(currentPosition.Angle),
                        from.Y + move.Distance * float.Sin(currentPosition.Angle));
                    
                    if (move.Draw) lineSegment = new LineSegment(from, to);     
                    
                    return currentPosition with
                    {
                        Origin = to
                    };
                },
                turn => currentPosition with
                {
                    Angle = currentPosition.Angle + turn.Angle
                },
                set => set.Position,
                _ => initialPosition,
                _ =>
                {
                    positionStack.Push(currentPosition);
                    return currentPosition;
                },
                _ => positionStack.Pop()
            );

            if (lineSegment.HasValue) yield return lineSegment.Value;
        }
    }
}


[GenerateOneOf]
public partial class TurtleInstruction : OneOfBase<Turtle.Move, Turtle.Turn, Turtle.Set, Turtle.Reset, Turtle.Push, Turtle.Pop>;