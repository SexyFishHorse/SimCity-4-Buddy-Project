namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;


    using log4net;

    public class GameArgumentsController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public enum RenderMode
        {
            OpenGl = 1, DirectX = 2, Software = 3
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

        public enum CpuPriority
        {
            Low = 1, Medium = 2, High = 3
        }

        protected string GetStringForAudio(bool enabled)
        {
            return string.Format("-audio:{0}", (enabled ? "on" : "off"));
        }

        protected string GetStringForMusic(bool enabled)
        {
            return string.Format("-music:{0}", (enabled ? "on" : "off"));
        }

        protected string GetStringForSounds(bool enabled)
        {
            return string.Format("-sounds:{0}", (enabled ? "on" : "off"));
        }

        protected string GetStringForCustomResolution(bool enabled)
        {
            return string.Format("-customResolution:{0}", (enabled ? "enabled" : "disabled"));
        }

        protected string GetStringForResolution(string widthTimesHeight, bool depth32)
        {
            var regEx = new Regex(@"\d+x\d+");
            if (!regEx.IsMatch(widthTimesHeight))
            {
                throw new ArgumentException(@"Must be in the format \d+x\d+", widthTimesHeight);
            }

            return string.Format("-r{0}x{1}", widthTimesHeight, (depth32 ? "32" : "16"));
        }

        protected string GetStringForWindowMode(bool enabled)
        {
            return enabled ? "-w" : "-f";
        }

        protected string GetStringForRenderMode(RenderMode renderMode)
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

        protected string GetStringForCustomCursors(CursorColorDepth cursorColorDepth)
        {
            return "-customCursors:" + (cursorColorDepth == CursorColorDepth.SystemCursors ? "enabled" : "disabled");
        }

        protected string GetStringForCursors(CursorColorDepth cursorColorDepth)
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

        protected string GetStringForNumberOfCpus(int numCpus)
        {
            return string.Format("-cpuCount:{0}", numCpus);
        }

        protected string GetStringForCpuPriority(CpuPriority priority)
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

        protected string GetStringForIntroSequence(bool enabled)
        {
            return string.Format("-intro:{0}", (enabled ? "on" : "off"));
        }

        protected string GetStringForPauseWhenMinimized(bool enabled)
        {
            return enabled ? "-gp" : string.Empty;
        }

        protected string GetStringForExceptionHandling(bool enabled)
        {
            return string.Format("-exceptionHandling:{0}", (enabled ? "on" : "off"));
        }

        protected string GetStringForBackgroundLoading(bool enabled)
        {
            return string.Format("-backgroundLoader:{0}", (enabled ? "on" : "off"));
        }

        protected string GetStringForIgnoreMissingModels(bool enabled)
        {
            return string.Format("-ignoreMissingModelDataBugs:{0}", (enabled ? "on" : "off"));
        }

        protected string GetStringForImeEnabled(bool enabled)
        {
            return string.Format("-ime:{0}", (enabled ? "enabled" : "disabled"));
        }

        protected string GetStringForWriteLog(bool enabled)
        {
            return string.Format("-writeLog:" + (enabled ? "enabled" : "disabled"));
        }

        protected string GetStringForLanguage(string language)
        {
            return string.Format("-l:{0}", language);
        }
    }
}
