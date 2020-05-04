namespace NetCoreApiSandbox.Infrastructure
{
    #region

    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;

    #endregion

    public class GroupByApiRootConvention: IControllerModelConvention
    {
        #region IControllerModelConvention Members

        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller.Attributes.OfType<RouteAttribute>().FirstOrDefault();

            var apiVersion = controllerNamespace?.Template?.Split('/')?.First()?.ToLower(CultureInfo.CurrentCulture) ??
                             "default";

            controller.ApiExplorer.GroupName = apiVersion;
        }

        #endregion
    }
}
