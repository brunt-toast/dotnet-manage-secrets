using System.CommandLine;

internal abstract class Flag : Option<bool>
{
  protected Flag(string name, params string[] aliases) : base(name, aliases)
  {
  }
}