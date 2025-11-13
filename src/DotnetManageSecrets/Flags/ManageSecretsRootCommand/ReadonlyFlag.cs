internal class ReadonlyFlag : Flag
{
    public ReadonlyFlag() : base("--readonly", "-r")
    {
        Description = """
                      Format the secrets and send them to standard output; don't launch the editor. 
                      """;
    }
}