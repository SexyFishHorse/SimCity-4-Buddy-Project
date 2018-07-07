namespace Nihei.SC4Buddy.Application.Control
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using log4net;
    using Nihei.SC4Buddy.Application.Models;
    using Nihei.SC4Buddy.Configuration;
    using Nihei.SC4Buddy.Model;

    public class GameArgumentsHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Regex ResolutionRegEx = new Regex(@"\d+x\d+");

        public string GetArgumentString(UserFolder selectedUserFolder)
        {
            var arguments = new List<string>();
            arguments.AddRange(GetAudioArguments());
            arguments.AddRange(GetVideoArguments());
            arguments.AddRange(GetPerformanceArguments());
            arguments.AddRange(GetOtherArguments());

            if (selectedUserFolder != null)
            {
                arguments.Add($"-userDir:\"{selectedUserFolder.FolderPath}\\\"");
            }

            return string.Join(" ", arguments.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());
        }

        private static IEnumerable<string> GetAudioArguments()
        {
            var output = new Collection<string>
                             {
                                 $"-audio:{(LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableAudio) ? "off" : "on")}",
                                 $"-music:{(LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableMusic) ? "off" : "on")}",
                                 $"-sounds:{(LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableSounds) ? "off" : "on")}"
                             };

            return output;
        }

        private static IEnumerable<string> GetOtherArguments()
        {
            var output = new Collection<string>
                             {
                                 $"-l:{LauncherSettings.Get(LauncherSettings.Keys.Language)}",
                                 $"-ignoreMissingModelDataBugs:{(LauncherSettings.Get<bool>(LauncherSettings.Keys.IgnoreMissingModels) ? "on" : "off")}",
                                 $"-ime:{(LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableIme) ? "disabled" : "enabled")}",
                                 $"-writeLog:{(LauncherSettings.Get<bool>(LauncherSettings.Keys.WriteLog) ? "enabled" : "disabled")}"
                             };

            return output;
        }

        private static IEnumerable<string> GetPerformanceArguments()
        {
            var output = new Collection<string>
                             {
                                 $"-intro:{(LauncherSettings.Get<bool>(LauncherSettings.Keys.SkipIntro) ? "off" : "on")}",
                                 $"-exceptionHandling:{(LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableBackgroundLoader) ? "off" : "on")}",
                                 $"-backgroundLoader:{(LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableBackgroundLoader) ? "off" : "on")}"
                             };

            if (LauncherSettings.GetInt(LauncherSettings.Keys.CpuCount) > 0)
            {
                output.Add($"-cpuCount:{LauncherSettings.Get(LauncherSettings.Keys.CpuCount)}");
            }

            if (!string.IsNullOrWhiteSpace(LauncherSettings.Get(LauncherSettings.Keys.CpuPriority)))
            {
                if (Enum.TryParse(LauncherSettings.Get(LauncherSettings.Keys.CpuPriority), true, out CpuPriority priority))
                {
                    output.Add(GetStringForCpuPriority(priority));
                }
                else
                {
                    Log.Warn(
                        $"Unknown CPU priority: \"{LauncherSettings.Get(LauncherSettings.Keys.CpuPriority)}\", skipping argument.");
                }
            }

            if (LauncherSettings.Get<bool>(LauncherSettings.Keys.PauseWhenMinimized))
            {
                output.Add("-gp");
            }

            return output;
        }

        private static string GetStringForCpuPriority(CpuPriority priority)
        {
            var builder = new StringBuilder("-cpuPriority:");

            switch (priority)
            {
                case CpuPriority.Low:
                    builder.Append("low");

                    break;
                case CpuPriority.Medium:
                    builder.Append("medium");

                    break;
                case CpuPriority.High:
                    builder.Append("high");

                    break;
            }

            return builder.ToString();
        }

        private static string GetStringForCursors(CursorColorDepth cursorColorDepth)
        {
            var builder = new StringBuilder("-cursors:");

            switch (cursorColorDepth)
            {
                case CursorColorDepth.Disabled:
                    builder.Append("disabled");

                    break;
                case CursorColorDepth.BlackAndWhite:
                    builder.Append("bw");

                    break;
                case CursorColorDepth.Colors16:
                    builder.Append("color16");

                    break;
                case CursorColorDepth.Colors256:
                    builder.Append("color256");

                    break;
                case CursorColorDepth.FullColors:
                    builder.Append("fullcolor");

                    break;
                default:
                    return string.Empty;
            }

            return builder.ToString();
        }

        private static string GetStringForRenderMode(RenderMode renderMode)
        {
            var builder = new StringBuilder("-d:");
            switch (renderMode)
            {
                case RenderMode.DirectX:
                    builder.Append("directX");
                    break;
                case RenderMode.OpenGl:
                    builder.Append("openGl");
                    break;
                case RenderMode.Software:
                    builder.Append("software");
                    break;
            }

            return builder.ToString();
        }

        private static string GetStringForResolution(string widthTimesHeight, bool depth32)
        {
            if (!ResolutionRegEx.IsMatch(widthTimesHeight))
            {
                throw new ArgumentException(@"Must be in the format \d+x\d+", widthTimesHeight);
            }

            return $"-r{widthTimesHeight}x{(depth32 ? "32" : "16")}";
        }

        private static IEnumerable<string> GetVideoArguments()
        {
            var output = new Collection<string>
                             {
                                 $"-customResolution:{(LauncherSettings.Get<bool>(LauncherSettings.Keys.EnableCustomResolution) ? "enabled" : "disabled")}"
                             };

            if (!string.IsNullOrWhiteSpace(LauncherSettings.Get(LauncherSettings.Keys.Resolution)))
            {
                output.Add(
                    GetStringForResolution(
                        LauncherSettings.Get(LauncherSettings.Keys.Resolution),
                        LauncherSettings.Get<bool>(LauncherSettings.Keys.ColourDepth32Bit)));
            }

            if (!string.IsNullOrWhiteSpace(LauncherSettings.Get(LauncherSettings.Keys.CursorColourDepth)))
            {
                if (Enum.TryParse(LauncherSettings.Get(LauncherSettings.Keys.CursorColourDepth), true, out CursorColorDepth cursorColorDepth))
                {
                    output.Add(
                        $"-customCursors:{(cursorColorDepth == CursorColorDepth.SystemCursors ? "enabled" : "disabled")}");
                    output.Add(GetStringForCursors(cursorColorDepth));
                }
            }

            if (Enum.TryParse(LauncherSettings.Get(LauncherSettings.Keys.RenderMode), out RenderMode renderMode))
            {
                output.Add(GetStringForRenderMode(renderMode));
            }

            output.Add(LauncherSettings.Get<bool>(LauncherSettings.Keys.WindowMode) ? "-w" : "-f");

            return output;
        }
    }
}
