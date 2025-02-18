using System.Text.Json;
using System.Text.Json.Serialization;
using Olve.LSystems;
using Olve.LSystems.Graphics;
using Olve.LSystems.Turtle;
using Olve.LSystems.Turtle.Readers;
using Olve.Utilities.Types.Results;

var result = Run(args);

if (result.TryPickProblems(out var problems))
{
    foreach (var problem in problems)
    {
        Console.Write("[ERROR] ");
        Console.WriteLine(problem.ToString());
    }

    return 1;
}

return 0;

static Result Run(string[] args)
{
    args = ["/home/oliver/RiderProjects/Olve.LSystems/Olve.LSystems/Systems/FractalPlant.json"];
    
    if (args.Length != 1)
    {
        return new ResultProblem(
            "Expected a single commandline argument with a path to the LSystem json file to render. got '{0}' arguments",
            args.Length);
    }

    var systemModelResult = ReadSystemModelFromFile(args.Single());
    if (systemModelResult.TryPickProblems(out var problems, out var systemModel))
    {
        return problems.Prepend(new ResultProblem("Failed to read LSystem file"));
    }

    const int steps = 6;

    var result = StepSystem(systemModel, steps);

    var turtleProgramReader = new TurtleProgramReader([
        new DrawInstructionReader(), new LeftInstructionReader(), new PopInstructionReader(),
        new PushInstructionReader(), new RightInstructionReader(), new SkipInstructionReader()
    ]);

    TurtleReaderConfiguration configuration = new();

    var turtleInstructions = result
        .Select(x => systemModel.DrawRules.TryGetValue(x, out var instruction) ? instruction : string.Empty)
        .Where(x => !string.IsNullOrWhiteSpace(x));

    var turtleProgram = string.Join(configuration.InstructionDelimiter, turtleInstructions);

    var instructionResult = turtleProgramReader.ReadTurtleProgram(turtleProgram, configuration);
    if (instructionResult.TryPickProblems(out problems, out var instructions))
    {
        return problems.Prepend(new ResultProblem("Could not read turtle instructions"));
    }

    var lineSegments = Turtle.RunInstructions(instructions).ToArray();

    var points = lineSegments.SelectMany(x => new[] { x.To, x.From }).Distinct().ToArray();
    var pointLookup = points.Index().ToDictionary(x => x.Item, x => x.Index);
    var indices = lineSegments.SelectMany(x => new[] { (uint)pointLookup[x.From], (uint)pointLookup[x.To] }).ToArray();

    Application application = new();
    
    application.Run(points, indices);

    return Result.Success();
}

static Result<LSystemModel> ReadSystemModelFromFile(string filePath)
{
    string fileContent;
    
    try
    {
        fileContent = File.ReadAllText(filePath);
    }
    catch (Exception exception)
    {
        return new ResultProblem(exception, "Tried to read LSystem at '{0}' but got exception", filePath);
    }

    LSystemModel? systemModel;

    var context = new ProjectJsonSerializerContext(new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });
    
    try
    {
        systemModel =
            JsonSerializer.Deserialize<LSystemModel>(fileContent, context.LSystemModel);
    }
    catch (Exception exception)
    {
        return new ResultProblem(exception, "Tried to parse the LSystem at '{0}' as json but got exception", filePath);
    }

    if (systemModel is null)
    {
        return new ResultProblem("Did not succeed in parsing the LSystem at '{0}' as json", filePath);
    }

    return systemModel;
}

static string StepSystem(LSystemModel systemModel, int steps)
{
    var state = systemModel.Axiom;
    
    for (var i = 0; i < steps; i++)
    {
        state = LSystemStepper.Step(state, systemModel);
    }

    return state;
}

[JsonSerializable(typeof(LSystemModel))]
public partial class ProjectJsonSerializerContext : JsonSerializerContext;