namespace NIHEI.SC4Buddy.Control
{
    using System.Text;

    public class GameArgumentsController
    {
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

        public string GetStringForAudio(bool enabled)
        {
            return string.Format("-audio:{0}", (enabled ? "on" : "off"));
        }

        public string GetStringForMusic(bool enabled)
        {
            return string.Format("-music:{0}", (enabled ? "on" : "off"));
        }

        public string GetStringForSounds(bool enabled)
        {
            return string.Format("-sounds:{0}", (enabled ? "on" : "off"));
        }

        public string GetStringForCustomResolution(bool enabled)
        {
            return string.Format("-customResolution:{0}", (enabled ? "enabled" : "disabled"));
        }

        public string GetStringForResolution(string width, string height, bool depth32)
        {
            return string.Format("-r{0}x{1}x{2}", width, height, (depth32 ? "32" : "16"));
        }

        public string GetStringForDisplayMode(bool enabled)
        {
            return enabled ? "-w" : "-f";
        }

        public string GetStringForRenderMode(RenderMode renderMode)
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

        public string GetStringForCursors(CursorColorDepth cursorColorDepth)
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
            }

            return builder.ToString();
        }

        public string GetStringForNumberOfCpus(int numCpus)
        {
            return string.Format("-cpuCount:{0}", numCpus);
        }

        public string GetStringForCpuPriority(CpuPriority priority)
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

        public string GetStringForIntroSequence(bool enabled)
        {
            return string.Format("-intro:{0}", (enabled ? "on" : "off"));
        }

        public string GetStringForPauseWhenMinimized(bool enabled)
        {
            return enabled ? "-gp" : string.Empty;
        }

        public string GetStringForExceptionHandling(bool enabled)
        {
            return string.Format("-exceptionHandling:{0}", (enabled ? "on" : "off"));
        }

        public string GetStringForBackgroundLoading(bool enabled)
        {
            return string.Format("-backgroundLoader:{0}", (enabled ? "on" : "off"));
        }

        public string GetStringForIgnoreMissingModels(bool enabled)
        {
            return string.Format("-ignoreMissingModelDataBugs:{0}", (enabled ? "on" : "off"));
        }

        public string GetStringForImeEnabled(bool enabled)
        {
            return string.Format("-ime:{0}", (enabled ? "enabled" : "disabled"));
        }

        public string GetStringForWriteLog(bool enabled)
        {
            return string.Format("-writeLog:" + (enabled ? "enabled" : "disabled"));
        }

        public string GetStringForLanguage(string language)
        {
            return string.Format("-l:{0}", language);
        }
    }
}
