using System;
using System.Windows.Forms;

namespace TipTop
{
  static class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      ApplicationConfiguration.Initialize();
      if (args.Length > 0 && args[0].StartsWith("--demo-out"))
      {
        string dir = args[0].Contains('=')
          ? args[0].Substring(args[0].IndexOf('=') + 1)
          : (args.Length > 1 ? args[1] : "images");
        Environment.ExitCode = DemoShots.Run(dir);
        return;
      }
      Application.Run(new Form1());
    }
  }
}
