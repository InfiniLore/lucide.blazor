// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Serilog;

namespace CodeOfChaosTools.BlazorIcons.Lucide.Setup;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GlobalCatcher {
    public static async Task ExecuteWithGlobalExceptionHandlingAsync(Func<Task> action) {
        try {
            await action.Invoke();
        }
        finally {
            await Log.CloseAndFlushAsync();
        }
    }
}
