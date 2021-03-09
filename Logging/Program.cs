

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Logging.EventLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Logging
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select logger provider \n1. Event logger\n2. Debug logger");
            string selectLogger = Console.ReadLine();
            ILoggerProvider provider = selectLogger switch
            {
                "1" => new EventLogLoggerProvider(),
                "2" => new DebugLoggerProvider(),
                _ => new DebugLoggerProvider(),
            };
            var a = new A();
            var b = new B();
            Console.WriteLine(a.Calculate(1_100_100, 2_200_200));
            Console.WriteLine(a.GetElapsedTicks(1_100_100, 2_200_200));
            a.Logging(1_100_100, 2_200_200, provider);
            Console.WriteLine(b.Calculate(1_100_100, 2_200_200));
            Console.WriteLine(b.GetElapsedTicks(1_100_100, 2_200_200));
            b.Logging(1_100_100, 2_200_200, provider);
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Class for an example of using the interface IAlgorithm.
    /// </summary>
    public class A: IAlgorithm
    {
        public int Calculate(int first, int second)
        {
            return first + second;
        }
    }

    /// <summary>
    /// Class for an example of using the interface IAlgorithm.
    /// </summary>
    public class B : IAlgorithm
    {
        public int Calculate(int first, int second)
        {
            double sum = 1;
            for (int i = 0; i < first * second; i++)
            {
                sum += Math.Pow(second, first);
            }
            return (int)sum;
        }
    }

    /// <summary>      
    /// Provides templates for some algorithm calculating.      
    /// </summary>      
    public interface IAlgorithm
    {
        int Calculate(int first, int second);
    }

    /// <summary>
    /// Class extending interface IAlgorithm.
    /// </summary>
    public static class IAlgorithmExtension
    {
        /// <summary>
        /// Method that returns the number of ticks for which the method is executed.
        /// </summary>
        /// <param name="algorithm">Object that implements the interface IAlgorithm.</param>
        /// <param name="first">First Int32 number.</param>
        /// <param name="second">Second Int32 number.</param>
        /// <returns>The number of ticks for which the method is executed.</returns>
        public static long GetElapsedTicks(this IAlgorithm algorithm, int first, int second)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            algorithm.Calculate(first, second);
            sw.Stop();
            return sw.ElapsedTicks;
        }

        /// <summary>
        /// Method that logging interface method Calculate.
        /// </summary>
        /// <param name="algorithm">Object that implements the interface IAlgorithm.</param>
        /// <param name="first">First Int32 number.</param>
        /// <param name="second">Second Int32 number.</param>
        /// <param name="provider">Logger provider.</param>
        public static void Logging(this IAlgorithm algorithm, int first, int second, ILoggerProvider provider)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(provider);
            ILogger logger = loggerFactory.CreateLogger<IAlgorithm>();
            var message = new StringBuilder();
            logger.LogDebug($"DateTime: {DateTime.Now}, first: {first}, second: {second}");
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int result = algorithm.Calculate(first, second);
                sw.Stop();
                logger.LogInformation($"result: {result}, lead time: {sw.ElapsedTicks} ticks");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
    }
}
