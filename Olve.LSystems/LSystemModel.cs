using System.Text.Json.Serialization;

namespace Olve.LSystems;

public class LSystemModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>['0', '1']</example>
    [JsonRequired]
    public List<char> Variables { get; set; } = [];
    
    /// <summary>
    /// 
    /// </summary>
    /// <example>['[', ']']     </example>
    [JsonRequired]
    public List<char> Constants { get; set; } = [];
    
    /// <summary>
    /// 
    /// </summary>
    /// <example>"0"</example>
    [JsonRequired]
    public string Axiom { get; set; } = string.Empty;
    
    /// <summary>
    /// 
    /// </summary>
    /// <example>{{'1': "11", '0': "1[0]0"}</example>
    [JsonRequired]
    public Dictionary<char, string> Rules { get; set; } = new();
    
    /// <summary>
    /// 
    /// </summary>
    /// <example>{'0': "Draw(1)", '1': "Draw(1)", '[': "Push, Left(1.57)", ']': "Pop, Right(1.57)"}</example>
    [JsonRequired]
    public Dictionary<char, string> DrawRules { get; set; } = new();
}