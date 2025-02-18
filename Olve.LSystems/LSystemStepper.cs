using System.Text;

namespace Olve.LSystems;

public static class LSystemStepper
{
    public static string Step(string currentState, LSystemModel model)
    {
        var nextState = new StringBuilder(currentState.Length);
        
        foreach (var symbol in currentState)
        {
            if (model.Rules.TryGetValue(symbol, out var replacement))
            {
                nextState.Append(replacement);
                continue;
            }
            
            nextState.Append(symbol);
        }

        return nextState.ToString();
    }
}