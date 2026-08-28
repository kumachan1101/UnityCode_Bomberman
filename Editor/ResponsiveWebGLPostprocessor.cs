#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class ResponsiveWebGLPostprocessor
{
    public const string Marker = "Bomberman responsive mobile canvas";

    [PostProcessBuild(1000)]
    public static void OnPostprocessBuild(BuildTarget target, string outputPath)
    {
        if (target != BuildTarget.WebGL) return;

        string indexPath = Path.Combine(outputPath, "index.html");
        string stylePath = Path.Combine(outputPath, "TemplateData", "style.css");
        if (!File.Exists(indexPath) || !File.Exists(stylePath))
            throw new FileNotFoundException("The WebGL template output is incomplete.");

        File.WriteAllText(indexPath, MakeIndexResponsive(File.ReadAllText(indexPath)));
        File.WriteAllText(stylePath, MakeStyleResponsive(File.ReadAllText(stylePath)));
        Debug.Log("[ResponsiveWebGLPostprocessor] Added mobile resize, orientation and safe-area handling.");
    }

    public static string MakeIndexResponsive(string html)
    {
        if (string.IsNullOrEmpty(html))
            throw new ArgumentException("WebGL index.html is empty.", nameof(html));
        if (html.Contains(Marker)) return html;

        html = html.Replace("\r\n", "\n");
        const string charset = "    <meta charset=\"utf-8\">";
        string viewport = charset + "\n" +
            "    <meta name=\"viewport\" content=\"width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, viewport-fit=cover\">";
        RequireContains(html, charset, "charset meta tag");
        html = html.Replace(charset, viewport);

        const string oldCondition =
            "      if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {";
        const string helpers = @"      // Bomberman responsive mobile canvas
      function isMobileDevice() {
        return /iPhone|iPad|iPod|Android/i.test(navigator.userAgent) ||
          (navigator.maxTouchPoints > 0 && window.matchMedia('(pointer: coarse)').matches);
      }

      function resizeMobileCanvas() {
        var viewport = window.visualViewport;
        var width = Math.max(1, Math.round(viewport ? viewport.width : window.innerWidth));
        var height = Math.max(1, Math.round(viewport ? viewport.height : window.innerHeight));
        container.style.left = Math.round(viewport ? viewport.offsetLeft : 0) + 'px';
        container.style.top = Math.round(viewport ? viewport.offsetTop : 0) + 'px';
        container.style.width = width + 'px';
        container.style.height = height + 'px';
        canvas.style.width = '100%';
        canvas.style.height = '100%';
      }

      function scheduleMobileCanvasResize() {
        window.requestAnimationFrame(resizeMobileCanvas);
        window.setTimeout(resizeMobileCanvas, 250);
      }

      if (isMobileDevice()) {";
        RequireContains(html, oldCondition, "mobile device condition");
        html = html.Replace(oldCondition, helpers);

        const string dynamicViewport = @"
        var meta = document.createElement('meta');
        meta.name = 'viewport';
        meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
        document.getElementsByTagName('head')[0].appendChild(meta);";
        RequireContains(html, dynamicViewport, "generated mobile viewport block");
        html = html.Replace(dynamicViewport, string.Empty);

        const string oneTimeSize = @"        canvas.style.width = window.innerWidth + 'px';
        canvas.style.height = window.innerHeight + 'px';";
        const string responsiveSize = @"        resizeMobileCanvas();
        window.addEventListener('resize', scheduleMobileCanvasResize, { passive: true });
        window.addEventListener('orientationchange', scheduleMobileCanvasResize, { passive: true });
        if (window.visualViewport) {
          window.visualViewport.addEventListener('resize', scheduleMobileCanvasResize, { passive: true });
          window.visualViewport.addEventListener('scroll', scheduleMobileCanvasResize, { passive: true });
        }";
        RequireContains(html, oneTimeSize, "generated one-time mobile canvas size");
        html = html.Replace(oneTimeSize, responsiveSize);

        html = html.Replace(
            "\n        unityShowBanner('WebGL builds are not supported on mobile devices.');",
            string.Empty);
        return html;
    }

    public static string MakeStyleResponsive(string css)
    {
        if (string.IsNullOrEmpty(css))
            throw new ArgumentException("WebGL style.css is empty.", nameof(css));
        if (css.Contains(Marker)) return css;

        return css.TrimEnd() + @"

/* Bomberman responsive mobile canvas */
html, body { width: 100%; height: 100%; overflow: hidden; background: #231F20; }
#unity-container.unity-mobile {
  position: fixed;
  box-sizing: border-box;
  margin: 0;
  padding-top: env(safe-area-inset-top, 0px);
  padding-right: env(safe-area-inset-right, 0px);
  padding-bottom: env(safe-area-inset-bottom, 0px);
  padding-left: env(safe-area-inset-left, 0px);
}
.unity-mobile #unity-canvas {
  display: block;
  width: 100% !important;
  height: 100% !important;
  touch-action: none;
}
";
    }

    private static void RequireContains(string value, string expected, string description)
    {
        if (!value.Contains(expected))
            throw new InvalidOperationException(
                "Unity WebGL template changed; could not find " + description + ".");
    }
}
#endif
