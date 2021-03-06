﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;

namespace FEM_NET.Examples
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false);
            app.Name = "femnet";
            app.HelpOption("-?|-h|--help");

            var meshArg = app.Argument("mesh", "Path to the mesh file");

            var elementTypeOption = app.Option("-e|--element-type", "Finite element type (default: P1)", CommandOptionType.SingleValue);
            var accuracyOption = app.Option("-a|--accuracy", "Accuracy of CG method (default: 1e-6)", CommandOptionType.SingleValue);

            app.OnExecute(() => {
                if (meshArg.Value == null)
                {
                    Console.WriteLine("ERROR: Missing path to the mesh file.");
                    return 1;
                }
                var meshPath = meshArg.Value;
                if (meshPath.EndsWith(".mesh"))
                    meshPath = meshPath.Substring(0, meshPath.Length - 5);

                var feType = elementTypeOption.HasValue() ? elementTypeOption.Value().ToLowerInvariant() : "p1";
                double accuracy = accuracyOption.HasValue() ? double.Parse(accuracyOption.Value()) : 1e-6;
                // TODO: Format error.

                Console.WriteLine("\nSolving...\n");

                var stdWriter = Console.Out;
                using (var fileWriter = File.CreateText($"{meshPath}.log"))
                using (var mirrorWriter = new MirrorWriter(stdWriter, fileWriter))
                {
                    Console.SetOut(mirrorWriter);
                    HomogeneousStationaryHeatProgram.Run(meshPath, feType, accuracy);
                }
                
                Console.SetOut(stdWriter);
                Console.WriteLine("\nPress ENTER to exit...");
                Console.ReadLine();
                return 0;
            });

            try
            {
                app.Execute(args);
            }
            catch (CommandParsingException exception)
            {
                Console.WriteLine(exception.Message);
            }
            catch (FileNotFoundException exception)
            {
                Console.WriteLine($"FILE NOT FOUND: {exception.Message}");
            }
            catch (DirectoryNotFoundException exception)
            {
                Console.WriteLine($"DIRECTORY NOT FOUND: {exception.Message}");
            }
        }
    }
}
