using java.io;
using java.lang;
using java.text;
using java.util;
using java.util.logging;
using StringWriter = java.io.StringWriter;

namespace BetaSharp.Server;

public class ConsoleFormatter : java.util.logging.Formatter
{
    private SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");

    public override string format(LogRecord logRecord)
    {
        StringBuilder var2 = new StringBuilder();
        var2.append(dateFormat.format(new Date(logRecord.getMillis())));
        Level var3 = logRecord.getLevel();
        if (var3 == Level.FINEST)
        {
            var2.append(" [FINEST] ");
        }
        else if (var3 == Level.FINER)
        {
            var2.append(" [FINER] ");
        }
        else if (var3 == Level.FINE)
        {
            var2.append(" [FINE] ");
        }
        else if (var3 == Level.INFO)
        {
            var2.append(" [INFO] ");
        }
        else if (var3 == Level.WARNING)
        {
            var2.append(" [WARNING] ");
        }
        else if (var3 == Level.SEVERE)
        {
            var2.append(" [SEVERE] ");
        }
        else if (var3 == Level.SEVERE)
        {
            var2.append(" [" + var3.getLocalizedName() + "] ");
        }

        var2.append(logRecord.getMessage());
        var2.append('\n');

        var thrown = logRecord.getThrown();
        if (thrown != null)
        {
            if (thrown is System.Exception netEx) // .NET exception
            {
                var2.append(netEx.ToString());
            }
            else if (thrown is java.lang.Throwable javaEx) // Java Throwable
            {
                StringWriter sw = new();
                javaEx.printStackTrace(new PrintWriter(sw));
                var2.append(sw.toString());
            }
            else // Fallback safety
            {
                var2.append(thrown.ToString());
            }
        }

        return var2.toString();
    }
}