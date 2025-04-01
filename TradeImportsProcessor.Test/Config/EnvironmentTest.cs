using Microsoft.AspNetCore.Builder;

namespace TradeImportsProcessor.Test.Config;

public class EnvironmentTest
{

   [Fact]
   public void IsNotDevModeByDefault()
   { 
       var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
       var isDev = TradeImportsProcessor.Config.Environment.IsDevMode(builder);
       Assert.False(isDev);
   }
}
