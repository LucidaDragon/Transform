using System;
using System.IO;
using System.Text;
using Transform.Compiler;

namespace TestBed
{
	class Program
	{
		static void Main(string[] args)
		{
			var source = @"
namespace HelloWorld
{
	struct Vector3
	{
		float X;
		float Y;
		float Z;

		float GetLength()
		{
			return Standard.Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
		}
	}

	void main(int argc, char** argv)
	{
		Standard.Print(""Hello World!"");
	}
}";
			var start = Environment.TickCount;

			var emit = new Transform.Emitters.C();
			var packages = new PackageCompiler() { Reflection = false };
			var pkg = new Package(emit.GetContext());
			//pkg.AddArrayType();
			//pkg.AddReflectionTypes();
			pkg.AddStandardLibrary();
			pkg.AddSource(new MemoryStream(Encoding.UTF8.GetBytes(source)));
			packages.Add(pkg);
			packages.Emit(emit);

			var output = new FileStream("./main.c", FileMode.Create);

			string name = "stdout";
			emit.WriteOutput(0, output, ref name);

			output.Close();

			Console.WriteLine($"Compiled in {Environment.TickCount - start}ms.");

			//Console.ReadKey(true);
		}
	}
}
