using System;
using System.Collections.Generic;
using System.CommandLine;
using System.ComponentModel.Design;
using System.Text;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Commands;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Ioc;
using Microsoft.Extensions.DependencyInjection;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Tests.Generators.Symbols;

internal static class SymbolGenerator
{
    public static IEnumerable<object[]> GetSymbols()
    {
        IServiceCollection sc = new ServiceCollection();
        CliServiceRegistrar.RegisterServices(sc);
        IServiceProvider sp = sc.BuildServiceProvider();
        RootCommand root = sp.GetRequiredService<ManageSecretsRootCommand>();

        Stack<Symbol> stack = new Stack<Symbol>();
        stack.Push(root);

        while (stack.Any())
        {
            Symbol cur = stack.Pop();

            if (cur is Command cmd)
            {
                foreach (var child in cmd.Children)
                {
                    stack.Push(child);
                }

                foreach (var child in cmd.Subcommands)
                {
                    stack.Push(child);
                }

                foreach (var child in cmd.Arguments)
                {
                    stack.Push(child);
                }

                foreach (var child in cmd.Options)
                {
                    stack.Push(child);
                }
            }

            yield return [cur];
        }
    }
}
