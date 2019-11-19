using CommandLine;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using PlasticNotificationSystem.TriggerEvents;

namespace PlasticNotificationSystem
{
    public class Options
    {
        [Option('t', "trigger", Required =true, HelpText ="Trigger that we are bound to")]
        public string TriggerType
        {
            get;
            set;
        }

        [Option('o', "output", Required = true, HelpText = "Output to send to")]
        public string OutputType
        {
            get;
            set;
        }

        [Option('c', "config", Required = false, Default ="config.json", HelpText ="Path to a json config file.  Defaults to config.json")]
        public string ConfigFile
        {
            get;
            set;
        }
    }


    class Program
    {
        static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {                                   
            ConfigureLogger();
            Logger.Info("Starting with: " + string.Join(' ', args));
            AppDomain.CurrentDomain.UnhandledException += (object Sender, UnhandledExceptionEventArgs args) =>
            {
                Exception ex = args.ExceptionObject as Exception;
                Logger.Error(ex.ToString() );
            };

            INotifier Notifier = null;
            ITriggerEvent Trigger = null;
            string Config = "config.json";

            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>((Options opt) =>
            {
                Type NotifierType = Type.GetType(opt.OutputType);
                Notifier = (INotifier)Activator.CreateInstance(NotifierType);

                Type TriggerType = Type.GetType(opt.TriggerType);

                Trigger = (ITriggerEvent)Activator.CreateInstance(TriggerType);

                Config = opt.ConfigFile;

            });

            //Load the config for this notifier
            string JsonText = "";
            if (Config != "" && File.Exists(Config))
            {
                JsonText = File.ReadAllText(Config);
            }
            Notifier.ParseJsonString(JsonText);
            Logger.Info("Parsed Config: \n{0}", JsonText);

            //Read everything from stdin
            string TriggerText = ReadTriggerTextFromStdIn();

            Logger.Info("Loaded Trigger: \n{0}", TriggerText);

            //Execute the trigger
            Trigger.Parse(TriggerText);
            Notifier.NotifyTrigger(Trigger);

            Logger.Info("Finished executing Trigger");
            NLog.LogManager.Flush();
        }

        private static string ReadTriggerTextFromStdIn()
        {
            StringBuilder Builder = new StringBuilder();
            string Line = Console.ReadLine();
            while (Line != null)
            {
                Builder.AppendLine(Line.Trim());
                Line = Console.ReadLine();
            }

            return Builder.ToString().Trim();
        }
             

        private static void ConfigureLogger()
        {
            var Config = new NLog.Config.LoggingConfiguration();
            var LogConsole = new NLog.Targets.ConsoleTarget("LogConsole");
            var FileTarget = new NLog.Targets.FileTarget("File")
            {
                FileName = "logs/log.txt",
                //Layout = "${message}"
            };

            Config.AddRuleForAllLevels(LogConsole);
            Config.AddRuleForAllLevels(FileTarget);

            NLog.LogManager.Configuration = Config;

        }
    }
}
