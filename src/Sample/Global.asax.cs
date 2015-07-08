using System;
using System.Web.Http;

namespace Sample
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            new Rocket.Web.Documenter.Core.DocumentHelper().Generate(GlobalConfiguration.Configuration);
        }
    }
}