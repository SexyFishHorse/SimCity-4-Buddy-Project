namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    using log4net;
    using NIHEI.SC4Buddy.Configuration;
    using NIHEI.SC4Buddy.Model;
    using OldSettings = NIHEI.SC4Buddy.Properties.Settings;

    public class GameArgumentsHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public enum ColorDepth
        {
            Bits16 = 1,

            Bits32 = 2
        }

        public enum CpuPriority
        {
            Low = 1,

            Medium = 2,

            High = 3
        }

        public enum CursorColorDepth
        {
            Disabled = 1,

            BlackAndWhite = 2,

            Colors16 = 3,

            Colors256 = 4,

            FullColors = 5,

            SystemCursors = 6
        }

        public enum RenderMode
        {
            OpenGl = 1,

            DirectX = 2,

            Software = 3
        }

        public string GetArgumentString(UserFolder selectedUserFolder)
        {
            var arguments = new List<string>();
            arguments.AddRange(GetAudioArguments());
            arguments.AddRange(GetVideoArguments());
            arguments.AddRange(GetPerformanceArguments());
            arguments.AddRange(GetOtherArguments());

            if (selectedUserFolder != null)
            {
                arguments.Add(string.Format("-userDir:\"{0}\\\"", selectedUserFolder.FolderPath));
            }

            return string.Join(" ", arguments.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());
        }

        private IEnumerable<string> GetAudioArguments()
        {
            var output = new Collection<string>
            {
                string.Format("-audio:{0}", LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableAudio) ? "off" : "on"), 
                string.Format("-music:{0}", LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableMusic) ? "off" : "on"), 
                string.Format("-sounds:{0}", LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableSounds) ? "off" : "on")
            };

            return output;
        }

        private IEnumerable<string> GetOtherArguments()
        {
            var output = new Collection<string>
            {
                string.Format("-l:{0}", OldSettings.Default.LauncherLanguage), 
                string.Format(
                    "-ignoreMissingModelDataBugs:{0}", 
                    OldSettings.Default.LauncherIgnoreMissingModels ? "on" : "off"), 
                string.Format("-ime:{0}", OldSettings.Default.LauncherDisableIME ? "disabled" : "enabled"), 
                string.Format("-writeLog:{0}", OldSettings.Default.LauncherWriteLog ? "enabled" : "disabled")
            };

            return output;
        }

        private IEnumerable<string> GetPerformanceArguments()
        {
            var output = new Collection<string>
            {
                string.Format("-intro:{0}", OldSettings.Default.LauncherSkipIntro ? "off" : "on"), 
                string.Format(
                    "-exceptionHandling:{0}", 
                    OldSettings.Default.LauncherDisableExceptionHandling ? "off" : "on"), 
                string.Format("-backgroundLoader:{0}", OldSettings.Default.LauncherDisableBackgroundLoader ? "off" : "on")
            };

            if (OldSettings.Default.LauncherCpuCount > 0)
            {
                output.Add(string.Format("-cpuCount:{0}", OldSettings.Default.LauncherCpuCount));
            }

            if (!string.IsNullOrWhiteSpace(OldSettings.Default.LauncherCpuPriority))
            {
                CpuPriority priority;
                if (Enum.TryParse(OldSettings.Default.LauncherCpuPriority, true, out priority))
                {
                    output.Add(GetStringForCpuPriority(priority));
                }
                else
                {
                    Log.Warn(
                        string.Format(
                            "Unknown CPU priority: \"{0}\", skipping argument.",
                            OldSettings.Default.LauncherCpuPriority));
                }
            }

            if (OldSettings.Default.LauncherPauseMinimized)
            {
                output.Add("-gp");
            }

            return output;
        }

        private string GetStringForCpuPriority(CpuPriority priority)
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

        private string GetStringForCursors(CursorColorDepth cursorColorDepth)
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

        private string GetStringForRenderMode(RenderMode renderMode)
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

        private string GetStringForResolution(string widthTimesHeight, bool depth32)
        {
            var regEx = new Regex(@"\d+x\d+");
            if (!regEx.IsMatch(widthTimesHeight))
            {
                throw new ArgumentException(@"Must be in the format \d+x\d+", widthTimesHeight);
            }

            return string.Format("-r{0}x{1}", widthTimesHeight, depth32 ? "32" : "16");
        }

        private IEnumerable<string> GetVideoArguments()
        {
            var output = new Collection<string>
            {
                string.Format(
                    "-customResolution:{0}", 
                    LauncherSettings.Get<bool>(LauncherSettings.Keys.EnableCustomResolution) ? "enabled" : "disabled")
            };

            if (!string.IsNullOrWhiteSpace(OldSettings.Default.LauncherResolution))
            {
                output.Add(
                    GetStringForResolution(
                        OldSettings.Default.LauncherResolution,
                        OldSettings.Default.Launcher32BitColourDepth));
            }

            if (!string.IsNullOrWhiteSpace(OldSettings.Default.LauncherCursorColour))
            {
                CursorColorDepth cursorColorDepth;
                if (Enum.TryParse(OldSettings.Default.LauncherCursorColour, true, out cursorColorDepth))
                {
                    output.Add(
                        string.Format(
                            "-customCursors:{0}",
                            cursorColorDepth == CursorColorDepth.SystemCursors ? "enabled" : "disabled"));
                    output.Add(GetStringForCursors(cursorColorDepth));
                }
            }

            RenderMode renderMode;
            if (Enum.TryParse(OldSettings.Default.LauncherRenderMode, out renderMode))
            {
                output.Add(GetStringForRenderMode(renderMode));
            }

            output.Add(LauncherSettings.Get<bool>(LauncherSettings.Keys.WindowMode) ? "-w" : "-f");

            return output;
        }
    }
}
